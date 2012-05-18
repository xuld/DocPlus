using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示文档执行时的一个作用域。
    /// </summary>
    sealed class Scope {

        /// <summary>
        /// 当前作用域的注释。
        /// </summary>
        public DocComment Comment;

        /// <summary>
        /// 当前作用域的父作用域。
        /// </summary>
        public Scope Parent;

        /// <summary>
        /// 当前作用域下成员的名字空间所属。
        /// </summary>
        public string CurrentMemberOf;

        /// <summary>
        /// 当前作用域下成员的名字空间所属是类。
        /// </summary>
        public bool CurrentMemberOfIsClass;

    }
}
