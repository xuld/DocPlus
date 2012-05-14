using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Core {
    /// <summary>
    /// 表示一个。
    /// </summary>
    public class ConsoleProgressReporter :IProgressReporter {

        static readonly ConsoleProgressReporter _instance = new ConsoleProgressReporter();

        /// <summary>
        /// 获取当前控制器实例 。
        /// </summary>
        /// <value></value>
        public static ConsoleProgressReporter Instance {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// 输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        public void Write(string message) {
            Console.WriteLine(message);
        }

        /// <summary>
        /// 输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        /// <param name="args">用于格式化字符串的参数。</param>
        public void Write(string message, params object[] args) {
            Console.WriteLine(message, args);
        }

        /// <summary>
        /// 输出一个错误信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        public void Error(string source, int line, string message, int errNo) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// 输出一个警告信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        public void Warning(string source, int line, string message, int errNo) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// 输出一个信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        public void Info(string source, int line, string message, int errNo) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// 调试输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        public void Trace(string message) {
            Console.Error.WriteLine(message);
        }

        /// <summary>
        /// 调试输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        /// <param name="args">用于格式化字符串的参数。</param>
        public void Trace(string message, params object[] args) {
            Console.Error.WriteLine(message, args);
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
            Console.Clear();
        }

        /// <summary>
        /// 报告生成已完成。
        /// </summary>
        public void Complete() {
            
        }

    }

}
