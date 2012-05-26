using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlus.Json;
using System.IO;
using System.Collections.Specialized;

namespace DocPlus.Javascript {
    public class DocGenerator {

        static string GetIcon(DocComment dc) {
            string access = null;

            if(dc.MemberAccess == null) {
            } else if(dc.MemberAccess == "protected" || dc.MemberAccess == "internal") {
                access = "-protected";
            } else if(dc.MemberAccess == "private") {
                access = "-private";
            }

            return dc.MemberType + access;
        }
        
        /// <summary>
        /// 正在处理的项目配置。
        /// </summary>
        DocProject _project;

        DocData _data;

        string _templatePath;

        string _outputPath;

        public DocGenerator(DocProject project) {
            _project = project;

            _templatePath = project.TemplateName;
            _outputPath = Path.Combine(project.TargetPath, "data/");

            if (_project.ClearBeforeRebuild) {
                Directory.Delete(_outputPath, true);
            }

            Directory.CreateDirectory(_outputPath);
        }

        void AddSingle(CorePlus.Json.JsonObject obj, string key, object value) {

            if (value is string) {
                obj.Add(key, (string)value);
            } else if (value is bool) {
                obj.Add(key, (bool)value);
            } else if (value is TypeSummary) {
                var t =  new JsonObject();
                var v = (TypeSummary)value;
                t.Add("type", v.Type);
                t.Add("summary", v.Summary);
                obj[key] = t;
            }else if(value is ArrayProxy<string>){
                var t = new JsonArray();
                foreach (string v in (ArrayProxy<string>)value) {
                    t.Add(v);
                }
                obj[key] = t;
            } else if (value is ArrayProxy<TypeSummary>) {
                var ta = new JsonArray();
                foreach (TypeSummary v in (ArrayProxy<TypeSummary>)value) {
                    var t = new JsonObject();
                    t.Add("type", v.Type);
                    t.Add("summary", v.Summary);
                    ta.Add(t);
                }
                obj[key] = ta;
            } else if(value is ParamInfoCollection) {
                var ta = new JsonArray();
                foreach(ParamInfo v in (ParamInfoCollection)value) {
                    var t = new JsonObject();
                    t.Add("type", v.Type);
                    t.Add("name", v.Name);
                    t.Add("defaultValue", v.DefaultValue);
                    t.Add("summary", v.Summary);
                    ta.Add(t);
                }
                obj["params"] = ta;
            } else {
                obj.Add(key, value.ToString());
            }

        }

        /// <summary>
        /// Adds the members.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="defines">要操作的字符串。</param>
        void AddMembers(Variant v, CorePlus.Json.JsonObject obj, string defines) {
            foreach(var vk in v) {
                Variant value = vk.Value;

                string names = "members";

                switch(value.DocComment.MemberType) {
                    case "method":
                    case "function":
                        names = "methods";
                        break;
                    case "field":
                        names = "fields";
                        break;
                    case "config":
                        names = "configs";
                        break;
                    case "property":
                    case "getter":
                    case "setter":
                        names = "properties";
                        break;
                    case "event":
                        names = "events";
                        break;

                }

                JsonArray arr = obj[names] as JsonArray;

                if(arr == null) {
                    obj[names] = arr = new JsonArray();
                }

                JsonObject values = new JsonObject();
                arr.Add(values);

                values.Add("name", value.DocComment.Name);
                values.Add("icon", GetIcon(value.DocComment));
                values.Add("fullName", value.DocComment.FullName);
                values.Add("summary", value.DocComment.Summary);
                values.Add("isStatic", !value.DocComment.IsMember);
                values.Add("defines", defines);
            }
        }

        Dictionary<string, List<string>> _extendsInfo = new Dictionary<string, List<string>>();

        void GetExtendsInfo(Dictionary<string, DocComment> comments) {
            foreach(var vk in comments) {
                string extends = vk.Value.Extends;
                if(extends != null) {


                     List<string> buffer;

                     if(!_extendsInfo.TryGetValue(extends, out buffer)) {
                         _extendsInfo[extends] = buffer = new List<string>();
                     }


                     buffer.Add(vk.Key);

                }
            }
        }

        Dictionary<string, List<KeyValuePair<string, DocComment>>> _sourceInfo = new Dictionary<string,List< KeyValuePair<string, DocComment>>>();

        void GetSourceInfo(Dictionary<string, DocComment> comments) {
            foreach(var vk in comments) {

                if(vk.Value.Source != null) {

                    List<KeyValuePair<string, DocComment>> list;

                    if(!_sourceInfo.TryGetValue(vk.Value.Source, out list)) {
                        _sourceInfo[vk.Value.Source] = list = new List<KeyValuePair<string, DocComment>>();
                    }

                    list.Add(vk);

                }
            }
        }

