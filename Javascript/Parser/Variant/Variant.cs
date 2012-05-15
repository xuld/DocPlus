using System.Collections.Generic;
using System.Collections.Specialized;
using CorePlus.Parser.Javascript;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个文档注释变量。
    /// </summary>
    public class Variant : NameObjectCollectionBase {

        #region 注释自身属性

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
        /// 获取或设置定义当前变量的源文件。
        /// </summary>
        public string Source {
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

        #endregion

        #region 计算方法

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

        public static bool IsNumberic(string type) {
            return type == "Number" || type == "Boolean" || type == "Null";
        }

        public static double ConvertToNumber(object value) {
            if(value is double)
                return (double)value;

            if(value is bool)
                return (bool)value ? 1d : 0d;

            string strValue = value.ToString();
            double dblValue;
            if(double.TryParse(strValue, out dblValue))
                return dblValue;

            if(strValue == "null")
                return 0d;

            return double.NaN;
        }

        public static bool ConvertToBoolean(object value) {
            if(value is bool)
                return (bool)value;

            if(value is double)
                return (double)value > 0;

            if(value is string)
                return ((string)value).Length > 0;

            return true;
        }

        public string GetParamType(string p) {
            if(Params == null)
                return null;
            foreach(string[] pp in Params) {
                if(pp[1] == p)
                    return pp[0];
            }
            return null;
        }

        public static Variant Create(string type) {
            switch(type) {
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

        #region 内置常量

        public readonly static Variant Null = new Variant() { Attribute = VariantAttribute.PredefinedMember, Type = "Null", Value = Null };

        public readonly static Variant Undefined = new Variant() { Attribute = VariantAttribute.PredefinedMember, Type = "Undefined" };

        public readonly static Variant True = new Variant() { Attribute = VariantAttribute.PredefinedMember, Type = "Boolean", Value = true };

        public readonly static Variant False = new Variant() { Attribute = VariantAttribute.PredefinedMember, Type = "Boolean", Value = false };

        #endregion

        #region 系统函数

        /// <summary>
        /// 获取当前标签的注释的条数。
        /// </summary>
        public int CommentCount {
            get {
                return base.Count;
            }
        }

        /// <summary>
        /// 获取指定节点对应的值。
        /// </summary>
        /// <param name="nodeName">节点名。</param>
        /// <returns>对应的值。</returns>
        public object GetComment(string nodeName) {
            return BaseGet(nodeName);
        }

        /// <summary>
        /// 设置指定节点对应的值。
        /// </summary>
        /// <param name="nodeName">节点名。</param>
        /// <returns>对应的值。</returns>
        public void SetComment(string nodeName, object value) {
            BaseSet(nodeName, value);
        }

        /// <summary>
        /// 自动填充指定的注释。
        /// </summary>
        /// <param name="nodeName">节点名。</param>
        /// <param name="value">值。</param>
        public void AutoFill(string nodeName, object value) {
            if(BaseGet(nodeName) == null) {
                BaseSet(nodeName, value);
            }
        }

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
        }

        /// <summary>
        /// 将指定变量的注释合并到当前变量。
        /// </summary>
        /// <param name="src"></param>
        public void Merge(Variant src) {
            for(int i = 0; i < src.Count; i++) {
                BaseSet(src.BaseGetKey(i), src.BaseGet(i));
            }
        }

        #endregion

        #region Node2Property

        /// <summary>
        /// 获取或设置注释节点名。
        /// </summary>
        public string Name {
            get {
                return (string)GetComment(NodeNames.Name);
            }
            set {
                SetComment(NodeNames.Name, value);
            }
        }

        /// <summary>
        /// 获取或设置注释节点名。
        /// </summary>
        public string Type {
            get {
                return (string)GetComment(NodeNames.Type);
            }
            set {
                SetComment(NodeNames.Type, value);
            }
        }

        /// <summary>
        /// 获取或设置注释节点名。
        /// </summary>
        public object Value {
            get {
                return GetComment(NodeNames.Value);
            }
            set {
                SetComment(NodeNames.Value, value);
            }
        }

        #endregion

    }

}
