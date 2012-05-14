using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示变量执行的作用域。
    /// </summary>
    public sealed class Scope :Variant {

        /// <summary>
        /// 获取当前成员的父成员。
        /// </summary>
        public Scope Parent {
            get;
            internal set;
        }

        /// <summary>
        /// 获取或设置用于存放默认子变量的父变量。此变量根据页面内的 namespace setter 决定。
        /// </summary>
        public Variant LastParent {
            get;
            set;
        }

        /// <summary>
        /// 当前用于存放子变量的父变量。此变量根据页面内的 namespace setter 决定。
        /// </summary>
        public Variant CurrentParent {
            get;
            set;
        }

        static readonly CorePlus.Parser.Javascript.Node _emptyStatement = new CorePlus.Parser.Javascript.EmptyStatement(CorePlus.Parser.Javascript.Location.Empty);

        /// <summary>
        /// 当前正在执行的语句。
        /// </summary>
        public CorePlus.Parser.Javascript.Node CurrentStatement {
            get;
            set;
        }

        public Scope() {
            CurrentStatement = _emptyStatement;
            Attribute = VariantAttribute.Scope;
        }

        public void SetCurrentParent(Variant v) {
            CurrentParent = v;
            LastParent = null;
        }

        public void StoreCurrentParent(Variant v) {
            LastParent = CurrentParent;
            CurrentParent = v;
        }

        public void RestoreCurrentParent() {
            CurrentParent = LastParent;
            LastParent = null;
        }

        public void CopyParent(Scope scope) {
            scope.CurrentParent = CurrentParent;
            scope.LastParent = LastParent;
        }


        public void UpdateParent(Variant src, Variant dest) {
            if (CurrentParent == src) {
                CurrentParent = dest;
            } else if (LastParent == src) {
                LastParent = dest;
            }
        }

        public void ClearCurrentParent() {
            CurrentParent = null;
            LastParent = null;
        }
    }
}
