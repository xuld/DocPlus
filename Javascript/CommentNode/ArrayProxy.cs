using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DocPlus.Javascript {

    /// <summary>
    /// 模拟可变长的数组。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ArrayProxy<T> :IEnumerable {

        T[] _value;

        public void Add(T value) {
            if (_value == null) {
                _value = new T[1] { value };
                return;

            }

            T[] r = new T[_value.Length + 1];
            _value.CopyTo(r, 0);
            r[_value.Length] = value;

            _value = r;
        }

        public T this[int index] {
            get {
                return _value[index];
            }
        }

        public int Count {
            get {
                return _value == null ? 0 : _value.Length;
            }
        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (T p in _value) {
                sb.AppendLine(p.ToString());
            }
            return sb.ToString();
        }

        #region IEnumerable 成员

        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator"/> 对象。
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _value.GetEnumerator();
        }

        #endregion
    }
}
