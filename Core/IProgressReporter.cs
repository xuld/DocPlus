
namespace DocPlus.Core {

    /// <summary>
    /// 用于在项目生成时输出进度。
    /// </summary>
    public interface IProgressReporter {

        /// <summary>
        /// 输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        void Write(string message);

        /// <summary>
        /// 输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        /// <param name="args">用于格式化字符串的参数。</param>
        void Write(string message, params object[] args);

        /// <summary>
        /// 调试输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        void Trace(string message);

        /// <summary>
        /// 调试输出一个日志信息。
        /// </summary>
        /// <param name="message">要输出的日志字符串。</param>
        /// <param name="args">用于格式化字符串的参数。</param>
        void Trace(string message, params object[] args);

        /// <summary>
        /// 输出一个错误信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="column">列号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        void Error(string source, int line, string message, int errNo);

        /// <summary>
        /// 输出一个警告信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="column">列号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        void Warning(string source, int line, string message, int errNo);

        /// <summary>
        /// 输出一个信息。
        /// </summary>
        /// <param name="source">错误源文件。</param>
        /// <param name="line">行号。</param>
        /// <param name="column">列号。</param>
        /// <param name="message">说明。</param>
        /// <param name="errNo">错误号。</param>
        void Info(string source, int line, string message, int errNo);

        /// <summary>
        /// 报告工作进度。
        /// </summary>
        /// <param name="value">进度值，0 - 100 。</param>
        void UpdateProgress(int value);

        /// <summary>
        /// 报告生成已开始。
        /// </summary>
        void Start();

        /// <summary>
        /// 报告生成已完成。
        /// </summary>
        void Complete();

    }

}
