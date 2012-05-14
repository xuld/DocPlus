using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {
    class DocOpList {

        List<DocOp> _list;

        public void Emit(DocOpType type) {

        }

        public void Emit(DocOpType type, Variant comment) {

        }

    }

    /// <summary>
    /// 表示文档操作类型。
    /// </summary>
    enum DocOpType {

        ///// <summary>
        ///// 表示创建一个名字空间。
        ///// </summary>
        //Namespace,

        /// <summary>
        /// 表示创建一个成员。
        /// </summary>
        Member,

        /// <summary>
        /// 表示进入一个函数作用域。
        /// </summary>
        PushFunctionScope,

        /// <summary>
        /// 表示退出一个函数作用域。
        /// </summary>
        PopFunctionScope,

        /// <summary>
        /// 表示进入上一个成员下的作用域。
        /// </summary>
        PushScope,

        /// <summary>
        /// 表示退出上一个成员下的作用域。
        /// </summary>
        PopScope

    }

    sealed class DocOp {

        public DocOpType Type;

        public Variant Comment;

    }
}
