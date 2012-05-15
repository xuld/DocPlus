using System;
using System.Collections.Generic;
using System.Text;
using CorePlus.Parser.Javascript;
using CorePlus.Api;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个变量。
    /// </summary>
    public class Variant :IEnumerable<KeyValuePair<string, Variant>> {

        #region 内置常量

        public readonly static Variant Null = new Variant() { Attribute = VariantAttribute.PredefinedMember, Type = "Null", Value = Null };

        public readonly static Variant Undefined = new Variant() { Attribute = VariantAttribute.PredefinedMember, Type = "Undefined" };

        public readonly static Variant True = new Variant() { Attribute = VariantAttribute.PredefinedMember, Type = "Boolean", Value = true };

        public readonly static Variant False = new Variant() { Attribute = VariantAttribute.PredefinedMember, Type = "Boolean", Value = false };

        #endregion

        #region 必须的属性

        /// <summary>
        /// 获取或设置当前变量的名字。
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前变量的类型。
        /// </summary>
        public string Type {
            get;
            set;
        }

        /// <summary>
        ///  获取或设置当前成员的 <see cref="MemberType"/> 。
        /// </summary>
        public MemberType MemberType {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前成员的 <see cref="MemberAttributes"/> 。
        /// </summary>
        public MemberAttributes MemberAttribute {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前成员的 <see cref="MemberAccess"/> 。
        /// </summary>
        public MemberAccess MemberAccess {
            get;
            set;
        }

        bool _ignore;

        /// <summary>
        /// 获取当前变量是否应该在输出时忽略。
        /// </summary>
        public bool Ignore {
            get {
                return _ignore || Attribute != VariantAttribute.DocumentedMember;
            }
            set {
                _ignore = true;
            }
        }

        /// <summary>
        /// 获取或设置变量在源码定义的位置。
        /// </summary>
        public Location StartLocation {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置变量在源码定义的位置。
        /// </summary>
        public Location EndLocation {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前变量的来源。
        /// </summary>
        public VariantAttribute Attribute {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前变量准备设置的子变量的名字空间。
        /// </summary>
        public string NamespaceSetter {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前变量准备设置的子变量的名字空间。
        /// </summary>
        internal Variant NamespaceSetterVariant;

        /// <summary>
        /// 获取或设置用户定义的当前成员的父成员。
        /// </summary>
        public string MemberOf {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置绑定到当前变量的值。
        /// </summary>
        public object Value {
            get;
            set;
        }

        /// <summary>
        ///  获取或设置用户定义的返回值的描述。
        /// </summary>
        public string ReturnSummary {
            get;
            set;
        }

        /// <summary>
        ///  获取或设置用户定义的返回值的类型。
        /// </summary>
        public string ReturnType {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前变量的所属源文件。
        /// </summary>
        public Source Source {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否在文档内隐藏当前变量。
        /// </summary>
        public bool Hide {
            get;
            set;
        }

        ///// <summary>
        ///// 获取当前变量对应的内置类型。
        ///// </summary>
        //public NativeType NativeType {
        //    get {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool IsValueType {
        //    get {
        //        switch(Type){
        //            case "Null":
        //            case "Undefined":
        //            case "Number":
        //            case "Boolean":
        //                return true;
        //            default:
        //                return false;
        //        }
        //    }
        //}

        internal bool Processed;

        #endregion

        #region 必须的方法

        /// <summary>
        /// 返回当前变量转为布尔类型的值。
        /// </summary>
        /// <returns></returns>
        public bool ToBoolean() {
            return ConvertToBoolean(Value);
        }

        /// <summary>
        /// 返回当前变量转为数字类型的值。
        /// </summary>
        /// <returns></returns>
        public double ToNumber() {
            return ConvertToNumber(Value);
        }

        /// <summary>
        /// 返回当前变量转为数字类型的值。
        /// </summary>
        /// <returns></returns>
        public int ToInteger() {
            return unchecked((int)ToNumber());
        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            return Name;
        }

        #endregion

        #region 子变量

        Map<string, Variant> _children = new Map<string, Variant>();

        /// <summary>
        /// 初始化 <see cref="DocPlus.Javascript.Variant"/> 类的新实例。
        /// </summary>
        public Variant() {
           
        }

        /// <summary>
        /// 初始化 <see cref="DocPlus.Javascript.Variant"/> 类的新实例。
        /// </summary>
        public Variant(Location startLocation, Location endLocation) {
            StartLocation = startLocation;
            EndLocation = endLocation;
            Attribute = VariantAttribute.DocumentedMember;
        }

        ///// <summary>
        ///// 获取或设置当前变量所有的注释。
        ///// </summary>
        //public Comment Comment {
        //    get;
        //    set;
        //}

        public int VariantCount {
            get {
                return _children.Count;
            }
        }

        /// <summary>
        /// 获取或设置当前变量所有的注释。
        /// </summary>
        public void RemoveVariant(string name) {
            _children.Remove(name);
        }

        /// <summary>
        /// 获取或设置当前变量所有的注释。
        /// </summary>
        public void RemoveVariant(Variant v) {
            foreach (var k in _children) {
                if (k.Value == v) {
                    RemoveVariant(k.Key);
                    return;
                }
            }
        }

        /// <summary>
        /// 获取或设置当前变量内的子变量。
        /// </summary>
        /// <param name="name">变量名。</param>
        /// <returns>变量实例。</returns>
        public Variant this[string name] {
            get {
                return HasVariant(name) ? _children[name] : null;
            }
            set {
                if (value == null) {
                    if (HasVariant(name)) {
                        _children.Remove(name);
                    }
                } else {
                    _children[name] = value;
                }
            }
        }

        /// <summary>
        /// 获取指定的子成员，如果这个变量不存在，则生成。
        /// </summary>
        /// <param name="ns">变量所在的名字空间。</param>
        public bool HasVariant(string name) {
            return _children.ContainsKey(name);
        }

        /// <summary>
        /// 获取指定的子成员，如果这个变量不存在，则返回 null。此方法会遍历原型链。
        /// </summary>
        /// <param name="ns">变量所在的名字空间。</param>
        public Variant GetVariant(string name) {
            if (!this.HasVariant(name))
                return null;
            return _children[name];
        }

        public Variant GetOrCreateVariant(string name) {
            if(HasVariant(name)) {
                return GetVariant(name);
            }

            return this[name] = new Variant() { Name = name };
        }

        /// <summary>
        /// 获取指定的子成员，如果这个变量不存在，则生成。
        /// </summary>
        /// <param name="ns">变量所在的名字空间。</param>
        public Variant GetOrCreateNamespace(string ns) {
            Variant v = this;
            foreach (string s in ns.Split('.')) {
                if (!v.HasVariant(s))
                    v[s] = new Variant() { Name = s };
                v = v._children[s];
            }

            return v;
        }

        #endregion

        #region 属性

        #region 顶级

        public string Author {
            get {
                return GetValue1(CommentNode.Author);
            }
            set {
                SetValue1(CommentNode.Author, value);
            }
        }

        public string ProjectDescription {
            get {
                return GetValue1(CommentNode.ProjectDescription);
            }
            set {
                SetValue1(CommentNode.ProjectDescription, value);
            }
        }

        public string License {
            get {
                return GetValue1(CommentNode.License);
            }
            set {
                SetValue1(CommentNode.License, value);
            }
        }

        public string Version {
            get {
                return GetValue1(CommentNode.Version);
            }
            set {
                SetValue1(CommentNode.Version, value);
            }
        }

        public string FileOverview {
            get {
                return GetValue1(CommentNode.FileOverview);
            }
            set {
                SetValue1(CommentNode.FileOverview, value);
            }
        }

        public string File {
            get {
                return GetValue1(CommentNode.File);
            }
            set {
                SetValue1(CommentNode.File, value);
            }
        }

        public string Requires {
            get {
                return GetValue1(CommentNode.Requires);
            }
            set {
                SetValue1(CommentNode.Requires, value);
            }
        }

        #endregion

        #region 类型说明

        public void UpdateNamespaceSetter() {
            if (MemberType == MemberType.Class || MemberType == MemberType.Interface) {
                NamespaceSetter = MemberOf == null ? Name : (MemberOf + "." + Name);
            }
        }

        ///// <summary>
        ///// 获取当前注释中表示成员的类型的标签。
        ///// </summary>
        //public CommentNode Member {
        //    set {
        //        if (value == CommentNode.Class) {
        //            MemberType = MemberType.Class;
        //            NamespaceSetter = MemberOf == null ? DisplayName : (MemberOf + "." + DisplayName);
        //        } else if (value == CommentNode.Interface) {
        //            MemberType = MemberType.Interface;
        //            NamespaceSetter = MemberOf == null ? DisplayName : (MemberOf + "." + DisplayName);
        //        } else if (value == CommentNode.Config) {
        //            MemberType = MemberType.Config;
        //        } else if (value == CommentNode.Enum) {
        //            MemberType = MemberType.Enum;
        //        } else if (value == CommentNode.Event) {
        //            MemberType = MemberType.Event;
        //        } else if (value == CommentNode.Field) {
        //            MemberType = MemberType.Field;
        //        } else if (value == CommentNode.Method) {
        //            MemberType = MemberType.Method;
        //        } else if (value == CommentNode.Property) {
        //            MemberType = MemberType.Property;
        //        } else if (value == CommentNode.Getter) {
        //            MemberType = MemberType.PropertyGetter;
        //        } else if (value == CommentNode.Setter) {
        //            MemberType = MemberType.PropertySetter;
        //        } else{
        //            MemberType = MemberType.Default;
        //        }
        //    }
        //}

        /// <summary>
        /// 获取当前注释中表示访问属性的标签。
        /// </summary>
        public CommentNode MemberAccessNode {
            //get {
            //    if (value == CommentNode.Public) {
            //        member.MemberAccess = MemberAccess.Public;
            //    } else if (dc.MemberAccess == CommentNode.Protected) {
            //        member.MemberAccess = MemberAccess.Protected;
            //    } else if (dc.MemberAccess == CommentNode.Private) {
            //        member.MemberAccess = MemberAccess.Private;
            //    } else if (dc.MemberAccess == CommentNode.Namespace) {
            //        member.MemberAccess = MemberAccess.Namespace;
            //    } else if (dc.MemberAccess == CommentNode.Internal) {
            //        member.MemberAccess = MemberAccess.Internal;
            //    } else if (dc.MemberAccess == CommentNode.ProtectedInternal) {
            //        member.MemberAccess = MemberAccess.ProtectedInternal;
            //    }
            //}
            set {
                if (value == CommentNode.Public) {
                    MemberAccess = MemberAccess.Public;
                } else if (value == CommentNode.Protected) {
                    MemberAccess = MemberAccess.Protected;
                } else if (value == CommentNode.Private) {
                    MemberAccess = MemberAccess.Private;
                } else if (value == CommentNode.Namespace) {
                    MemberAccess = MemberAccess.Namespace;
                } else if (value == CommentNode.Internal) {
                    MemberAccess = MemberAccess.Internal;
                } else {
                    MemberAccess = MemberAccess.Default;
                }
            }
        }

        /// <summary>
        /// 获取当前注释中表示属性的标签。
        /// </summary>
        public CommentNode MemberAttributeNode {
          //  get;
            set {
                if (value == CommentNode.Abstract) {
                    MemberAttribute = MemberAttributes.Abstract;
                } else if (value == CommentNode.Override) {
                    MemberAttribute = MemberAttributes.Override;
                } else if (value == CommentNode.Static) {
                    MemberAttribute = MemberAttributes.Static;
                } else if (value == CommentNode.Virtual) {
                    MemberAttribute = MemberAttributes.Virtual;
                } else {
                    MemberAttribute = MemberAttributes.None;
                }
            }
        }

        public string[] Extends {
            get {
                return _otherTags[CommentNode.Extends];
            }
            set {
                _otherTags[CommentNode.Extends] = value;
            }
        }

        public string[] Implements {
            get {
                return _otherTags[CommentNode.Implements];
            }
            set {
                _otherTags[CommentNode.Implements] = value;
            }
        }

        public bool IsConstructor {
            get;
            set;
        }

        public bool System {
            get;
            set;
        }

        public string DefaultValue {
            get {
                return GetValue1(CommentNode.DefaultValue);
            }
            set {
                SetValue1(CommentNode.DefaultValue, value);
            }
        }

        #endregion

        #region 公共

        string GetValue1(CommentNode node) {
            return _otherTags.ContainsKey(node) ? _otherTags[node][0] : null;
        }

        void SetValue1(CommentNode node, string value) {
            _otherTags[node] = new string[] { value };
        }

        public string Summary {
            get {
                return GetValue1(CommentNode.Summary);
            }
            set {
                SetValue1(CommentNode.Summary, value);
            }
        }

        public string Remark {
            get {
                return GetValue1(CommentNode.Remark);
            }
            set {
                SetValue1(CommentNode.Remark, value);
            }
        }

        public string Example {
            get {
                return GetValue1(CommentNode.Example);
            }
            set {
                SetValue1(CommentNode.Example, value);
            }
        }

        public string Syntax {
            get {
                return GetValue1(CommentNode.Syntax);
            }
            set {
                SetValue1(CommentNode.Syntax, value);
            }
        }

        public string Category {
            get {
                return GetValue1(CommentNode.Category);
            }
            set {
                SetValue1(CommentNode.Category, value);
            }
        }

        public string[] See {
            get {
                return _otherTags[CommentNode.See];
            }
            set {
                _otherTags[CommentNode.See] = value;
            }
        }

        public string[] SeeAlso {
            get {
                return _otherTags[CommentNode.SeeAlso];
            }
            set {
                _otherTags[CommentNode.SeeAlso] = value;
            }
        }

        public string Since {
            get {
                return GetValue1(CommentNode.Since);
            }
            set {
                SetValue1(CommentNode.Since, value);
            }
        }

        public string Deprecated {
            get {
                return GetValue1(CommentNode.Deprecated);
            }
            set {
                SetValue1(CommentNode.Deprecated, value);
            }
        }

        /// <summary>
        /// 获取或设置当前成员的别名。
        /// </summary>
        public string[] Alias {
            get {
                return _otherTags[CommentNode.Alias];
            }
            set {
                _otherTags[CommentNode.Alias] = value;
            }
        }

        /// <summary>
        /// 获取当前注释的所有参数，如果没有参数，返回 null 。
        /// </summary>
        public System.Collections.Generic.List<string[]> Params {
            get;
            set;
        }

        /// <summary>
        /// 获取当前注释的所有异常，如果没有异常，返回 null 。
        /// </summary>
        public System.Collections.Generic.List<string[]> Exceptions {
            get;
            set;
        }

        #endregion

        #region 注释

        ///// <summary>
        ///// 获取指定类型的第一个节点。
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public CommentNode GetCommentNode(CommentNodeType type) {
        //    foreach(var k in this){
        //        if (k.Key.Type == type) {
        //            return k.Key;
        //        }
        //    }

        //    return null;
        //}

        Map<CommentNode, string[]> _otherTags = new Map<CommentNode, string[]>();

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public string GetRaw() {
            StringBuilder sb = new StringBuilder();

            if (Ignore)
                sb.AppendLine("@ignore");

            if (Name != null) {
                sb.Append("@name = ");
                sb.Append(Name);
                sb.AppendLine();
            }

            if (Type != null) {
                sb.Append("@type = ");
                sb.Append(Type);
                sb.AppendLine();
            }

            foreach (var item in _otherTags) {
                sb.Append('@');
                sb.Append(item.Key);
                sb.Append(" = ");
                foreach (string s in item.Value) {
                    sb.Append(s);
                    sb.Append(' ');
                }
                sb.AppendLine();
            }

            if (Params != null) {
                foreach (string[] pp in Params) {
                    sb.Append("@param ");
                    foreach (string s in pp) {
                        sb.Append(s);
                        sb.Append(' ');
                    }
                    sb.AppendLine();
                }
            }

            if (Exceptions != null) {
                foreach (string[] pp in Exceptions) {
                    sb.Append("@exception ");
                    foreach (string s in pp) {
                        sb.Append(s);
                        sb.Append(' ');
                    }
                    sb.AppendLine();
                }
            }

            if (MemberType != MemberType.Default) {
                sb.Append("@member = ");
                sb.Append(MemberType);
                sb.AppendLine();
            }

            if (MemberAttribute != MemberAttributes.None) {
                sb.Append("@attribute = ");
                sb.Append(MemberAttribute);
                sb.AppendLine();
            }

            if (MemberAccess != CorePlus.Api.MemberAccess.Default) {
                sb.Append("@access = ");
                sb.Append(MemberAccess);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string[] this[CommentNode node] {
            get {
                return _otherTags[node];
            }
            set {
                _otherTags[node] = value;
            }
        }

        public bool ContainsKey(CommentNode node) {
            return _otherTags.ContainsKey(node);
        }

        /// <summary>
        /// 把当前变量的值拷贝到指定的变量内。
        /// </summary>
        /// <param name="v">变量。</param>
        public void CopyTo(Variant node) {

            foreach (var a in _otherTags) {
                if (!node.ContainsKey(a.Key)) {
                    node._otherTags[a.Key] = a.Value;
                }
            }


            node.Params = node.Params ?? Params;
            node.Exceptions = node.Exceptions ?? Exceptions;

            if (node.MemberAccess != CorePlus.Api.MemberAccess.Default) {
                node.MemberAccess = MemberAccess;
            }

            if (node.MemberAttribute != CorePlus.Api.MemberAttributes.None) {
                node.MemberAttribute = MemberAttribute;
            }

            if (node.MemberType != MemberType.Default) {
                node.MemberType = MemberType;
            }

            node.ReturnType = node.ReturnType ?? ReturnType;
            node.ReturnSummary = node.ReturnSummary ?? ReturnSummary;

            node.IsConstructor = IsConstructor;
            node.Ignore = node.Ignore || Ignore;
            node.Type = node.Type ?? Type;
            node.Name = node.Name ?? Name;
            node.MemberOf = node.MemberOf ?? MemberOf;
        }

        #endregion

        #endregion

        #region IEnumerable<KeyValuePair<string,Variant>> 成员

        public IEnumerator<KeyValuePair<string, Variant>> GetEnumerator() {
            return _children.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _children.GetEnumerator();
        }

        #endregion

        public static bool IsNumberic(string type) {
            return type == "Number" || type == "Boolean" || type == "Null";
        }

        public static double ConvertToNumber(object value) {
            if (value is double)
                return (double)value;

            if (value is bool)
                return (bool)value ? 1d : 0d;

            string strValue = value.ToString();
            double dblValue;
            if (double.TryParse(strValue, out dblValue))
                return dblValue;

            if (strValue == "null")
                return 0d;

            return double.NaN;
        }

        public static bool ConvertToBoolean(object leftValue) {
            if (leftValue is bool)
                return (bool)leftValue;

            if (leftValue is double)
                return (double)leftValue > 0;

            if (leftValue is string)
                return ((string)leftValue).Length > 0;

            return true;
        }

        public string GetParamType(string p) {
            if (Params == null)
                return null;
            foreach (string[] pp in Params) {
                if (pp[1] == p)
                    return pp[0];
            }
            return null;
        }

        public static Variant Create(string type) {
            switch (type) {
                case null:
                case "Undefined":
                    return Undefined;
                case "Null":
                    return Null;
                case "Boolean":
                    return False;
                default:
                    return new Variant() { Type = type };
            }
        }
    }

}
