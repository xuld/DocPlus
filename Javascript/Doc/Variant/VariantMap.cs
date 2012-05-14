using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlus.Parser.Javascript;

namespace DocPlus.Javascript {

    /// <summary>
    /// 存储注释在源文件的位置。
    /// </summary>
    public sealed class VariantMap {

        Variant[] _cache;

        int _index;

        public VariantMap(Queue<Variant> comments) {

          _cache = comments.ToArray();

            //_cache = new Variant[comments.Count];

            //Variant c;

            //int i = 0;
            //while (comments.Count > 0) {
            //    c = comments.Dequeue();
            //    // Console.WriteLine(c.ToString());

            //    _cache[i] = new Variant(c);

            //    _cache[i].StartLocation = c.StartLocation;
            //    _cache[i].EndLocation = c.EndLocation;
            //    _cache[i].DisplayName = c.DisplayName;

            //    if (c.Member == CommentNode.Namespace || c.Member == CommentNode.Class) {
            //        _cache[i].NamespaceSetter = c.DisplayName;
            //        if(c.Member == CommentNode.Class)
            //        _cache[i].MemberType = MemberType.Class;
            //    }
            //    i++;

            //}
        }

        /// <summary>
        /// 获取某行以前没有处理的变量。
        /// </summary>
        /// <param name="location"></param>
        /// <returns>返回符合要求的变量。</returns>
        public Variant DequeeBefore(Location location) {

            if (_index >= _cache.Length)
                return null;

            Variant v = _cache[_index];

            // 超过行号。
            if (v.EndLocation > location) {
                return null;
            }

            // 找到了一个之前的变量。
            _index++;
            return v;
        }

        /// <summary>
        /// 获取某行以前没有处理的变量。
        /// </summary>
        /// <param name="location"></param>
        /// <returns>返回符合要求的变量。</returns>
        public Variant DequeeAll() {

            if (_index >= _cache.Length)
                return null;

            return _cache[_index++];
        }

        /// <summary>
        /// 获取用于某行的变量。
        /// </summary>
        /// <param name="location"></param>
        /// <returns>返回符合要求的变量。</returns>
        public Variant FindFor(Location location) {

            if (_index > _cache.Length)
                return null;

            Variant v = _cache[_index];

            //  需要往前找
            if (v.EndLocation > location) {
                for (int i = _index; i >= 0; i--) {
                    int off = _cache[i].EndLocation.Line - location.Line;
                    if (off <= 0) {
                        if (off == 0 || off == 1) {
                            return _cache[i];
                        }

                        return null;
                    }
                }
            } else {
                for (int i = _index; i < _cache.Length; i++) {
                    int off = _cache[i].EndLocation.Line - location.Line;
                    if (off <= 0) {
                        if (off == 0 || off == 1) {
                            return _cache[i];
                        }

                        return null;
                    }
                }
            }

            return null;
        }

        ///// <summary>
        ///// 获取一个变量，如果此变量之后还有可用的变量，返回 true。
        ///// </summary>
        ///// <param name="location">获取指定行之前的变量。</param>
        ///// <param name="v">返回符合要求的变量。</param>
        ///// <returns>如果此变量之后还有可用的变量，返回 true。</returns>
        //public bool PeekNext(Location location) {

        //    if (_index < _cache.Length) {
        //        int off = _cache[_index].EndLocation.Line - location.Line;

        //        return off == 0 || off == -1;

        //    }

        //    return false;
        //}

        /// <summary>
        /// 获取一个变量，如果此变量之后还有可用的变量，返回 true。
        /// </summary>
        /// <param name="location">获取指定行之前的变量。</param>
        /// <param name="v">返回符合要求的变量。</param>
        /// <returns>如果此变量之后还有可用的变量，返回 true。</returns>
        public bool GetNext(Location location, out Variant v) {

            int off;

            // 如果当前索引在范围内。
            if (_index < _cache.Length) {

                // 获取下一个变量。
                v = _cache[_index];
                off = v.EndLocation.Line - location.Line;

                // 如果下一个变量在当前行之上。
                if (off <= 0) {
                    _index++;

                    bool r = off < -1;

                    // 假如当前的注释是接近的注释，那检查是否存在更接近的注释。
                    if (!r && _index < _cache.Length) {
                        off = _cache[_index].EndLocation.Line - location.Line;
                        if(off == 0 || off == -1)
                            r = true;
                    }

                    return r;
                }

            }

            // 否则，没有可用的变量了。必须回去找。
            for (int i = _index - 1; i >= 0; i--) {
                off = _cache[i].EndLocation.Line - location.Line;
                if (off <= 0) {
                    if (off == 0 || off == -1) {
                        v = _cache[i];
                        return false;
                    }

                    break;
                }
            }


            v = null;
            return false;
        }

    }
}