        void SaveComment(DocComment dc, CorePlus.Json.JsonObject obj) {
            string fullName = dc.FullName;
            obj.Add("fullName",fullName);
            obj.Add("source", dc.Source);


            if(dc.Source != null) {
                obj.Add("sourceFile", "data/source/" + dc.Source + ".html#" + fullName.Replace('.', '-'));
            }

            if (dc.MemberType == "class" && dc.Variant.Members != null) {
                AddMembers(dc.Variant.Members, obj, String.Empty);
            }

            // 如果有成员。生成成员字段。
            if(dc.Variant.Count > 0) {
                AddMembers(dc.Variant, obj, String.Empty);
            }

            DocComment e;
            if(dc.Extends != null && _data.DocComments.TryGetValue(dc.Extends, out e)) {
                string extends = dc.Extends;
                if (e.Variant.Members != null) {
                    AddMembers(e.Variant.Members, obj, extends);
                }

                JsonArray baseClasses = new JsonArray();

                obj["baseClasses"] = baseClasses;
                while (extends != null && _data.DocComments.TryGetValue(extends, out e)) {
                    baseClasses.Insert(0, extends);

                    extends = e.Extends;
                }
            }
            if(dc.Implements != null) {
                foreach(string im in dc.Implements) {
                    if(_data.DocComments.TryGetValue(im, out e))
                        AddMembers(e.Variant, obj, e.FullName);
                }
            }


            if(_extendsInfo.ContainsKey(fullName)) {

                JsonArray subClasses = new JsonArray();

                obj["subClasses"] = subClasses;
                var list = _extendsInfo[fullName];
                list.Sort();
                subClasses.AddRange(list);
            }

            foreach(string key in dc){
                AddSingle(obj, key, dc[key]);
            }
        }

        void SaveJSONP(string filePath, CorePlus.Json.JsonObject obj) {
            File.WriteAllText(filePath, "jsonp(" + obj.ToString() + ");", Encoding.UTF8);
        }

        void SaveSimpleComment(DocComment dc, CorePlus.Json.JsonObject obj) {

            obj.Add("type", dc.MemberType);
            obj.Add("icon", GetIcon(dc));
        }

        void GenerateAPISignle(CorePlus.Json.JsonObject dom, Variant value) {

            // 如果有子成员。
            if(value.Count > 0){
                CorePlus.Json.JsonObject parent = new JsonObject();
                dom.Add(value.DocComment.Name, parent);
                foreach(var vk in value) {
                    GenerateAPISignle(parent, vk.Value);
                }


            } else {
                dom.Add(value.DocComment.Name, 0);
            }

                


        }

        void GenerateAPI(DocData value) {
            CorePlus.Json.JsonObject obj = new CorePlus.Json.JsonObject();

            CorePlus.Json.JsonObject dom = new CorePlus.Json.JsonObject();
            obj["dom"] = dom;



            foreach(var vk in value.Global) {
                GenerateAPISignle(dom, vk.Value);
            }

            CorePlus.Json.JsonObject members = new CorePlus.Json.JsonObject();
            obj["members"] = members;

            foreach (var kv in value.DocComments) {
                CorePlus.Json.JsonObject t = new CorePlus.Json.JsonObject();
                SaveSimpleComment(kv.Value, t);
                members[kv.Key] = t;
            }

            SaveJSONP(Path.Combine(_outputPath, "api.js"), obj);
        }

        void GenerateDetail(Dictionary<string, DocComment> comments) {

            string api = _outputPath + "api/";

            Directory.CreateDirectory(api);

            foreach (var kv in comments) {
                CorePlus.Json.JsonObject obj = new CorePlus.Json.JsonObject();
                SaveComment(kv.Value, obj);

                SaveJSONP(api + kv.Key + ".js", obj);
            }
        }

        void GenerateSource(NameValueCollection comments) {

            string source = _outputPath + "source/";

            Directory.CreateDirectory(source);

            foreach(string key in comments) {
                string[] lines = File.ReadAllLines(comments[key], _project.Encoding ?? Encoding.UTF8);

                for(int i = 0; i < lines.Length; i++){
                    lines[i] = CorePlus.Core.Str.HtmlEncode(lines[i]);
                }

                List<KeyValuePair<string, DocComment>> list;
                if(_sourceInfo.TryGetValue(key, out list)) {

                    foreach(var vk in list) {
                        string id = vk.Key.Replace('.', '-');


                        DocComment dc = vk.Value;


                        int line = dc.EndLocation.Line - 1;

                        lines[line] = lines[line].Insert(dc.EndLocation.Column - 1, "</span>");
                        line = dc.StartLocation.Line - 1;

                        lines[line] = lines[line].Insert(dc.StartLocation.Column - 1, "<span id=\"" + id + "\">");
                    }

                }

                lines[0] = @"<!doctype html>
<html>
	<head>
		<meta charset=""utf-8"">
		<title>" + key + @" 源码</title>
		<link href=""../../assets/styles/prettify.css"" type=""text/css"" rel=""stylesheet"" />
		<script src=""../../assets/scripts/prettify.js"" type=""text/javascript""></script>
		<style type=""text/css"">.highlight { display: block; background-color: #ddd; }</style>
</head>
<body onload=""setTimeout('prettyPrint()', 0);var node = document.getElementById(location.hash.replace(/#/, ''));if(node)node.className = 'highlight';""><pre class=""prettyprint lang-js"">" + lines[0];

                lines[lines.Length - 1] += @"</pre>
</body>
</html>";

                string path = source + key + ".html";


                Directory.CreateDirectory(Path.GetDirectoryName(path));

                File.WriteAllLines(path, lines, Encoding.UTF8);
            }
        }

        void CopyBasicTemplate() {
            if(Directory.Exists(_templatePath))
                CorePlus.IO.FileHelper.CopyDirectory(_templatePath, _project.TargetPath);
        }

        public void Generate(DocData data) {
            _data = data;
            CopyBasicTemplate();

            GetExtendsInfo(data.DocComments);
            GenerateAPI(data);
            GenerateDetail(data.DocComments);

            GetSourceInfo(data.DocComments);
            GenerateSource(data.Files);

        }

    }
}
