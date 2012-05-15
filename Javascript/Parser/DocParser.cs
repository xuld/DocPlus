﻿using System;
using System.Collections.Generic;
using System.IO;
using CorePlus.Parser.Javascript;
using System.Collections.Specialized;

namespace DocPlus.Javascript {

    /// <summary>
    /// 解析 Javascript 文档的工具。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 本类包含了文档生成的核心算法。
    /// </para>
    /// 
    /// <para>
    /// 输入是一个 <see cref="DocProject"/>，输出是 <see cref="ApiDoc"/>。
    /// </para>
    /// 
    /// <para>
    /// 文档生成有以下步骤:
    /// </para>
    /// 
    /// <list type="number">
    ///     <item>
    ///         <listheader>使用 <see cref="CorePlus.Parser.Javascript.Scanner"/> 进行词法解析。</listheader>
    ///         词法解析用于提取单词，供语法解析使用。同时也进行注释解析。
    ///     </item>
    ///     <item>
    ///         <listheader>使用 <see cref="CorePlus.Parser.Javascript.Parser"/> 进行语法解析。</listheader>
    ///         语法解析用于构建语法树。
    ///     </item>
    ///     <item>
    ///         <listheader>使用 <see cref="DocPlus.DocParser.Javascript.JavaCommentParser"/> 进行注释解析。</listheader>
    ///         注释解析用于构建注释队列。   
    ///     </item>
    ///     <item>
    ///         <listheader>使用 <see cref="DocPlus.DocParser.Javascript.DocAstVistor"/> 进行文档解析。</listheader>
    ///         根据语法解析返回的语法树和注释分析返回的注释队列生成 Javascript 对象树。
    ///     </item>
    ///     <item>
    ///         <listheader>使用 <see cref="DocPlus.Javascript.DocMerger"/> 合成为文档。</listheader>
    ///         将 Javascript 对象树合成为文档。
    ///     </item>
    /// </list>
    /// 
    /// <para>
    /// 通过属性 <see cref="JavaScriptDocParser.JavaDocEntries"/> 可查看中间产生的全部注释。
    /// </para>
    /// 
    /// <para>
    /// 通过属性 <see cref="JavaScriptDocParser.Global"/> 可查看中间产生的全部变量。
    /// </para>
    /// 
    /// <para>
    /// 通过属性 <see cref="JavaScriptDocParser.Scanner"/> 可查看使用的扫描器。
    /// </para>
    /// 
    /// <para>
    /// 通过属性 <see cref="JavaScriptDocParser.Parser"/> 可查看使用的解释器。
    /// </para>
    /// 
    /// <example>
    /// 以下代码演示了如何解析一个 Javascript 源码。并且将解析后的注释保存在一个 script.api 文件。
    /// <code>
    /// DocProject project = new DocProject();
    /// project.AddFile("script1.js");
    /// 
    /// DocParser docParser = new DocParser(project);
    /// ApiDoc doc = docParser.Parse();
    /// 
    /// doc.Save("script.api");
    /// </code>
    /// </example>
    /// 
    /// <para>
    /// 本类实现了 1 个接口。
    /// </para>
    /// 
    /// <para>
    /// <see cref="CorePlus.Parser.Javascript.ICommentParser"/> 是用于 <see cref="CorePlus.Parser.Javascript.ScannerBase"/> 使用的接口实现， 用于 <see cref="CorePlus.Parser.Javascript.ScannerBase"/> 在扫描至文档解析时，可以通知当前对象解析注释。
    /// </para>
    /// 
    /// <para>类内部含一个 <see cref="CorePlus.Parser.Javascript.IErrorReporter"/> 的实现。用于将解析器错误转发到 <see cref="JavaScriptDocParser.ProgressReporter"/> 。</para>
    /// 
    /// </remarks>
    public class DocParser :ICommentParser {

        #region 字段

        /// <summary>
        /// 正在处理的项目配置。
        /// </summary>
        DocProject _project;

        /// <summary>
        /// 当前全部注释节点。
        /// </summary>
        Queue<Variant> _comments = new Queue<Variant>();

        /// <summary>
        /// 解析的源文件缓存。
        /// </summary>
        NameValueCollection _files = new NameValueCollection();

        /// <summary>
        /// 解析Javascript语法的解释器。
        /// </summary>
        Parser _parser;

        /// <summary>
        /// 当前使用的文档注释解释器。
        /// </summary>
        JavaCommentParser _docCommentParser;

        /// <summary>
        /// 当前使用的文档遍历工具。
        /// </summary>
        DocAstVistor _docAstVistor;

        /// <summary>
        /// 当前文档的生成工具。
        /// </summary>
        DocMerger _docMerger;

        /// <summary>
        /// 目前正在解析的源文件地址。
        /// </summary>
        string _currentSource;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前解析的变量树。
        /// </summary>
        public Variant Global {
            get {
                return _docAstVistor.Global;
            }
        }

