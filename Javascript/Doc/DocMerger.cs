using System;
using System.Collections.Generic;
using System.Text;
using DocPlus.Core;
using System.IO;
using CorePlus.Api;
using CorePlus.Parser.Javascript;

namespace DocPlus.Javascript {

    /// <summary>
    /// 分析变量，解析得到 <see cref="ApiDoc"/> 实例。
    /// </summary>
    public class DocMerger {

        /// <summary>
        /// 正在处理的项目配置。
        /// </summary>
        DocProject _project;

        Dictionary<Variant, Member> _caches = new Dictionary<Variant, Member>();

        Dictionary<Variant, Member> _processed = new Dictionary<Variant, Member>();

        /// <summary>
        /// 获取当前生成的最终实例。
        /// </summary>
        public ApiDoc ApiDoc {
            get;
            private set;
        }

        public DocMerger(DocProject project) {
            _project = project;

            ApiDoc = new ApiDoc() { Language = "Javascript" };
        }

        /// <summary>
        /// 解析一个变量树。
        /// </summary>
        /// <param name="scope"></param>
        public void Parse(Variant scope) {

            scope.RemoveVariant("this");
            scope.RemoveVariant("window");
            ParseChildren(ApiDoc.Global, scope, null);
        }

        void ParseChildren(Member parent, Variant obj, bool? isStatic) {

            if(_processed.ContainsKey(obj))
                return;


            _processed[obj] = parent;

            foreach(var value in obj) {

                // 不符合命名标准的不算。
                if(_project.CheckIdentifierStart && !ECMACharType.IsIdentifierStart(obj.Name[0]))
                    continue;

                if(_caches.ContainsKey(value.Value)) {

                    if(!_caches[value.Value].Ignore) {
                        // 解析一个  Member  结果放入  Namespace
                        parent.Members.Add(parent.OwnerDocument.CreateLink(_caches[value.Value]));
                    }
                } else {

                    if(value.Value.Name == null) {
                        value.Value.Name = value.Key;
                    }

                    Member m = ParseMember(value.Value);

                    if(m != null) {

                        if(isStatic.HasValue) {
                            m.IsStatic = isStatic.Value;
                        }

                        _caches[value.Value] = m;

                        if(!m.Ignore) {
                            // 解析一个  Member  结果放入  Namespace
                            parent.Members.Add(m);
                        }

                        if(value.Value.IsConstructor) {
                            Constructor c = ApiDoc.CreateConstructor();
                            FillConstructor(c, obj);

                            if(!c.Ignore)
                                m.Members.Add(c);
                        }
                    }

                }
            }

        }

        /// <summary>
        /// 解析一个变量返回一个 <see cref="Member"/>。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Member ParseMember(Variant obj) {

            switch(obj.MemberType) {
                case MemberType.Class:
                    Class @class = ApiDoc.CreateClass();
                    FillClass(@class, obj);
                    return @class;
                case MemberType.Config:
                    Config @config = ApiDoc.CreateConfig();
                    FillConfig(@config, obj);
                    return @config;
                case MemberType.Default:

                    // 有 prototype 属性的解释为类。
                    if(obj["prototype"] != null)
                        goto case MemberType.Class;

                    if(obj.Type == "Function" || obj.Params != null || obj.ReturnSummary != null || obj.ReturnType != null)
                        goto case MemberType.Method;

                    goto case MemberType.Field;

                case MemberType.Enum:
                    CorePlus.Api.Enum @enum = ApiDoc.CreateEnum();
                    FillEnum(@enum, obj);
                    return @enum;
                case MemberType.Event:
                    Event @event = ApiDoc.CreateEvent();
                    FillEvent(@event, obj);
                    return @event;
                case MemberType.Field:
                    Field field = ApiDoc.CreateField();
                    FillField(field, obj);
                    return field;
                case MemberType.Interface:
                    Interface @interface = ApiDoc.CreateInterface();
                    FillInterface(@interface, obj);
                    return @interface;
                case MemberType.Method:
                    Method method = ApiDoc.CreateMethod();
                    FillMethod(method, obj);
                    return method;
                case MemberType.Package:
                    Namespace ns = ApiDoc.CreateNamespace();
                    FillNamespace(ns, obj);
                    return ns;
                case MemberType.Property:
                    Property property = ApiDoc.CreateProperty();
                    property.PropertyAccess = PropertyAccess.GetSet;
                    FillProperty(property, obj);
                    return property;
                case MemberType.PropertyGetter:
                    Property propertyGetter = ApiDoc.CreateProperty();
                    propertyGetter.PropertyAccess = PropertyAccess.Get;
                    FillProperty(propertyGetter, obj);
                    return propertyGetter;
                case MemberType.PropertySetter:
                    Property propertySetter = ApiDoc.CreateProperty();
                    propertySetter.PropertyAccess = PropertyAccess.Set;
                    FillProperty(propertySetter, obj);
                    return propertySetter;

                default:
                    return null;
            }
        }

