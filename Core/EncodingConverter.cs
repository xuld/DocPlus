using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DocPlus.Core {

    /// <summary>
    /// 提供在编码与其他表示形式之间实现相互转换的类型转换器。
    /// </summary>
    public class EncodingConverter : System.ComponentModel.TypeConverter {

        Encoding[] _knownEncodings = new Encoding[] { null, Encoding.Default, Encoding.UTF8, Encoding.Unicode, Encoding.UTF32, Encoding.BigEndianUnicode };

        /// <summary>
        /// 使用指定的上下文返回此对象是否支持可以从列表中选取的标准值集。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <returns>
        /// 如果应调用 <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> 来查找对象支持的一组公共值，则为 true；否则，为 false。
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        /// <summary>
        /// 使用指定的上下文返回从 <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> 返回的标准值的集合是否为可能值的独占列表。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <returns>
        /// 如果从 <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> 返回的 <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> 是可能值的穷举列表，则为 true；如果还可能有其他值，则为 false。
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return false;
        }

        /// <summary>
        /// 当与格式上下文一起提供时，返回此类型转换器设计用于的数据类型的标准值集合。
        /// </summary>
        /// <param name="context">提供格式上下文的 <see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，可用来提取有关从中调用此转换器的环境的附加信息。此参数或其属性 (Property) 可以为 null。</param>
        /// <returns>
        /// 包含标准有效值集的 <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/>；如果数据类型不支持标准值集，则为 null。
        /// </returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection
        GetStandardValues(ITypeDescriptorContext context) {

            return new System.ComponentModel.TypeConverter.StandardValuesCollection(_knownEncodings);
        }

        /// <summary>
        /// 获取一个值，该值指示此转换器是否可以使用指定的上下文将给定源类型中的对象转换为字符串。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <param name="sourceType"><see cref="T:System.Type"/>，表示要从中进行转换的类型。</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return true;
        }

        /// <summary>
        /// 将指定的值对象转换为 <see cref="T:System.String"/> 对象。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <param name="culture">要使用的 <see cref="T:System.Globalization.CultureInfo"/>。</param>
        /// <param name="value">要转换的 <see cref="T:System.Object"/>。</param>
        /// <returns>
        /// 表示转换的 value 的 <see cref="T:System.Object"/>。
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">转换未能执行。</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
            if(value == null) {
                return null;
            }
            if(value is string) {

                string v = (string)value;

                if(v.Length == 0 || v == "自动") {
                    return null;
                }

                foreach(Encoding e in _knownEncodings) {
                    if(e != null && e.EncodingName == v)
                        return e;
                }


                try {
                    return Encoding.GetEncoding(v);
                } catch {

                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <param name="culture"><see cref="T:System.Globalization.CultureInfo"/>。如果传递 null，则采用当前区域性。</param>
        /// <param name="value">要转换的 <see cref="T:System.Object"/>。</param>
        /// <param name="destinationType"><paramref name="value"/> 参数要转换成的 <see cref="T:System.Type"/>。</param>
        /// <returns>
        /// 表示转换的 value 的 <see cref="T:System.Object"/>。
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="destinationType"/> 参数为 null。</exception>
        /// <exception cref="T:System.NotSupportedException">不能执行转换。</exception>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
            if(value != null) {
                return ((Encoding)value).EncodingName;
            } else {
                return "自动";
            }
        }

    }

}
