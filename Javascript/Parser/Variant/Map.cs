using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {

    /// <summary>
    /// 一个找不到指定键值就返回默认值的集合。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class Map<T, K> : CorePlus.Collections.Dictionary<T, K> {

        protected override K OnKeyNotFound(T key) {
            return default(K);
        }

    }
}