        #endregion

        #region 方法

        void ApplyIgnores() {
            //var options = _docCommentParser.GlobalConfigs;
            //var g = _docAstVistor.Global;

            //foreach(string s in options.Ignores) {
            //    g.GetOrCreateNamespace(s).Ignore = true;
            //}
        }

        /// <summary>
        /// 初始化 <see cref="DocPlus.Javascript.DocParser"/> 类的新实例。
        /// </summary>
        /// <param name="project">一个 <see cref="DocProject"/> 对象，包含用于解析的全部配置。</param>
        public DocParser(DocProject project) {
            _project = project;
            _parser = new Parser(this, new ErrorReporter(_project));
            _parser.IsStrictMode = project.UseStrictMode;
            _docCommentParser = new JavaCommentParser(_project);
            _docAstVistor = new DocAstVistor(_project);
            _docMerger = new DocMerger(_project);
        }

        /// <summary>
        /// 解析一个文件。
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        void ParseScript(Script script) {

            // 如果不允许语法错误，则解析停止。
            if(_parser.ErrorCount > 0 && !_project.SkipSyntaxError) {
                throw new System.Exception("因为有语法错误，所以文档解析已停止");
            }

            VariantMap map = new VariantMap(_comments.ToArray());

            // 进行文档解析。
            _docAstVistor.Parse(script, map);
        }

        /// <summary>
        /// 解析一个文件。
        /// </summary>
        /// <param name="file">解析的文件名称。</param>
        /// <param name="path">解析的文件绝对位置。</param>
        public void ParseFile(string file, string path) {

            if(_files[file] == null) {
                _files[file] = path;
            } else {
                int i = 0;
                while(_files[file + "_" + (++i)] != null) ;

                file += i;
                _files[file] = path;
            }

            _currentSource = path;

            _project.ProgressReporter.Write(">> {0}", file);

            _comments.Clear();

            // 解析语法树。
            Script script = _parser.ParseFile(path, _project.Encoding);

            ParseScript(script);


        }

        /// <summary>
        /// 解析字符串中的代码。
        /// </summary>
        /// <param name="sourceCode">解析的代码。</param>
        public void ParseString(string sourceCode) {
            _currentSource = String.Empty;

            _comments.Clear();

            // 解析语法树。
            Script script = _parser.ParseString(sourceCode);


            ParseScript(script);
        }

        /// <summary>
        /// 编译整个项目。
        /// </summary>
        /// <returns></returns>
        public void Build() {

            for(int i = 0; i < _project.Items.Count; i++) {
                string name = _project.Items[i];
                if(Directory.Exists(name)) {
                    foreach(string s in Directory.GetFiles(name, "*.js", SearchOption.AllDirectories)) {
                        ParseFile(s.Substring(name.Length), s);
                    }
                } else {
                    ParseFile(Path.GetFileName(name), name);
                }
            }

            _docMerger.Parse(_docAstVistor.Global, _files);

        }

        #endregion

        #region DocPlus.Parser.Javascript.IDocParser 成员

        class ErrorReporter :IErrorReporter {

            DocProject _owner;

            public ErrorReporter(DocProject owner) {
                _owner = owner;
            }

            #region IErrorReporter 成员

            public void Error(string message, Location startLocation, Location endLocation) {
                _owner.ProgressReporter.Error(_owner.CurrentFile, startLocation.Line, message, ErrorNumber.SyntaxError);
            }

            public void Warning(string message, Location startLocation, Location endLocation) {
                _owner.ProgressReporter.Warning(_owner.CurrentFile, startLocation.Line, message, ErrorNumber.SyntaxWarning);
            }

            public void WarningStrict(string message, Location startLocation, Location endLocation) {
                _owner.ProgressReporter.Warning(_owner.CurrentFile, startLocation.Line, message, ErrorNumber.SyntaxWarning);
            }

            public void Clear() {
                ErrorCount = WarningCount = 0;
            }

            public int WarningCount {
                get;
                set;
            }

            public int ErrorCount {
                get;
                set;
            }

            #endregion
        }

        bool ICommentParser.CanParseSingleLineCommtent {
            get {
                return false;
            }
        }

        bool ICommentParser.CanParseMultiLineCommtent {
            get {
                return true;
            }
        }

        void ICommentParser.ParseSingleLineCommtent(ScannerBase scanner, TokenInfo token) {
            throw new NotSupportedException();
        }

        void ICommentParser.ParseMultiLineCommtent(ScannerBase scanner, TokenInfo token) {

            if (token.LiteralBuffer.Length > 0 && token.LiteralBuffer[0] == '*') {
                Variant v = _docCommentParser.Parse(token);
                v.Source = _currentSource;
                _comments.Enqueue(v);
            }
        }

        #endregion


    }
}
