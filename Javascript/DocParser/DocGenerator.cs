using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlus.Json;
using System.IO;

namespace DocPlus.Javascript {
    public class DocGenerator {
        
        /// <summary>
        /// 正在处理的项目配置。
        /// </summary>
        DocProject _project;

        string _templatePath;

        string _outputPath;

        public DocGenerator(DocProject project) {
            _project = project;

            _templatePath = project.TemplateName;
            _outputPath = Path.Combine(project.TargetPath, "data/");

            Directory.CreateDirectory(_outputPath);
        }

        void SaveSimpleComment(DocComment dc, CorePlus.Json.JsonObject obj) {

            switch (dc.MemberType) {
                case "member":
                    if (dc.Type == "Function" || dc[NodeNames.Param] != null || dc[NodeNames.Return] != null) {
                        dc.MemberType = "method";
                    } else {
                        dc.MemberType = "field";
                    }
                    break;
                case null:
                case "":
                    dc.MemberType = "field";
                    break;
            }

            obj.Add("type", dc.MemberType);
            obj.Add("access", dc.MemberAccess);
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
            } else {
                obj.Add(key, value.ToString());
            }

        }

        void SaveComment(DocComment dc, CorePlus.Json.JsonObject obj) {
            obj.Add("fullName", dc.FullName);
            foreach(string key in dc){
                AddSingle(obj, key, dc[key]);
            }
        }

        void SaveJSONP(string filePath, CorePlus.Json.JsonObject obj) {
            File.WriteAllText(filePath, "jsonp(" + obj.ToString() + ");", Encoding.UTF8);
        }

        void GenerateAPI(Dictionary<string, DocComment> comments) {
            CorePlus.Json.JsonObject obj = new CorePlus.Json.JsonObject();

            CorePlus.Json.JsonObject dom = new CorePlus.Json.JsonObject();
            obj["dom"] = dom;

            CorePlus.Json.JsonObject members = new CorePlus.Json.JsonObject();
            obj["members"] = members;

            foreach (var kv in comments) {
                CorePlus.Json.JsonObject t = new CorePlus.Json.JsonObject();
                SaveSimpleComment(kv.Value, t);
                members[kv.Key] = t;

                string[] memberNames = kv.Key.Split('.');
                CorePlus.Json.JsonObject parent = dom;

                int len = memberNames.Length - 1;

                for (int i = 0; i < len; i++) {
                    string memberName = memberNames[i];
                    JsonObject value = parent[memberName] as JsonObject;
                    if (value == null)
                        parent[memberName] = parent = new CorePlus.Json.JsonObject();
                    else
                        parent = value;
                }

                string memberName2 = memberNames[len];
                if (parent[memberName2] == null) {
                    parent.Add(memberName2, 0);
                }
            }

            EnsureObject(dom, members, comments, null);

            SaveJSONP(Path.Combine(_outputPath, "api.js"), obj);
        }

        void EnsureObject(CorePlus.Json.JsonObject dom, CorePlus.Json.JsonObject members, Dictionary<string, DocComment> comments, string parentNamespace) {
            foreach (var vk in dom) {
                string m = parentNamespace + vk.Key;
                if (!comments.ContainsKey(m)) {
                    comments[m] = new DocComment() { MemberType = "Object" };
                    SaveSimpleComment(comments[m], members);
                }
                JsonObject d = vk.Value as JsonObject;
                if (d != null) {
                    EnsureObject(d, members, comments, m + '.');
                }
            }
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
            GenerateAPI(data.DocComments);
            GenerateDetail(data.DocComments);
        }
    }
}
