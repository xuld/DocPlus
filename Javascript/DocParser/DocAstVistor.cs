using System;
using System.Collections.Generic;
using System.Text;

using CorePlus.Parser.Javascript;
using CorePlus.Core;
using System.Diagnostics;

namespace DocPlus.Javascript {

    /// <summary>
    /// 遍历语法树生成文档的工具。通过遍历语法树自动填充注释。
    /// </summary>
    /// <remarks>
    /// 全部文档生成的算法都在此类。
    /// <para>
    /// 层次关系算法。
    /// </para>
    /// <para>
    /// 一个变量在定义时这样来决定它的上级。
    /// </para>
    /// <list type="ol">
    /// <item>如果是赋值操作，上级变量为赋值的目标变量。</item>
    /// <item>查找所在语句的注释，上级变量为改语句所定义的名字空间。</item>
    /// <item>上一个定义的名字空间。</item>
    /// <item>最后返回空，忽略成员。</item>
    /// </list>
    /// </remarks>
    public class DocAstVistor :IAstVisitor {

        #region 遍历器必备

        /// <summary>
        /// 正在处理的项目配置。
        /// </summary>
        DocProject _project;

        /// <summary>
        /// 当前变量列表。
        /// </summary>
        DocComment[] _map;

        /// <summary>
        /// 当前变量列表的位置。
        /// </summary>
        int _position;

        bool GetNext(Location location, out DocComment dc) {

            int off;

            // 如果当前索引在范围内。
            if (_position < _map.Length) {

                // 获取下一个变量。
                dc = _map[_position];
                off = dc.EndLocation.Line - location.Line;

                // 如果下一个变量在当前行之上。
                if (off <= 0) {
                    _position++;

                    bool r = off < -1;

                    // 假如当前的注释是接近的注释，那检查是否存在更接近的注释。
                    if (!r && _position < _map.Length) {
                        off = _map[_position].EndLocation.Line - location.Line;
                        if (off == 0 || off == -1)
                            r = true;
                    }

                    return r;
                }

            }

            // 否则，没有可用的变量了。必须回去找。
            //for (int i = _position - 1; i >= 0; i--) {
            //    off = _map[i].EndLocation.Line - location.Line;
            //    if (off <= 0) {
            //        if (off == 0 || off == -1) {
            //            dc = _map[i];
            //            return false;
            //        }

            //        break;
            //    }
            //}


            dc = null;
            return false;
        }

        /// <summary>
        /// 获取指定的文档节点附近定义的注释变量。并处理之前的全部注释变量。如果不存在变量，则返回 null 。
        /// </summary>
        /// <param name="node">节点。</param>
        /// <returns>变量。</returns>
        DocComment GetDocCommentBy(Node node) {

            DocComment dc;

            // 获取一个变量，如果此变量之后还有可用的变量，直接处理。
            while (GetNext(node.StartLocation, out dc)) {

                // 处理不属于节点，但之前的变量。
                Process(dc);
            }

            return dc;
        }

        void ProcessCommentBefore(Node node) {

            DocComment dc;
            while (GetNext(node.EndLocation, out dc)) {

                // 处理不属于节点，但之前的变量。
                Process(dc);
            }
        }

        /// <summary>
        /// 初始化 <see cref="DocPlus.DocParser.Javascript.DocAstVistor"/> 的新实例。
        /// </summary>
        public DocAstVistor(DocProject project) {
            _project = project;
        }

        /// <summary>
        /// 开始对指定的节点解析。
        /// </summary>
        /// <param name="script">语法树。</param>
        /// <param name="comments">所有注释。</param>
        /// <returns>全局对象。所有变量都可从这个节点找到。</returns>
        public void Parse(Script script, DocComment[] comments) {
            _map = comments;
            CurrentScope = new Scope() { CurrentMemberOf = "window" };
            VisitScript(script);
        }

        #endregion

        #region 运行

        /// <summary>
        /// 获取或设置上一步执行代码返回的变量值。
        /// </summary>
        object ReturnValue {
            get;
            set;
        }

        /// <summary>
        /// 获取当前执行的作用域。
        /// </summary>
        Scope CurrentScope {
            get;
            set;
        }

        Stack<DocComment> _objectStack = new Stack<DocComment>();

        bool IsGlobal {
            get {
                return CurrentScope.Parent == null;
            }
        }

        DocComment CurrentFunction {
            get {
                return CurrentScope.Comment;
            }
        }