        private void FillEvent(Event p, Variant obj) {
            FillMember(p, obj);
        }

        private void FillConfig(Config p, Variant obj) {
            FillField(p, obj);
        }

        private void FillConstructor(Constructor c, Variant obj) {
            FillMethod(c, obj);
        }

        private void FillField(Field p, Variant obj) {
            FillMember(p, obj);

            p.DefaultValue = obj.DefaultValue;
            p.Type = obj.Type;
        }

        private void FillEnum(CorePlus.Api.Enum e, Variant obj) {

            FillType(e, obj);

            // TODO
        }

        private void FillEnumMember(EnumMember em, Variant p) {
            FillMember(em, p);
        }

        private void FillInterface(Interface i, Variant obj) {
            FillType(i, obj);
        }

        private void FillNamespace(Namespace ns, Variant obj) {
            FillMember(ns, obj);
        }

        private void FillClass(Class c, Variant obj) {
            FillStructOrClass(c, obj);
        }

        private void FillStructOrClass(StructOrClass c, Variant obj) {



            Variant prototype = obj["prototype"];

            if(prototype != null) {
                obj.RemoveVariant("prototype");
                ParseChildren(c, prototype, false);
                FillMember(c, obj, true);
                obj["prototype"] = prototype;
            } else {
                FillMember(c, obj, true);
            }

            if(obj.Extends != null) {
                foreach(string s in obj.Extends)
                    c.Extends.Add(c.OwnerDocument.CreateExtends(s));
            }

            if(obj.Implements != null) {
                foreach(string s in obj.Implements)
                    c.Extends.Add(c.OwnerDocument.CreateExtends(s, true));
            }




        }

        private void FillType(CorePlus.Api.Type c, Variant obj) {
            FillMember(c, obj);
        }

        private void FillProperty(Property p, Variant obj) {
            FillMember(p, obj);
        }

        void FillMethod(Method member, Variant obj) {

            FillMember(member, obj);

            if(obj.ReturnType != null || obj.ReturnSummary != null)
                member.Returns = ApiDoc.CreateReturn(obj.ReturnType, obj.ReturnSummary);

            if(obj.Exceptions != null) {

                foreach(string[] s in obj.Exceptions) {

                    member.Exceptions.Add(ApiDoc.CreateException(s[0], s[1]));


                }

            }


            if(obj.Params != null) {

                foreach(string[] s in obj.Params) {

                    //string name = s[1];
                    //string defaultValue = null;
                    //if (name.Length > 0) {
                    //    if (name[0] == '[') {
                    //        defaultValue = String.Empty;
                    //        name = Str.Unsurround(name, '[', ']');
                    //    } else if (name.IndexOf('=') > 0) {
                    //        defaultValue = Str.RemoveBefore(name, '=');
                    //        name = Str.RemoveAfter(name, '=');
                    //    }
                    //}

                    //  member.Params.Add(new Param() { DisplayName = name, Summary = new RemarkNode() { Text = s[2] }, Type = s[0], DefaultValue = defaultValue });

                    member.Params.Add(ApiDoc.CreateParam(s[1], s[2], s[3]));

                }


            }

        }

        void FillMember(Member member, Variant obj, bool? isStatic = null) {

            member.Name = obj.Name;
            member.Category = obj.Category;
            member.Deprecated = obj.Deprecated;
            member.Since = obj.Since;
            member.Source = obj.Source;
            member.SourceLine = obj.StartLocation.Line;

            member.MemberAccess = obj.MemberAccess;
            member.MemberAttribute = obj.MemberAttribute;

            if(obj.Remark != null)
                member.Remark = ApiDoc.CreateRemark(obj.Remark);

            if(obj.Example != null)
                member.Example = ApiDoc.CreateExample(obj.Example);

            if(obj.See != null)
                foreach(string see in obj.See) {
                    member.See.Add(ApiDoc.CreateSee(see));
                }

            if(obj.SeeAlso != null)
                foreach(string see in obj.SeeAlso) {
                    member.SeeAlso.Add(ApiDoc.CreateSeeAlso(see));
                }

            if(obj.Summary != null)
                member.Summary = ApiDoc.CreateSummary(obj.Summary);

            if(obj.Syntax != null)
                member.Syntax = ApiDoc.CreateSyntax(obj.Syntax);


            ParseChildren(member, obj, isStatic);
        }
    }
}
