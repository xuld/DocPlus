using System;
using CorePlus.Collections;
using System.Text;
using CorePlus.Parser.Javascript;
using CorePlus.Core;
using System.Diagnostics;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个文档注释解析后的内容。
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public sealed class Variant :Dictionary<CommentNode, string[]> {

        protected override string[] OnKeyNotFound(CommentNode key) {
            return null;
        }
    }
}