        void PushScope(DocComment docComment) {
            Scope scope = new Scope();
            scope.Comment = docComment;
            scope.Parent = CurrentScope;
            scope.CurrentMemberOf = CurrentScope.CurrentMemberOf;
            scope.CurrentMemberOfIsClass = CurrentScope.CurrentMemberOfIsClass;
            CurrentScope = scope;
        }

        void PopScope() {

            // 如果启用就近原则匹配，则内作用域的名字空间可以覆盖父作用域。
            if (_project.EnableGlobalNamespaceSetter) {
                CurrentScope.Parent.CurrentMemberOf = CurrentScope.CurrentMemberOf;
                CurrentScope.Parent.CurrentMemberOfIsClass = CurrentScope.CurrentMemberOfIsClass;
            }

            CurrentScope = CurrentScope.Parent;
        }

        DocComment CurrentObject {
            get {
                return _objectStack.Count > 0 ? _objectStack.Peek() : null;
            }
        }

        void PushObject(DocComment docComment) {
            _objectStack.Push(docComment);
        }

        void PopObject() {
            _objectStack.Pop();
        }

        private static bool ConvertToBoolean(object value) {
            if (value == null) {
                return false;
            }

            if (value is bool)
                return (bool)value;

            if (value is double)
                return (double)value > 0;

            if (value is string)
                return ((string)value).Length > 0;

            return true;
        }

        private static double ConvertToNumber(object value) {
            if (value == null) {
                return double.NaN;
            }

            if (value is double)
                return (double)value;

            if (value is bool)
                return (bool)value ? 1d : 0d;

            string strValue = value.ToString();
            double dblValue;
            if (double.TryParse(strValue, out dblValue))
                return dblValue;

            if (strValue == "null")
                return 0d;

            return double.NaN;
        }

        private static bool IsNumberic(object value) {
            return value is double || value is bool;
        }

        /// <summary>
        /// 返回当前变量转为数字类型的值。
        /// </summary>
        /// <returns></returns>
        private static int ConvertToInteger(object value) {
            double d = ConvertToNumber(value);
            return double.IsNaN(d) ? 0 : unchecked((int)d);
        }

        #endregion

        #region 注释分析

        /// <summary>
        /// 根据注释所处位置，自动分析注释的所属对象。
        /// </summary>
        /// <param name="dc"></param>
        private void AutoFillMemberOf(DocComment dc) {
            string memberType = dc.MemberType;
            int memberTypeNo =
                memberType == "class" ? 0 :
                memberType == "enum" || memberType == "interface" || memberType == "module" ? 1 : 2;

            // 处理当前的所属成员问题。
            if (dc.NamespaceSetter == null) {
                if (memberTypeNo <= 1) {
                    if (dc.Name == null) {
                        CurrentScope.CurrentMemberOf = null;
                        CurrentScope.CurrentMemberOfIsClass = false;
                    } else if (memberTypeNo == 0) {
                        CurrentScope.CurrentMemberOf = dc.FullName;
                        CurrentScope.CurrentMemberOfIsClass = !dc.IsStatic;
                    } else if (memberTypeNo == 1) {
                        CurrentScope.CurrentMemberOf = dc.FullName;
                        CurrentScope.CurrentMemberOfIsClass = false;
                    }
                }
            } else {
                if (dc.NamespaceSetter.Length == 0) {
                    CurrentScope.CurrentMemberOf = dc.FullName;
                } else {
                    CurrentScope.CurrentMemberOf = dc.NamespaceSetter;
                }
                CurrentScope.CurrentMemberOfIsClass = false;
            }

            // 处理类成员问题。
            if (memberTypeNo == 2) {

                // 如果没有指定 memberOf ，程序需要自行猜测。
                if (dc.MemberOf == null) {
                    string parentNamspace;
                    bool parentIsClass;

                    // 如果现在正在 obj = {} 状态下。
                    DocComment obj = CurrentObject;

                    if (obj != null) {
                        parentNamspace = obj.FullName;
                        parentIsClass = obj.MemberType == "class";

                        if (obj.Ignore) {
                            dc.Ignore = true;
                        }
                    } else {
                        parentNamspace = CurrentScope.CurrentMemberOf;
                        parentIsClass = CurrentScope.CurrentMemberOfIsClass;
                    }

                    if (parentIsClass) {
                        if (dc.IsStatic) {
                            dc.MemberOf = parentNamspace;
                        } else {
                            dc.MemberOf = parentNamspace + ".prototype";
                        }
                    } else {
                        dc.MemberOf = parentNamspace;
                    }
                }

            }
        }

