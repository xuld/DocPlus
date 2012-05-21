using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlus.Json;
using System.IO;

namespace DocPlus.Javascript {
    public class DocGenerator {

        static string GetIcon(DocComment dc) {
            string access = null, type;

            switch(dc.MemberAccess) {
                case null:
                case "public":
                    break;

                case "protected":
                    access = "F";
                    break;

                case "private":
                    access = "T";
                    break;
            }

            switch(dc.MemberType) {
                case "class":
                    type = "C";
                    break;

                case "field":
                    type = "F";
                    break;

                case "method":
                case "function":
                    type = "M";
                    break;

                case "property":
                    type = "P";
                    break;

                case "config":
                    type = "O";
                    break;

                case "event":
                    type = "E";
                    break;

                case "interface":
                    type = "I";
                    break;

                case "enum":
                    type = "U";
                    break;

                default:
                    type = "N";
                    break;
            }

            return access + type;
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
            } else if (value is ArrayProxy<TypeSummary>) {
                var ta = new JsonArray();
                foreach (TypeSummary v in (ArrayProxy<TypeSummary>)value) {
                    var t = new JsonObject();
                    t.Add("type", v.Type);
                    t.Add("summary", v.Summary);
                    ta.Add(t);
                }
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
        void AddMembers(DocComment dc, CorePlus.Json.JsonObject obj, string defines) {
            foreach(var vk in dc.Variant) {
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

        void SaveComment(DocComment dc, CorePlus.Json.JsonObject obj) {
            obj.Add("fullName", dc.FullName);

            // 如果有成员。生成成员字段。
            if(dc.Variant.Count > 0) {
                AddMembers(dc, obj, String.Empty);
            }

            DocComment e;
            if(dc.Extends != null && _data.DocComments.TryGetValue(dc.Extends, out e)) {
                AddMembers(e, obj, e.FullName);
            }
            if(dc.Implements != null) {
                foreach(string im in dc.Implements) {
                    if(_data.DocComments.TryGetValue(im, out e))
                        AddMembers(e, obj, e.FullName);
                }
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

        public void Generate(DocData data) {
            _data = data;
            GenerateAPI(data);
            GenerateDetail(data.DocComments);
        }

    }
}
