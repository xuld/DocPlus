using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocPlus.Core;

namespace DocPlus.GUI {

    /// <summary>
    /// 默认的进度报告器。
    /// </summary>
    public class ProgressReporter : IProgressReporter {

        TraceWindowControl _ctrl;

        /// <summary>
        /// 初始化 <see cref="DocPlus.GUI.ProgressReporter"/> 类的新实例。
        /// </summary>
        /// <param name="ctrl">The CTRL.</param>
        public ProgressReporter(TraceWindowControl ctrl) {
            _ctrl = ctrl;
        }

        /// <summary>
        /// 输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        public void Write(string message) {
            _ctrl.Trace(message);
        }

        /// <summary>
        /// 输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        /// <param name="args">用于格式化字符串的参数。</param>
        public void Write(string message, params object[] args) {
            _ctrl.Trace(String.Format(message, args));
        }

        /// <summary>
        /// 输出一个错误信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        public void Error(string source, int line, string message, int errNo) {
            Write("[错误] {0} {1}#{2}", message, source, line);
        }

        /// <summary>
        /// 输出一个警告信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        public void Warning(string source, int line, string message, int errNo) {
            Write("[警告] {0} {1}#{2}", message, source, line);
        }

        /// <summary>
        /// 输出一个信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        public void Info(string source, int line, string message, int errNo) {
            Write("[消息] {0} {1}#{2}", message, source, line);
        }

        /// <summary>
        /// 调试输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        public void Trace(string message) {
            Write("[调试] {0}", message);
        }

        /// <summary>
        /// 调试输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        /// <param name="args">用于格式化字符串的参数。</param>
        public void Trace(string message, params object[] args) {
            Write("[调试] " + message, args);
        }

        /// <summary>
        /// 报告工作进度。
        /// </summary>
        /// <param name="value">进度值，0 - 100 。</param>
        public void UpdateProgress(int value) {

        }

        /// <summary>
        /// 报告生成已开始。
        /// </summary>
        public void Start() {
            _ctrl.Clear();
        }

        /// <summary>
        /// 报告生成已完成。
        /// </summary>
        public void Complete() {

        }

    }

}