        private void Process(DocComment dc) {
            AutoFillMemberOf(dc);
        }

        private void Assign(Node node, string name, Expression value, bool isVar = false) {
            DocComment dc = GetDocCommentBy(node);
            if (dc != null) {
                if (isVar && !IsGlobal && _project.EnableClosure) {
                    dc.Ignore = true;
                }
                AutoFillName(dc, name);
                AutoFillMemberOf(dc);
                PushObject(_project.ResolveObjectSetter && value is ObjectLiteral ? dc : null);
                VisitExpression(value);
                PopObject();
                AutoFillTypeByReturnValue(dc);
            }
        }

        /// <summary>
        /// 自动识别注释中的名字。
        /// </summary>
        /// <param name="dc">赋值的父对象。</param>
        /// <param name="value">变量。</param>
        private void AutoFillName(DocComment dc, string name) {

            if (dc != null && name != null)
                dc.AutoFill(NodeNames.Name, name);

        }

        private void AutoFillTypeByReturnValue(DocComment dc) {
            if (dc != null && ReturnValue != null) {
                if (ReturnValue is bool) {
                    dc.AutoFill(NodeNames.Name, "Boolean");
                    dc.AutoFill(NodeNames.Value, (bool)ReturnValue == true ? "true" : "false");
                } else if (ReturnValue is string) {
                    dc.AutoFill(NodeNames.Type, "String");
                    dc.AutoFill(NodeNames.Value, new StringLiteral((string)ReturnValue, '"', Location.Empty, Location.Empty).ToString());
                } else if (ReturnValue is double) {
                    dc.AutoFill(NodeNames.Type, "Number");
                    if (!(double.IsNaN((double)ReturnValue)))
                        dc.AutoFill(NodeNames.Value, ReturnValue.ToString());
                } else if (ReturnValue is ILiteral) {
                    if (ReturnValue is ArrayLiteral) {
                        dc.AutoFill(NodeNames.Type, "Array");
                    } else if (ReturnValue is NullLiteral) {
                        dc.AutoFill(NodeNames.Value, "null");
                    } else if (ReturnValue is RegExpLiteral) {
                        dc.AutoFill(NodeNames.Type, "RegExp");
                        dc.AutoFill(NodeNames.Value, ((RegExpLiteral)ReturnValue).ToString());
                    } else if (ReturnValue is FunctionExpression) {
                        dc.AutoFill(NodeNames.Type, "method");
                        FunctionExpression fn = (FunctionExpression)ReturnValue;

                        var param = (ParamInfoCollection)dc[NodeNames.Param];
                        if (param == null) {
                            dc[NodeNames.Param] = param = new ParamInfoCollection();
                            foreach (Identifier p in fn.Params) {
                                param.Add(p.Value);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region IAstVisitor 成员

        [DebuggerStepThrough()]
        public void VisitScript(Script script) {

            // 访问语句。
            VisitBlock(script);

            // 处理全部剩下的注释。
            while (_position < _map.Length) {
                DocComment dc = _map[_position++];
                Process(dc);
            }
        }

        [DebuggerStepThrough()]
        public void VisitStatements(NodeList<Statement> statements) {
            foreach (Statement s in statements) {
                VisitStatement(s);
            }
        }

        public void VisitStatement(Statement statement) {
            // 遍历语句内容。
            statement.Accept(this);

            ReturnValue = null;
        }

        [DebuggerStepThrough()]
        public void VisitExpression(Expression expression) {
            expression.Accept(this);
        }

        public void VisitArrayLiteral(ArrayLiteral arrayLiteral) {

            foreach (Expression e in arrayLiteral.Values) {
                VisitExpression(e);
            }

            ReturnValue = null;
        }

        public void VisitAssignmentExpression(AssignmentExpression assignmentExpression) {

            DocComment dc = GetDocCommentBy(assignmentExpression);

            IndexCallExpression e = assignmentExpression.Target as IndexCallExpression;

            // obj[index]
            if (e != null) {

                // 获取父对象。
                VisitExpression(e.Target);

                // 获取值对象。
                VisitExpression(e.Argument);

            } else {

                PropertyCallExpression e2 = assignmentExpression.Target as PropertyCallExpression;

                if (e2 != null) {

                    // 获取父对象。
                    VisitExpression(e2.Target);

                    AutoFillName(dc, e2.Argument);

                } else {
                    Identifier e3 = assignmentExpression.Target as Identifier;

                    if (e3 != null) {

                        AutoFillName(dc, e3.Name);

                    }

                }

            }

            if (dc != null)
                AutoFillMemberOf(dc);
            PushObject(assignmentExpression.Value is ObjectLiteral ? dc : null);
            VisitExpression(assignmentExpression.Value);
            PopObject();

            AutoFillTypeByReturnValue(dc);
        }

        public void VisitBlock(Block blockStatement) {
            VisitStatements(blockStatement.Statements);

            // 获取一个变量，如果此变量之后还有可用的变量，直接处理。
            ProcessCommentBefore(blockStatement);
        }

        public void VisitBreakStatement(BreakStatement breakStatement) {
            // EMPTY
        }

        public void VisitCallNative(CallNative callNative) {
            // EMPTY
        }

        public void VisitCaseClause(CaseClause caseLabel) {

            if (caseLabel.Label != null) {
                VisitExpression(caseLabel.Label);
            }




            VisitStatements(caseLabel.Statements);
        }

        public void VisitConditionalExpression(ConditionalExpression conditionalExpression) {
            VisitExpression(conditionalExpression.Condition);
            VisitExpression(conditionalExpression.ThenExpression);
            object r = ReturnValue;
            VisitExpression(conditionalExpression.ElseExpression);

            ReturnValue = ReturnValue ?? r;
        }

        public void VisitContinueStatement(ContinueStatement continueStatement) {
            // EMPTY
        }

        public void VisitPostfixExpression(PostfixExpression countOperation) {
            VisitExpression(countOperation.Expression);
            object v = ReturnValue;
            if (v != null) {
                switch (countOperation.Operator) {
                    case TokenType.Inc:
                        ReturnValue = ConvertToNumber(v) + 1;
                        break;
                    case TokenType.Dec:
                        ReturnValue = ConvertToNumber(v) - 1;
                        break;
                }
            } else {
                ReturnValue = double.NaN;
            }
        }

        public void VisitConstStatement(ConstStatement constStatement) {
            VisitVariableStatement(constStatement);
        }

        public void VisitDebuggerStatement(DebuggerStatement debuggerStatement) {
            // EMPTY
        }

        public void VisitDoWhileStatement(DoWhileStatement doWhileStatement) {
            VisitExpression(doWhileStatement.Condition);
            VisitStatement(doWhileStatement.Body);
        }

        public void VisitEmptyStatement(EmptyStatement emptyStatement) {
            // EMPTY
        }

        public void VisitExpressionStatement(ExpressionStatement expressionStatement) {
            VisitExpression(expressionStatement.Expression);
        }

        public void VisitFalseLiteral(FalseLiteral falseLiteral) {
            ReturnValue = false;
        }

        public virtual void VisitForInStatement(ForInStatement forinStatement) {
            VisitStatement(forinStatement.Each);
            VisitExpression(forinStatement.Enumerable);
            VisitStatement(forinStatement.Body);
        }

        public virtual void VisitForStatement(ForStatement forStatement) {
            VisitStatement(forStatement.InitStatement);
            VisitExpression(forStatement.Condition);
            VisitExpression(forStatement.NextExpression);
            VisitStatement(forStatement.Body);
        }

        public void VisitFunctionExpression(FunctionExpression functionExpression) {

            DocComment dc = GetDocCommentBy(functionExpression);
            if (_project.EnableClosure && dc != null && functionExpression.Name != null && !IsGlobal) {
                dc.Ignore = true;
            }
            AutoFillName(dc, functionExpression.Name);

            if (dc != null)
                AutoFillMemberOf(dc);

            PushScope(dc);
            VisitStatements(functionExpression.Statements);
            ProcessCommentBefore(functionExpression);
            PopScope();
        }

        public void VisitFunctionCallExpression(FunctionCallExpression functionCallExpression) {

            VisitExpression(functionCallExpression.Target);

            ReturnValue = null;

            foreach (Expression s in functionCallExpression.Arguments) {
                VisitExpression(s);
            }
        }

        public void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration) {
            VisitFunctionExpression(functionDeclaration.Function);
        }

        public void VisitLabelledStatement(LabelledStatement labelledStatement) {
            VisitStatement(labelledStatement.Statement);
        }

        public void VisitNewExpression(NewExpression newExpression) {
            VisitExpression(newExpression.Expression);
        }

        public void VisitNumberLiteral(NumberLiteral numberLiteral) {
            ReturnValue = ConvertToNumber(numberLiteral.Value);
        }

        public void VisitNullLiteral(NullLiteral nullLiteral) {
            ReturnValue = nullLiteral;
        }

        public void VisitIdentifier(Identifier identifier) {
            ReturnValue = null;
        }

        public void VisitIndexCallExpression(IndexCallExpression indexCallExpression) {
            DocComment dc = GetDocCommentBy(indexCallExpression);
            VisitExpression(indexCallExpression.Target);
            if (ReturnValue is string) {
                AutoFillName(dc, (string)ReturnValue);

                if (dc != null)
                    AutoFillMemberOf(dc);
            }
            VisitExpression(indexCallExpression.Argument);
            ReturnValue = null;
        }

        public void VisitIfStatement(IfStatement ifStatement) {
            VisitExpression(ifStatement.Condition);
            VisitStatement(ifStatement.Then);
            VisitStatement(ifStatement.Else);
        }

        public void VisitIncrementOperation(IncrementOperation incrementOperation) {
            VisitExpression(incrementOperation.Expression);
        }

        public void VisitObjectLiteral(ObjectLiteral objectLiteral) {

            foreach (ObjectLiteral.Property p in objectLiteral.Value) {
                VisitProperty(p);
            }

            ProcessCommentBefore(objectLiteral);

            ReturnValue = null;
        }

        public void VisitParamedExpression(ParamedExpression paramedExpression) {
            VisitExpression(paramedExpression.Expression);
        }

        public void VisitProperty(ObjectLiteral.Property property) {
            Assign(property, property.Key.Value, property.Value);
        }

        public void VisitPropertyCallExpression(PropertyCallExpression propertyCallExpression) {
            //DocComment dc = GetDocCommentBy(propertyCallExpression);
            //if (dc != null) {
            //    AutoFillName(dc, propertyCallExpression.Argument);
            //}
            VisitExpression(propertyCallExpression.Target);
            ReturnValue = null;
        }

        public void VisitRegExpLiteral(RegExpLiteral regExpLiteral) {
            ReturnValue = regExpLiteral;
        }

        public void VisitReturnStatement(ReturnStatement returnStatement) {
            VisitExpression(returnStatement.Expression ?? new UndefinedExpression());

            DocComment currentFunction = CurrentFunction;

            if (currentFunction != null) {

                if (currentFunction[NodeNames.Return] == null) {
                    currentFunction[NodeNames.Return] = new TypeSummary();
                }

                currentFunction.Type = null;
                AutoFillTypeByReturnValue(currentFunction);

                ((TypeSummary)currentFunction[NodeNames.Return]).Type = currentFunction.Type;
                currentFunction.Type = "Function";
            }
        }

        public void VisitSemicolon(Semicolon semicolon) {
            //   EMPTY     
        }

        public void VisitStringLiteral(StringLiteral stringLiteral) {
            ReturnValue = stringLiteral.Value;
        }

        public void VisitSwitchStatement(SwitchStatement switchStatement) {
            VisitExpression(switchStatement.Condition);
            foreach (CaseClause c in switchStatement.Cases) {
                VisitCaseClause(c);
            }
        }

        public void VisitThisLiteral(ThisLiteral thisLiteral) {
            ReturnValue = null;
        }

        public void VisitThrowStatement(ThrowStatement throwStatement) {
            VisitExpression(throwStatement.Expression);

            if (CurrentFunction != null && ReturnValue is string) {

                ArrayProxy<TypeSummary> p = (ArrayProxy<TypeSummary>)CurrentFunction[NodeNames.Exception];

                if (p == null) {
                    CurrentFunction[NodeNames.Exception] = p = new ArrayProxy<TypeSummary>();
                }

                p.Add(new TypeSummary { Summary = (string)ReturnValue, Type = "String" });

            }
        }

        public void VisitTrueLiteral(TrueLiteral trueLiteral) {
            ReturnValue = true;
        }

        public virtual void VisitTryCatchFinallyStatement(TryCatchFinallyStatement tryCatchFinallyStatement) {
            VisitBlock(tryCatchFinallyStatement.TryBlock);
            VisitBlock(tryCatchFinallyStatement.CatchBlock);
            VisitBlock(tryCatchFinallyStatement.FinallyBlock);
        }

        public virtual void VisitTryCatchStatement(TryCatchStatement tryCatchStatement) {
            VisitBlock(tryCatchStatement.TryBlock);
            VisitBlock(tryCatchStatement.CatchBlock);
        }

        public virtual void VisitTryFinallyStatement(TryFinallyStatement tryFinallyStatement) {
            VisitBlock(tryFinallyStatement.TryBlock);
            VisitBlock(tryFinallyStatement.FinallyBlock);
        }

        public void VisitUndefinedExpression(UndefinedExpression undefinedExpression) {
            ReturnValue = null;
        }

        public void VisitUnaryExpression(UnaryExpression unaryExpression) {

            VisitExpression(unaryExpression.Expression);

            switch (unaryExpression.Operator) {
                case TokenType.Not:
                    ReturnValue = !ConvertToBoolean(ReturnValue);
                    break;

                case TokenType.Delete:
                case TokenType.Void:
                    ReturnValue = null;
                    break;

                case TokenType.Typeof:
                    if (ReturnValue is string) {
                        ReturnValue = "string";
                    } else if (ReturnValue is bool) {
                        ReturnValue = "boolean";
                    } else if (ReturnValue is double) {
                        ReturnValue = "number";
                    } else if (ReturnValue is FunctionExpression) {
                        ReturnValue = "function";
                    } else if (ReturnValue != null) {
                        ReturnValue = "object";
                    } else {
                        ReturnValue = null;
                    }
                    break;

                case TokenType.BitNot:
                    int i = ConvertToInteger(ReturnValue);
                    ReturnValue = i == 0 ? double.NaN : (double)~i;
                    break;

                case TokenType.Add:
                    ReturnValue = ConvertToNumber(ReturnValue);
                    break;

                case TokenType.Sub:
                    ReturnValue = -ConvertToNumber(ReturnValue);
                    break;

                default:
                    ReturnValue = double.NaN;
                    break;


            }

        }

        public void VisitVariableDeclaration(VariableDeclaration variableDeclaration) {
            Assign(variableDeclaration, variableDeclaration.Name.Name, variableDeclaration.Initialiser ?? new UndefinedExpression(), true);
        }

        public void VisitVariableStatement(VariableStatement variableStatement) {
            foreach (VariableDeclaration vd in variableStatement.Variables)
                VisitVariableDeclaration(vd);
        }

        public virtual void VisitWhileStatement(WhileStatement whileStatement) {
            VisitExpression(whileStatement.Condition);
            VisitStatement(whileStatement.Body);
        }

        public virtual void VisitWithStatement(WithStatement withStatement) {
            VisitStatement(withStatement.Body);
        }

        public void VisitAdditiveExpression(AdditiveExpression additiveExpression) {
            VisitExpression(additiveExpression.Left);
            object left = ReturnValue;

            VisitExpression(additiveExpression.Right);
            object right = ReturnValue;

            // 如果是加， 且有一个类型是 String ，则返回 String 。
            if (additiveExpression.Operator == TokenType.Add && (!IsNumberic(left) || !IsNumberic(right))) {
                ReturnValue = String.Concat(left, right);
            } else {
                ReturnValue = double.NaN;
                if (left != null && right != null) {
                    switch (additiveExpression.Operator) {
                        case TokenType.Add:
                            ReturnValue = ConvertToNumber(left) + ConvertToNumber(right);
                            break;
                        case TokenType.Sub:
                            ReturnValue = ConvertToNumber(left) - ConvertToNumber(right);
                            break;
                    }
                }
            }
        }

        public void VisitMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression) {
            VisitExpression(multiplicativeExpression.Left);
            object left = ReturnValue;

            VisitExpression(multiplicativeExpression.Right);
            object right = ReturnValue;

            ReturnValue = double.NaN;
            if (left != null && right != null) {
                switch (multiplicativeExpression.Operator) {
                    case TokenType.Mul:
                        ReturnValue = ConvertToNumber(left) * ConvertToNumber(right);
                        break;
                    case TokenType.Div:
                        ReturnValue = ConvertToNumber(left) / ConvertToNumber(right);
                        break;
                }
            }
        }

        public void VisitShiftExpression(ShiftExpression shiftExpression) {
            VisitExpression(shiftExpression.Left);
            object left = ReturnValue;

            VisitExpression(shiftExpression.Right);
            object right = ReturnValue;

            ReturnValue = double.NaN;
            if (left != null && right != null) {
                switch (shiftExpression.Operator) {
                    case TokenType.Shl:
                        int i = ConvertToInteger(left) << ConvertToInteger(right);
                        if (i != 0)
                            ReturnValue = (double)i;
                        break;
                    case TokenType.Shr:
                        int j = ConvertToInteger(left) >> ConvertToInteger(right);
                        if (j != 0)
                            ReturnValue = (double)j;
                        break;
                }
            }
        }

        public void VisitRelationalExpression(RelationalExpression relationalExpression) {
            VisitExpression(relationalExpression.Left);
            object left = ReturnValue;

            VisitExpression(relationalExpression.Right);
            object right = ReturnValue;

            ReturnValue = false;
            if (left != null && right != null) {
                if (IsNumberic(left) && IsNumberic(right)) {
                    switch (relationalExpression.Operator) {
                        case TokenType.Gt:
                            ReturnValue = ConvertToNumber(left) > ConvertToNumber(right);
                            break;
                        case TokenType.Gte:
                            ReturnValue = ConvertToNumber(left) >= ConvertToNumber(right);
                            break;
                        case TokenType.Lt:
                            ReturnValue = ConvertToNumber(left) < ConvertToNumber(right);
                            break;
                        case TokenType.Lte:
                            ReturnValue = ConvertToNumber(left) <= ConvertToNumber(right);
                            break;
                    }
                } else {
                    switch (relationalExpression.Operator) {
                        case TokenType.Gt:
                            ReturnValue = String.CompareOrdinal(left.ToString(), right.ToString()) > 0;
                            break;
                        case TokenType.Gte:
                            ReturnValue = String.CompareOrdinal(left.ToString(), right.ToString()) >= 0;
                            break;
                        case TokenType.Lt:
                            ReturnValue = String.CompareOrdinal(left.ToString(), right.ToString()) < 0;
                            break;
                        case TokenType.Lte:
                            ReturnValue = String.CompareOrdinal(left.ToString(), right.ToString()) <= 0;
                            break;
                    }
                }
            }


        }

        public void VisitEqualityExpression(EqualityExpression equalityExpression) {
            VisitExpression(equalityExpression.Left);
            object left = ReturnValue;

            VisitExpression(equalityExpression.Right);
            object right = ReturnValue;

            ReturnValue = null;
            if (left != null && right != null) {
                switch (equalityExpression.Operator) {
                    case TokenType.Eq:
                        ReturnValue = left == right;
                        break;
                    case TokenType.EqStrict:
                        ReturnValue = left == right;
                        break;
                    case TokenType.Ne:
                        ReturnValue = left != right;
                        break;
                    case TokenType.NeStrict:
                        ReturnValue = left != right;
                        break;
                }
            }
        }

        public void VisitBitwiseExpression(BitwiseExpression bitwiseExpression) {
            VisitExpression(bitwiseExpression.Left);
            object left = ReturnValue;

            VisitExpression(bitwiseExpression.Right);
            object right = ReturnValue;

            ReturnValue = double.NaN;
            if (left != null && right != null) {
                switch (bitwiseExpression.Operator) {
                    case TokenType.BitOr:
                        int i = ConvertToInteger(left) | ConvertToInteger(right);
                        if (i != 0)
                            ReturnValue = (double)i;
                        break;
                    case TokenType.BitXor:
                        int j = ConvertToInteger(left) ^ ConvertToInteger(right);
                        if (j != 0)
                            ReturnValue = (double)j;
                        break;
                    case TokenType.BitAnd:
                        int k = ConvertToInteger(left) & ConvertToInteger(right);
                        if (k != 0)
                            ReturnValue = (double)k;
                        break;
                }
            }
        }

        public void VisitLogicalExpression(LogicalExpression logicalExpression) {
            object last = ReturnValue;
            VisitExpression(logicalExpression.Left);
            VisitExpression(logicalExpression.Right);

            ReturnValue = ReturnValue ?? last;
        }

        public void VisitCommaExpression(CommaExpression commaExpression) {
            VisitExpression(commaExpression.Left);
            VisitExpression(commaExpression.Right);
        }

        public void VisitUserDefinedOperatorExpression(UserDefinedOperatorExpression userDefinedOperatorExpression) {
            VisitExpression(userDefinedOperatorExpression.Left);
            VisitExpression(userDefinedOperatorExpression.Right);
        }

        #endregion

    }

}


