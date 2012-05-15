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

        //#region 变量域

        ///// <summary>
        ///// 全局对象。
        ///// </summary>
        //Scope _global;

        ///// <summary>
        ///// 临时存放的变量。
        ///// </summary>
        //Variant _lastValue;

        ///// <summary>
        ///// 获取全局对象。
        ///// </summary>
        //public Scope Global {
        //    get {
        //        return _global;
        //    }
        //}

        ///// <summary>
        ///// 获取当前变量解析域。
        ///// </summary>
        //Scope CurrentScope {
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 获取或设置上一步执行代码返回的变量值。
        ///// </summary>
        //Variant ReturnValue {
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 创建函数作用域。
        ///// </summary>
        ///// <returns></returns>
        //Scope CreateScope(Variant parent) {
        //    Scope scope = new Scope();
        //    parent[String.Empty] = scope;
        //    return scope;
        //}

        ///// <summary>
        ///// 压入一个变量到作用域，之后全部变量都将添加到此变量下。
        ///// </summary>
        ///// <param name="scope">变量。</param>
        //void PushScope(Scope scope) {
        //    scope.Parent = CurrentScope;
        //    CurrentScope = scope;
        //}

        ///// <summary>
        ///// 弹出一个作用域。把作用域恢复到上一次 PushScope 之前的变量。
        ///// </summary>
        //void PopScope() {
        //    CurrentScope = CurrentScope.Parent;
        //}

        //private static Variant GetVariantOfParam(Variant f, string p) {
        //    string pt = f.GetParamType(p);
        //    if (pt != null) {
        //        return new Variant() { Type = pt, Name = p };
        //    }
        //    return new Variant() { Name = p };
        //}

        /////// <summary>
        /////// 获取和设置一个值，该值指示目前是否在全局对象。
        /////// </summary>
        /////// <value>如果目前是否在全局对象， 则 true; 否则, false。</value>
        ////bool IsGlobal {
        ////    get {
        ////        return CurrentScope == _global;
        ////    }
        ////}

        //#endregion

        //#region 创建变量

        ///// <summary>
        ///// 当前变量列表。
        ///// </summary>
        //VariantMap _map;

        ///// <summary>
        ///// 获取指定的文档节点附近定义的注释变量。并处理之前的全部注释变量。如果不存在变量，则创建变量。此函数不会返回相同的节点。
        ///// </summary>
        ///// <param name="node">节点。</param>
        ///// <returns>变量。</returns>
        //Variant GetVariantFor(Node node) {

        //    Variant v;

        //    // 获取一个变量，如果此变量之后还有可用的变量，直接处理。
        //    while (_map.GetNext(node.StartLocation, out v)) {

        //        // 处理不属于节点，但之前的变量。
        //        Process(v);
        //    }

        //    if (v == null || v.Processed) {
        //        v = new Variant() { StartLocation = node.StartLocation, EndLocation = node.StartLocation };
        //    } else {
        //        v.Processed = true;
        //        CheckNamespaceForOwnedVariant(v);
        //    }

        //    return v;
        //}

        ///// <summary>
        ///// 获取指定的文档节点附近定义的注释变量。并处理之前的全部注释变量。如果不存在变量，则返回 null 。
        ///// </summary>
        ///// <param name="node">节点。</param>
        ///// <returns>变量。</returns>
        //Variant GetVariantIf(Node node) {

        //    Variant v;

        //    // 获取一个变量，如果此变量之后还有可用的变量，直接处理。
        //    while (_map.GetNext(node.StartLocation, out v)) {

        //        // 处理不属于节点，但之前的变量。
        //        Process(v);
        //    }

        //    if (v != null && !v.Processed)
        //        CheckNamespaceForOwnedVariant(v);

        //    return v;
        //}

        ///// <summary>
        ///// 获取指定的文档节点附近定义的注释变量。并处理之前的全部注释变量。如果不存在变量，则返回 null 。
        ///// </summary>
        ///// <param name="node">节点。</param>
        ///// <returns>变量。</returns>
        //void ProcessVariantBefore(Location location) {

        //    Variant v;

        //    // 获取一个变量，如果此变量之后还有可用的变量，直接处理。
        //    while ((v = _map.DequeeBefore(location)) != null) {

        //        // 处理不属于节点，但之前的变量。
        //        Process(v);
        //    }

        //}

        ///// <summary>
        ///// 处理指定语句块内没有被处理的变量。
        ///// </summary>
        ///// <param name="blockStatement"></param>
        //void ProcessVariantBy(Node node) {

        //    Variant v;

        //    // 获取一个变量，如果此变量之后还有可用的变量，直接处理。
        //    while ((v = _map.DequeeBefore(node.EndLocation)) != null) {

        //        // 处理不属于节点，但之前的变量。
        //        Process(v);
        //    }

        //}

        ///// <summary>
        ///// 返回用于指定节点的父容器。
        ///// </summary>
        ///// <param name="node"></param>
        ///// <param name="parent">如果不是 null，则需要重新拷贝到此对象。</param>
        ///// <returns></returns>
        //Variant GetParentFor(Node node, out Variant parent) {

        //    // 获取并清空赋值的变量。
        //    Variant lastValue = _lastValue;
        //    _lastValue = null;

        //    //Variant v;

        //    // 优先使用适合指定节点的注释。
        //    //v = GetVariantIf(node);

        //    //if (v != null && !v.Processed) {
        //    //    v.Processed = true;
        //    //    parent = null;
        //    //    return v;
        //    //}

        //    // 然后使用赋值时的变量。
        //    // 当对象是赋值操作的时候，不再拷贝成员到当前父成员。
        //    // 所以 设置 parent = null 。
        //    if (lastValue != null) {

        //        parent = null;

        //        if (lastValue.NamespaceSetter != null && lastValue.NamespaceSetter.Length > 0) {
        //            GenerateNamespace(lastValue);
        //            parent = lastValue.NamespaceSetterVariant;
        //        }


        //        return lastValue;
        //    }

        //    //// 处理之前的变量，以更新父元素。
        //    //ProcessVariantBefore(node.StartLocation);

        //    // 无目标成员，只能使用当前的父成员。
        //    parent = CurrentParent;

        //    // 最后使用临时变量。
        //    return new Variant();
        //}

        /////// <summary>
        /////// 返回用于指定节点的父容器。
        /////// </summary>
        /////// <param name="node"></param>
        /////// <returns></returns>
        ////Variant GetParentFor2(Node node) {

        ////    Variant v;

        ////    //// 优先使用适合指定节点的注释。
        ////    //v= GetVariantIf(node);

        ////    //if (v != null) {
        ////    //    _lastValue = null;
        ////    //    return v;
        ////    //}

        ////    // 然后使用赋值时的变量。
        ////    if (_lastValue != null) {
        ////        v = _lastValue;
        ////        _lastValue = null;
        ////        return v;
        ////    }

        ////    // 处理之前的变量，以更新父元素。
        ////    ProcessVariantBefore(node.StartLocation);

        ////    // 然后使用当前的父元素。
        ////    if (CurrentScope.CurrentParent != null) {
        ////        return CurrentScope.CurrentParent;
        ////    }

        ////    // 最后使用临时变量。
        ////    return new Variant();
        ////}

        //Scope GetVariantParent(Scope c, string name) {
        //    while (c.Parent != c && !c.HasVariant(name)) {
        //        c = c.Parent;
        //    }

        //    return c;
        //}

        ///// <summary>
        ///// 根据指定的名字空间返回变量，如果不存在变量，则创建变量。
        ///// </summary>
        ///// <param name="ns"></param>
        ///// <returns></returns>
        //Variant GetOrCreateVariantByName(Scope p, string name) {
        //    Scope old = p;
        //    while (p.Parent != p) {
        //        if (p.HasVariant(name))
        //            return p[name];
        //        p = p.Parent;
        //    }

        //    return p[name] ?? (old[name] = new Variant() { Name = name });
        //}

        ///// <summary>
        ///// 根据指定的名字空间返回变量，如果不存在变量，则创建变量。
        ///// </summary>
        ///// <param name="ns"></param>
        ///// <returns></returns>
        //Variant GetOrCreateProptery(Variant p, string name) {
        //    return p.GetVariant(name) ?? (p[name] = new Variant() { Name = name });
        //}

        ///// <summary>
        ///// 根据指定的名字空间返回变量，如果不存在变量，则创建变量。
        ///// </summary>
        ///// <param name="ns"></param>
        ///// <returns></returns>
        //Variant GetOrCreateVariantByNamespace(string ns) {
        //    Variant v = _global;

        //    if (ns != null) {
        //        foreach (string s in ns.Split('.')) {
        //            v = GetOrCreateProptery(v, s);
        //        }
        //    }
        //    return v;
        //}

        ///// <summary>
        ///// 根据指定的名字空间返回变量，如果不存在变量，则创建变量。
        ///// </summary>
        ///// <param name="ns"></param>
        ///// <returns></returns>
        //void SetVariantByNamespace(string ns, Variant v) {
        //    string[] vals = ns.Split('.');
        //    Variant c = _global;
        //    for (int i = 0, len = vals.Length - 1; i < len; i++) {
        //        c = GetOrCreateProptery(c, vals[i]);
        //    }

        //    AddVariant(c, vals[vals.Length - 1], v);
        //}

        /////// <summary>
        /////// 返回指定节点之前的最邻的注释，并从注释队列中移除返回值。
        /////// </summary>
        /////// <param name="node">节点。</param>
        /////// <returns>注释。</returns>
        ////Comment GetCommentFor(CorePlus.Parser.Javascript.Node node) {

        ////    Comment lastComment = _lastComment;

        ////    _lastComment = null;

        ////    long offs;

        ////redo:



        ////    if (_comments.Count == 0)
        ////        return lastComment;

        ////    offs = (long)node.StartLocation.Line - (long)_comments.Peek().EndLocation.Line;


        ////    if (offs < 0)
        ////        return _lastComment;
        ////    if (offs == 1) {
        ////        Comment c = _comments.Dequeue();
        ////        if (!c.Ignore) {
        ////            return c;
        ////        }


        ////        goto redo;
        ////    }
        ////    if (offs == 0) {
        ////        if (node.StartLocation.Column >= _comments.Peek().EndLocation.Column) {
        ////            Comment c = _comments.Dequeue();
        ////            if (!c.Ignore) {
        ////                return c;
        ////            }
        ////        }
        ////        return lastComment;
        ////    }


        ////    DequeueAndCheckComment();

        ////    goto redo;
        ////}

        /////// <summary>
        /////// 返回指定节点之前的最邻的注释并使用指定的信息填充，然后从注释队列中移除返回值。
        /////// </summary>
        /////// <param name="node">节点。</param>
        /////// <param name="name">名字。</param>
        /////// <param name="member">可能的类型。</param>
        /////// <returns>注释。</returns>
        ////Comment GetCommentFor(CorePlus.Parser.Javascript.Node node, string name = null, CommentNodeNames member = null) {
        ////    return FillComment(GetCommentFor(node), name, member);
        ////}

        /////// <summary>
        /////// 处理属于一个文档注释。
        /////// </summary>
        /////// <param name="dc">分析的注释对象。</param>
        /////// <param name="mostName">最肯能是的名字。</param>
        /////// <param name="mostMember">最肯能是的类型。</param>
        ////private void ProcessComment(Comment dc, string mostName, Comment mostMember) {

        ////}

        /////// <summary>
        /////// 使用智能分析来完成注释。
        /////// </summary>
        /////// <param name="dc">要填补的注释。</param>
        /////// <param name="name">名字。</param>
        /////// <param name="member">类型。</param>
        /////// <returns>分析后的新注释，肯能等于 <paramref name="dc"/> 。</returns>
        ////Comment FillComment(Comment dc, string name = null, CommentNodeNames member = null) {
        ////    if (dc == null || dc.Ignore)
        ////        dc = new Comment();

        ////    // 如果注释里没有名字，且解析时分析了名字，则使用解析器的名字。
        ////    if (dc.DisplayName == null && name != null)
        ////        dc.DisplayName = name;

        ////    // 如果注释里没有名字，且解析时分析了名字，则使用解析器的名字。
        ////    if (dc.Member == null && member != null)
        ////        dc.Member = member;

        ////    return dc;
        ////}

        /////// <summary>
        /////// 处理属于一个节点范围内的全部剩下的注释。
        /////// </summary>
        /////// <param name="node"></param>
        ////void DequeueAndCheckBetween(CorePlus.Parser.Javascript.Node node) {

        ////    //   处理空函数
        ////    while (_comments.Count > 0) {

        ////        Comment dc = _comments.Peek();

        ////        long offs = (long)node.EndLocation.Line - (long)dc.EndLocation.Line;

        ////        if (offs > 0 || (offs == 0 && node.EndLocation.Column > dc.EndLocation.Column)) {
        ////            DequeueAndCheckComment();
        ////        }

        ////        break;
        ////    }
        ////}

        /////// <summary>
        /////// 访问一个独立的节点。
        /////// </summary>
        ////void DequeueAndCheckComment() {
        ////    CheckNamespace(_comments.Dequeue());
        ////}

        //#endregion

        //#region 处理变量和变量填充

        ///// <summary>
        ///// 获取当前用于存放子变量的父变量。
        ///// </summary>
        //Variant CurrentParent {
        //    get {

        //        // 首先使用属于当前语句的父变量。
        //        Variant v = GetVariantIf(CurrentScope.CurrentStatement);

        //        if (v != null && v.NamespaceSetter != null && v.NamespaceSetter.Length > 0) {
        //            if (v.NamespaceSetterVariant == null)
        //                GenerateNamespace(v);
        //            return v.NamespaceSetterVariant;
        //        }

        //        // 如果不存在，则取最后一次。
        //        return CurrentScope.LastParent;
        //    }
        //}

        //void GenerateNamespace(Variant variant) {

        //    string[] vals = variant.NamespaceSetter.Split('.');
        //    Variant c = _global;
        //    for (int i = 0, len = vals.Length - 1; i < len; i++) {
        //        c = GetOrCreateProptery(c, vals[i]);
        //    }

        //    string name = vals[vals.Length - 1];
        //    if (c.HasVariant(name)) {
        //        variant.NamespaceSetterVariant = c[name];
        //        variant.NamespaceSetterVariant.MemberType = variant.MemberType;
        //    } else {
        //        c[name] = variant.NamespaceSetterVariant = variant;
        //    }
        //}

        ///// <summary>
        ///// 所有添加的注释都会执行此函数。
        ///// </summary>
        ///// <param name="variant"></param>
        //void CheckNamespace(Variant variant) {

        //    if (variant.NamespaceSetter != null) {

        //        // 空字符串时清除名字空间。
        //        if (variant.NamespaceSetter.Length == 0) {
        //            CurrentScope.LastParent = null;
        //        } else if (variant.NamespaceSetterVariant == null) {
        //            GenerateNamespace(variant);
        //            CurrentScope.LastParent = variant.NamespaceSetterVariant;
        //        }


        //    }



        //}

        //void CheckNamespaceForOwnedVariant(Variant v) {
        //    if (_project.EnableGlobalNamespaceSetter)
        //        CheckNamespace(v);
        //}

        ///// <summary>
        ///// 处理一个独立的注释。
        ///// </summary>
        ///// <param name="variant"></param>
        //void Process(Variant variant) {

        //    CheckNamespace(variant);

        //    // 如果设置了名字，说明是变量。
        //    if (variant.Name != null) {

        //        Variant p;

        //        // 如果在当前语句前定义，则表示和当前语句无关。
        //        if (variant.EndLocation < CurrentScope.CurrentStatement.StartLocation) {
        //            p = CurrentScope.LastParent;
        //        } else {
        //            p = CurrentParent;
        //        }

        //        Assign(p ?? CurrentScope, variant.Name, variant);

        //    }


        //}

        ///// <summary>
        ///// 在当前作用域添加一个预定义的变量。同时处理变量已存在问题。此函数实现添加的底层功能。
        ///// </summary>
        ///// <param name="variant">要处理的变量。</param>
        //Variant AddVariant(Variant parent, string name, Variant variant, bool useNew = true) {

        //    // 如果父成员是类，且当前成员不是静态的，则添加到 prototype 属性。
        //    if (!_project.EnableAutoCreateStatic && variant.MemberAttribute != CorePlus.Api.MemberAttributes.Static && variant.MemberType != MemberType.Class && variant.MemberType != MemberType.Enum && variant.MemberType != MemberType.Interface && variant.MemberType != MemberType.Package && parent.MemberType == MemberType.Class && parent.MemberAttribute != CorePlus.Api.MemberAttributes.Static) {
        //        parent = parent.GetOrCreateVariant("prototype");
        //    }

        //    // 如果已经存在此变量。
        //    if (parent.HasVariant(name)) {

        //        if (useNew) {

        //            // 拷贝全部属性。
        //            CopyProperties(variant, parent[name]);

        //            // 同步父作用域。
        //            CurrentScope.UpdateParent(parent[name], variant);

        //        } else {

        //            // 拷贝全部属性。
        //            CopyProperties(parent[name], variant);

        //            return parent[name];
        //        }

        //    }

        //    // 自动填充名字。
        //    if (variant.Name == null)
        //        variant.Name = name;
        //    parent[name] = variant;

        //    return variant;
        //}

        //void AutoFillValue(Variant dest, Variant src, bool autoStatic) {
        //    if (dest == src)
        //        return;
        //    if (dest.Type == null) {
        //        dest.Type = src.Type;
        //    }

        //    if (dest.Value == null) {
        //        dest.Value = src.Value;
        //    }

        //    CopyVariants(dest, src);
        //}

        //void CopyVariants(Variant dest, Variant src) {
        //    if (dest != src) {

        //        // 如果父成员是类，且当前成员不是静态的，则添加到 prototype 属性。
        //        foreach (var kv in src) {
        //            AddVariant(dest, kv.Key, kv.Value);
        //        }

        //    }
        //}

        //void CopyProperties(Variant dest, Variant src) {
        //    if (dest != src) {
        //        src.CopyTo(dest);
        //    }
        //}

        ///// <summary>
        ///// 执行赋值操作。
        ///// </summary>
        ///// <param name="parent">赋值的父对象。</param>
        ///// <param name="value">变量。</param>
        //void Assign(Variant parent, string name, Variant value) {

        //    // 仅仅存在名字才可实行赋值操作。
        //    if (name != null) {

        //        // 如果定义了 MemberOf， 则覆盖系统的 parent 。
        //        if (value.MemberOf != null) {

        //            // 检测成员的 memberOf
        //            parent = GetOrCreateVariantByNamespace(value.MemberOf);
        //        }

        //        AddVariant(parent, name, value);
        //    }

        //    // 如果定义了别名。
        //    if (value.Alias != null) {
        //        foreach (string alias in value.Alias) {
        //            SetVariantByNamespace(alias, value);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 执行赋值操作。
        ///// </summary>
        ///// <param name="parent">赋值的父对象。</param>
        ///// <param name="name">变量名。肯能为 null 。</param>
        ///// <param name="node">全部赋值语句。</param>
        ///// <param name="e">要更新的值表达式。</param>
        //void Assign(Variant parent, string name, Node node, Expression e) {

        //    // a = 1

        //    // 处理赋值的目标。
        //    Variant value = GetVariantFor(node);

        //    // 底层的赋值操作。
        //    Assign(parent, name ?? value.Name, value);

        //    // 如果存在用于设置的名字空间对象，则同时拷贝到名字空间对象。
        //    //if(currentParent != null && value.DisplayName != null) {
        //    //    AddVariant(parent, value.DisplayName, value);
        //    //}

        //    //bool notADocument = !parent.HasVariant(name) && value.DisplayName == null;

        //    // 如果是子变量定义表达式，则保存值。
        //    if (e is ObjectLiteral || e is FunctionExpression || e is ConditionalExpression || e is LogicalExpression) {
        //        _lastValue = value;
        //    }

        //    VisitExpression(e);

        //    AutoFillValue(value, ReturnValue, true);

        //    //// 值类型需要拷贝变量，不能直接赋值。
        //    //// 如果已经存在此名字的变量，则合并文档。
        //    //// 如果定义的类有新的名字，则合并文档，不赋值。
        //    //if (!notADocument || ReturnValue.IsValueType) {
        //    //    AutoFillValue(value, ReturnValue);
        //    //} else if (ReturnValue != value) {

        //    //    // 真正的赋值。
        //    //    CopyProperties(ReturnValue, value);
        //    //    parent[name] = ReturnValue;
        //    //}

        //}

        ////#region 变量

        //////private static bool IsClassName(string memberOf) {
        //////    int i = memberOf.LastIndexOf('.') + 1;
        //////    char c = memberOf.Length > i ? memberOf[i] : '\0';

        //////    return char.IsUpper(c);
        //////}

        //////static void SplitMember(string value, out string ns, out string name) {
        //////    int i = value.LastIndexOf('.');
        //////    ns = value.Substring(0, i);
        //////    name = Str.Substring(value, i + 1);
        //////}

        //////static bool IsParentSetter(Variant variant) {
        //////    return GetTopLeval(variant.Comment.Member) == CommentNodeNames.Namespace;
        //////}

        ///////// <summary>
        ///////// 获取一个成员上级的成员。
        ///////// </summary>
        ///////// <param name="node">节点。</param>
        ///////// <returns>父类成员。</returns>
        ///////// <remarks>
        ///////// 方法、属性、字段、配置、构造函数、事件 的上级成员是 类。
        ///////// 类、枚举、名字空间 的上级成员是 名字空间。
        ///////// </remarks>
        //////static CommentNodeNames GetTopLeval(CommentNodeNames node) {
        //////    if (node == null)
        //////        return null;
        //////    if (node == CommentNodeNames.Property || node == CommentNodeNames.Method ||
        //////        node == CommentNodeNames.Field || node == CommentNodeNames.Event ||
        //////        node == CommentNodeNames.Constructor || node == CommentNodeNames.Config || node == CommentNodeNames.Getter || node == CommentNodeNames.Setter) {
        //////        return CommentNodeNames.Class;
        //////    }

        //////    //  else if (node == CommentNodeNames.Class || node == CommentNodeNames.Namespace || node == CommentNodeNames.Enum) {
        //////    //    return CommentNodeNames.Namespace;
        //////    //}

        //////    return CommentNodeNames.Namespace;
        //////}

        ///////// <summary>
        ///////// 根据注释创建并添加新变量。
        ///////// </summary>
        ///////// <param name="dc">注释。</param>
        ///////// <returns>刚添加的变量。</returns>
        //////Variant AddVariant(Comment dc, bool enableScope = false) {

        //////    // 如果不存在名字，无法返回成员。
        //////    if (dc.DisplayName == null)
        //////        return null;

        //////    Variant p;

        //////    // 这个变量已经自己定义了属于的范围。
        //////    if (dc.MemberOf != null) {
        //////        p = _global.GetOrCreateNamespace(dc.MemberOf);
        //////    } else {
        //////        p = enableScope ? CurrentScope : CurrentParent;
        //////    }

        //////    p = p.GetVariant(dc.DisplayName);

        //////    // 拷贝注释。
        //////    if (p.Comment == null)
        //////        p.Comment = dc;
        //////    else
        //////        dc.CopyTo(p.Comment);

        //////    return p;

        //////}

        ///////// <summary>
        ///////// 找到一个注释，因此添加到当前作用域。
        ///////// </summary>
        ///////// <param name="dc">注释。</param>
        ///////// <param name="name">名字。</param>
        ///////// <param name="type">类型。</param>
        ///////// <returns>刚添加的变量。</returns>
        //////Variant AddVariant(ref Comment dc, string name = null, CommentNodeNames member = null) {

        //////    if (dc == null || dc.Ignore)
        //////        dc = new Comment();

        //////    // 如果注释里没有成员类别，且解析时分析了成员类别，则使用解析器的成员类别。
        //////    if (member != null && dc.Member == null)
        //////        dc.Member = member;

        //////    Variant p;

        //////    // 如果是定义空间，则自定义转换的方法。
        //////    if (!HandleNamespaceSetter(dc)) {

        //////        // 这个变量已经自己定义了属于的范围。
        //////        if (dc.MemberOf != null) {
        //////            p = _global.GetOrCreateNamespace(dc.MemberOf);
        //////        } else {
        //////            p = CurrentScope;
        //////        }

        //////        // 如果不存在名字，无法返回成员。
        //////        if (name == null)
        //////            return null;

        //////        p = p.GetVariant(name);

        //////    } else {

        //////        p = CurrentScope;

        //////    }

        //////    // 拷贝注释。
        //////    if (p.Comment == null)
        //////        p.Comment = dc;
        //////    else
        //////        dc.CopyTo(p.Comment);

        //////    return p;
        //////}


        ////// TODO  inline    

        ///////// <summary>
        ///////// 如果一个注释是一个名字空间，则设置名字空间，同时会创建名字空间所属变量。
        ///////// </summary>
        ///////// <param name="dc"></param>
        ///////// <returns></returns>
        //////bool CheckNamespace(Comment dc) {
        //////    if (dc != null && !dc.Processed && GetTopLeval(dc.Member) == CommentNodeNames.Namespace) {



        //////        dc.Processed = true;

        //////        // 不存在名字。表示清空空间。
        //////        if (dc.DisplayName == null)
        //////            PopScope();
        //////        else {
        //////            // 这里设置名字空间。
        //////            SetNamespace(dc);
        //////            return true;
        //////        }
        //////    }

        //////    return false;
        //////}

        ///////// <summary>
        ///////// 分析节点，检查其是否可以成为名字空间，如果符合要求，设置。
        ///////// </summary>
        ///////// <param name="dc">注释。</param>
        ///////// <param name="name">已知的名字。</param>
        ///////// <remarks>
        ///////// /**@class */ 等结构被解析成新的名字空间。
        ///////// /**@member */ 等结构被解析成普通成员。
        ///////// </remarks>
        //////bool HandleNamespaceSetter(Comment dc) {

        //////    // 获取上级。
        //////    CommentNodeNames topLeval = GetTopLeval(dc.Member);

        //////    // 如果上级是 namespace， 说明以下成员可作为此成员的成员。 如 class
        //////    if (topLeval == CommentNodeNames.Namespace) {


        //////        // 不存在名字。表示清空空间。
        //////        if (dc.DisplayName == null)
        //////            ClearNamespace();
        //////        else
        //////            // 这里设置名字空间。
        //////            SetNamespace(dc);

        //////        return true;
        //////    }



        //////    return false;

        //////}

        ///////// <summary>
        ///////// 重新设置当前的全局名字空间。
        ///////// </summary>
        ///////// <param name="dc">指明了名字空间的成员。</param>
        //////void SetNamespace(Comment dc) {

        //////    // 这个变量已经自己定义了属于的范围。
        //////    // 如果当前不是全局的名字空间
        //////    // 并且不允许多级的名字空间
        //////    // 先删除已存在的名字空间。
        //////    Variant p = dc.MemberOf != null ? _global.GetOrCreateNamespace(dc.MemberOf) : CurrentParent != _global && !_project.EnableMultiNamespace ? _global : CurrentParent;

        //////    p = p.GetVariant(dc.DisplayName);

        //////    // 拷贝注释。
        //////    if (p.Comment == null)
        //////        p.Comment = dc;
        //////    else
        //////        dc.CopyTo(p.Comment);

        //////    PushScope(p);
        //////}

        ///////// <summary>
        ///////// 清除名字空间。
        ///////// </summary>
        //////void ClearNamespace() {

        //////    if (_project.DisableNamespaceClear)
        //////        return;

        //////    // 恢复到全局的成员。
        //////    PopScope();
        //////}

        ////#endregion

        //#endregion

        //#region 初始化和配置

        ///// <summary>
        ///// 正在处理的项目配置。
        ///// </summary>
        //DocProject _project;

        ///// <summary>
        ///// 初始化 <see cref="DocPlus.DocParser.Javascript.DocAstVistor"/> 的新实例。
        ///// </summary>
        //public DocAstVistor(DocProject project) {
        //    _project = project;
        //    _global = new Scope();
        //}

        //void InitSystemDefinedVariant() {
        //    _global["window"] = _global["this"] = _global;
        //}

        ///// <summary>
        ///// 开始对指定的节点解析。
        ///// </summary>
        ///// <param name="script">语法树。</param>
        ///// <param name="comments">所有注释。</param>
        ///// <returns>全局对象。所有变量都可从这个节点找到。</returns>
        //public void Parse(Script script, VariantMap comments) {

        //    _map = comments;

        //    _global.Parent = _global;
        //    CurrentScope = _global;

        //    InitSystemDefinedVariant();
        //    VisitScript(script);

        //}

        //#endregion

        //#region IAstVisitor 成员

        //[DebuggerStepThrough()]
        //public void VisitScript(Script script) {

        //    // 访问语句。
        //    VisitBlock(script);

        //    Variant v;

        //    // 获取一个变量，如果此变量之后还有可用的变量，直接处理。
        //    while ((v = _map.DequeeAll()) != null) {

        //        // 处理不属于节点，但之前的变量。
        //        Process(v);
        //    }
        //}

        //[DebuggerStepThrough()]
        //public void VisitStatements(NodeList<Statement> statements) {
        //    foreach (Statement s in statements) {
        //        VisitStatement(s);
        //    }
        //}

        //public void VisitStatement(Statement statement) {

        //    // 保存当前执行的变量，以便回查从属。
        //    CurrentScope.CurrentStatement = statement;

        //    // 遍历语句内容。
        //    statement.Accept(this);

        //    ReturnValue = Variant.Undefined;
        //}

        //[DebuggerStepThrough()]
        //public void VisitExpression(Expression expression) {
        //    expression.Accept(this);
        //}

        //public void VisitArrayLiteral(ArrayLiteral arrayLiteral) {

        //    Variant v = new Variant();

        //    int i = 0;
        //    foreach (Expression e in arrayLiteral.Values) {
        //        Assign(v, (i++).ToString(), e, e);
        //    }

        //    if (v.Type == null) {
        //        v.Type = "Array";
        //    }

        //    ReturnValue = v;
        //}

        //public void VisitAssignmentExpression(AssignmentExpression assignmentExpression) {

        //    IndexCallExpression e = assignmentExpression.Target as IndexCallExpression;

        //    // obj[index]
        //    if (e != null) {

        //        // 获取父对象。
        //        VisitExpression(e.Target);
        //        Variant p = ReturnValue;

        //        // 获取值对象。
        //        VisitExpression(e.Argument);
        //        Variant c = ReturnValue;

        //        // 如果可以根据值对象获取其字符串，则添加属性。
        //        if (c.Value != null) {

        //            Assign(p, c.Value.ToString(), assignmentExpression, assignmentExpression.Value);

        //        } else {

        //            Assign(p, null, assignmentExpression, assignmentExpression.Value);
        //        }

        //    } else {

        //        PropertyCallExpression e2 = assignmentExpression.Target as PropertyCallExpression;

        //        if (e2 != null) {

        //            // 获取父对象。
        //            VisitExpression(e2.Target);

        //            Assign(ReturnValue, e2.Argument, assignmentExpression, assignmentExpression.Value);

        //        } else {
        //            Identifier e3 = assignmentExpression.Target as Identifier;

        //            if (e3 != null) {

        //                Assign(GetVariantParent(CurrentScope, e3.Name), e3.Name, assignmentExpression, assignmentExpression.Value);

        //            } else {

        //                Assign(CurrentScope, null, assignmentExpression, assignmentExpression.Value);

        //            }

        //        }

        //    }

        //}

        //public void VisitBlock(Block blockStatement) {
        //    VisitStatements(blockStatement.Statements);

        //    ProcessVariantBy(blockStatement);
        //}

        //public void VisitBreakStatement(BreakStatement breakStatement) {
        //    // EMPTY
        //}

        //public void VisitCallNative(CallNative callNative) {
        //    // EMPTY
        //}

        //public void VisitCaseClause(CaseClause caseLabel) {

        //    if (caseLabel.Label != null) {
        //        VisitExpression(caseLabel.Label);
        //    }




        //    VisitStatements(caseLabel.Statements);
        //}

        //public void VisitConditionalExpression(ConditionalExpression conditionalExpression) {
        //    Variant lastValue = _lastValue;
        //    VisitExpression(conditionalExpression.Condition);

        //    _lastValue = lastValue;
        //    VisitExpression(conditionalExpression.ThenExpression);
        //    Variant left = ReturnValue;

        //    _lastValue = lastValue;
        //    VisitExpression(conditionalExpression.ElseExpression);
        //    Variant right = ReturnValue;

        //    _lastValue = null;

        //    if (left.Value != null) {
        //        ReturnValue = left;
        //    } else if (right.Value != null) {
        //        ReturnValue = right;
        //    }
        //}

        //public void VisitContinueStatement(ContinueStatement continueStatement) {
        //    // EMPTY
        //}

        //public void VisitPostfixExpression(PostfixExpression countOperation) {
        //    VisitExpression(countOperation.Expression);
        //    Variant v = ReturnValue;
        //    if (v.Value != null) {
        //        ReturnValue = new Variant() { Type = "Number" };
        //        switch (countOperation.Operator) {
        //            case TokenType.Inc:
        //                ReturnValue.Value = v.ToNumber() + 1;
        //                break;
        //            case TokenType.Dec:
        //                ReturnValue.Value = v.ToNumber() - 1;
        //                break;
        //        }
        //    } else {
        //        ReturnValue = new Variant() { Type = "Number" };
        //    }
        //}

        //public void VisitConstStatement(ConstStatement constStatement) {
        //    VisitVariableStatement(constStatement);
        //}

        //public void VisitDebuggerStatement(DebuggerStatement debuggerStatement) {
        //    // EMPTY
        //}

        //public void VisitDoWhileStatement(DoWhileStatement doWhileStatement) {
        //    VisitExpression(doWhileStatement.Condition);
        //    VisitStatement(doWhileStatement.Body);
        //}

        //public void VisitEmptyStatement(EmptyStatement emptyStatement) {
        //    // EMPTY
        //}

        //public void VisitExpressionStatement(ExpressionStatement expressionStatement) {
        //    VisitExpression(expressionStatement.Expression);
        //}

        //public void VisitFalseLiteral(FalseLiteral falseLiteral) {
        //    ReturnValue = Variant.False;
        //}

        //public virtual void VisitForInStatement(ForInStatement forinStatement) {
        //    VisitStatement(forinStatement.Each);
        //    VisitExpression(forinStatement.Enumerable);
        //    VisitStatement(forinStatement.Body);
        //}

        //public virtual void VisitForStatement(ForStatement forStatement) {
        //    VisitStatement(forStatement.InitStatement);
        //    VisitExpression(forStatement.Condition);
        //    VisitExpression(forStatement.NextExpression);
        //    VisitStatement(forStatement.Body);
        //}

        //public void VisitFunctionExpression(FunctionExpression functionExpression) {

        //    Variant parent;
        //    Variant v = GetParentFor(functionExpression, out parent);

        //    if (v.Name == null) {
        //        v.Name = functionExpression.Name;
        //    }

        //    if (v.Type == null) {
        //        v.Type = "Function";
        //    }

        //    if (v.Value == null) {
        //        v.Value = "function(){}";
        //    }

        //    Scope scope = CreateScope(v);
        //    scope.LastParent = parent;
        //    //  CurrentScope.CopyParent(scope);
        //    PushScope(scope);

        //    foreach (Identifier p in functionExpression.Params) {
        //        AddVariant(scope, p.Name, GetVariantOfParam(v, p.Name));
        //    }

        //    VisitStatements(functionExpression.Statements);
        //    ProcessVariantBy(functionExpression);

        //    if (v.ReturnType == null) {
        //        v.ReturnType = scope.ReturnType ?? "Undefined";
        //    }

        //    PopScope();

        //    ReturnValue = v;
        //    if (parent != null) {
        //        AutoFillValue(parent, v, _project.EnableAutoCreateStatic);
        //    }
        //}

        ///// <summary>
        ///// ss
        ///// </summary>
        ///// <param name="functionCallExpression"></param>
        //public void VisitFunctionCallExpression(FunctionCallExpression functionCallExpression) {

        //    VisitExpression(functionCallExpression.Target);

        //    Variant r;

        //    if (ReturnValue.Type == null) {
        //        ReturnValue.Type = "Function";
        //    }

        //    r = Variant.Create(ReturnValue.ReturnType);

        //    foreach (Expression s in functionCallExpression.Arguments) {
        //        VisitExpression(s);
        //    }

        //    ReturnValue = r;
        //}

        //public void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration) {
        //    Assign(CurrentScope, functionDeclaration.Name, functionDeclaration, functionDeclaration.Function);
        //    VisitFunctionExpression(functionDeclaration.Function);
        //}

        //public void VisitLabelledStatement(LabelledStatement labelledStatement) {
        //    VisitStatement(labelledStatement.Statement);
        //}

        //public void VisitNewExpression(NewExpression newExpression) {

        //    //     Comment dc = GetCommentFor(newExpression.Expression);
        //    //    AutoFillNodeType(CommentNodeNames.Class);

        //    //VisitExpression(newExpression.Expression);

        //    //if (ReturnType == "Function") {

        //    //}

        //    //Return(dc.DisplayName ?? "Class", null);
        //}

        //public void VisitNumberLiteral(NumberLiteral numberLiteral) {
        //    ReturnValue = new Variant() { Type = "Number", Value = Variant.ConvertToNumber(numberLiteral.Value) };
        //}

        //public void VisitNullLiteral(NullLiteral nullLiteral) {
        //    ReturnValue = Variant.Null;
        //}

        //public void VisitIdentifier(Identifier identifier) {
        //    ReturnValue = GetOrCreateVariantByName(CurrentScope, identifier.Name);
        //}

        //public void VisitIndexCallExpression(IndexCallExpression indexCallExpression) {
        //    VisitExpression(indexCallExpression.Target);
        //    Variant left = ReturnValue;

        //    VisitExpression(indexCallExpression.Argument);
        //    Variant right = ReturnValue;

        //    if (right.Value == null) {
        //        ReturnValue = Variant.Undefined;
        //    } else {
        //        ReturnValue = GetOrCreateProptery(left, right.Value.ToString());
        //    }
        //}

        //public void VisitIfStatement(IfStatement ifStatement) {
        //    VisitExpression(ifStatement.Condition);
        //    VisitStatement(ifStatement.Then);
        //    VisitStatement(ifStatement.Else);
        //}

        //public void VisitIncrementOperation(IncrementOperation incrementOperation) {
        //    VisitExpression(incrementOperation.Expression);
        //}

        //public void VisitObjectLiteral(ObjectLiteral objectLiteral) {

        //    Variant parent;
        //    Variant v = GetParentFor(objectLiteral, out  parent);

        //    if (v.Type == null) {
        //        v.Type = "Object";
        //    }

        //    foreach (ObjectLiteral.Property p in objectLiteral.Value) {
        //        VisitProperty(p, v);
        //    }

        //    ReturnValue = v;

        //    if (parent != null) {
        //        AutoFillValue(parent, v, _project.EnableAutoCreateStatic);
        //    }
        //}

        //public void VisitParamedExpression(ParamedExpression paramedExpression) {
        //    VisitExpression(paramedExpression.Expression);
        //}

        //void VisitProperty(ObjectLiteral.Property p, Variant parent) {
        //    Assign(parent, p.Key.Value, p, p.Value);
        //}

        //public void VisitProperty(ObjectLiteral.Property property) {
        //    VisitProperty(property, CurrentParent);
        //}

        //public void VisitPropertyCallExpression(PropertyCallExpression propertyCallExpression) {

        //    VisitExpression(propertyCallExpression.Target);
        //    Variant left = ReturnValue;

        //    ReturnValue = GetOrCreateProptery(left, propertyCallExpression.Argument);



        //}

        //public void VisitRegExpLiteral(RegExpLiteral regExpLiteral) {
        //    ReturnValue = new Variant() { Type = "RegExp", Value = regExpLiteral.Value };
        //}

        //public void VisitReturnStatement(ReturnStatement returnStatement) {
        //    if (returnStatement.Expression != null) {
        //        VisitExpression(returnStatement.Expression);
        //        if (CurrentScope.ReturnType == null) {
        //            CurrentScope.ReturnType = ReturnValue.Type;
        //        }

        //    }
        //}

        //public void VisitSemicolon(Semicolon semicolon) {
        //    //   EMPTY     
        //}

        //public void VisitStringLiteral(StringLiteral stringLiteral) {
        //    ReturnValue = new Variant() { Type = "String", Value = stringLiteral.Value };
        //}

        //public void VisitSwitchStatement(SwitchStatement switchStatement) {
        //    VisitExpression(switchStatement.Condition);
        //    foreach (CaseClause c in switchStatement.Cases) {
        //        VisitCaseClause(c);
        //    }
        //}

        //public void VisitThisLiteral(ThisLiteral thisLiteral) {
        //    ReturnValue = CurrentScope.CurrentParent ?? CurrentScope;
        //}

        //public void VisitThrowStatement(ThrowStatement throwStatement) {
        //    VisitExpression(throwStatement.Expression);

        //    if (ReturnValue.Value != null) {

        //        if (CurrentScope.Exceptions == null) {
        //            CurrentScope.Exceptions = new List<string[]>();
        //        }

        //        CurrentScope.Exceptions.Add(new string[] { ReturnValue.Type, ReturnValue.Value.ToString() });

        //    }
        //}

        //public void VisitTrueLiteral(TrueLiteral trueLiteral) {
        //    ReturnValue = Variant.True;
        //}

        //public virtual void VisitTryCatchFinallyStatement(TryCatchFinallyStatement tryCatchFinallyStatement) {
        //    VisitBlock(tryCatchFinallyStatement.TryBlock);
        //    VisitBlock(tryCatchFinallyStatement.CatchBlock);
        //    VisitBlock(tryCatchFinallyStatement.FinallyBlock);
        //}

        //public virtual void VisitTryCatchStatement(TryCatchStatement tryCatchStatement) {
        //    VisitBlock(tryCatchStatement.TryBlock);
        //    VisitBlock(tryCatchStatement.CatchBlock);
        //}

        //public virtual void VisitTryFinallyStatement(TryFinallyStatement tryFinallyStatement) {
        //    VisitBlock(tryFinallyStatement.TryBlock);
        //    VisitBlock(tryFinallyStatement.FinallyBlock);
        //}

        //public void VisitUndefinedExpression(UndefinedExpression undefinedExpression) {
        //    ReturnValue = Variant.Undefined;
        //}

        //public void VisitUnaryExpression(UnaryExpression unaryExpression) {

        //    VisitExpression(unaryExpression.Expression);

        //    switch (unaryExpression.Operator) {
        //        case TokenType.Not:
        //            ReturnValue = ReturnValue.ToBoolean() ? Variant.False : Variant.True;
        //            break;

        //        case TokenType.Delete:
        //        case TokenType.Void:
        //            ReturnValue = Variant.Undefined;
        //            break;

        //        case TokenType.Typeof:
        //            switch (ReturnValue.Type) {
        //                case "String":
        //                    ReturnValue = new Variant() { Type = "String", Value = "string" };
        //                    break;
        //                case "Number":
        //                    ReturnValue = new Variant() { Type = "String", Value = "number" };
        //                    break;
        //                case "Undefined":
        //                    ReturnValue = new Variant() { Type = "String", Value = "undefined" };
        //                    break;
        //                case "Boolean":
        //                    ReturnValue = new Variant() { Type = "String", Value = "boolean" };
        //                    break;
        //                case "Function":
        //                    ReturnValue = new Variant() { Type = "String", Value = "function" };
        //                    break;
        //                default:
        //                    ReturnValue = new Variant() { Type = "String", Value = "object" };
        //                    break;
        //            }
        //            break;

        //        case TokenType.BitNot:
        //            if (ReturnValue.Value != null) {
        //                ReturnValue = new Variant() { Type = "Number", Value = ~ReturnValue.ToInteger() };
        //            } else {
        //                ReturnValue = new Variant() { Type = "Number" };
        //            }
        //            break;

        //        case TokenType.Add:
        //            if (ReturnValue.Value != null) {
        //                ReturnValue = new Variant() { Type = "Number", Value = ReturnValue.ToNumber() };
        //            } else {
        //                ReturnValue = new Variant() { Type = "Number" };
        //            }
        //            break;

        //        case TokenType.Sub:
        //            if (ReturnValue.Value != null) {
        //                ReturnValue = new Variant() { Type = "Number", Value = -ReturnValue.ToNumber() };
        //            } else {
        //                ReturnValue = new Variant() { Type = "Number" };
        //            }
        //            break;

        //        default:
        //            ReturnValue = new Variant() { Type = "Number" };
        //            break;


        //    }

        //}

        //public void VisitVariableDeclaration(VariableDeclaration variableDeclaration) {
        //    CurrentScope.CurrentStatement = variableDeclaration;
        //    Assign(CurrentScope, variableDeclaration.Name.Name, variableDeclaration, variableDeclaration.Initialiser ?? new UndefinedExpression(variableDeclaration.StartLocation));

        //}

        //public void VisitVariableStatement(VariableStatement variableStatement) {
        //    foreach (VariableDeclaration vd in variableStatement.Variables)
        //        VisitVariableDeclaration(vd);
        //}

        //public virtual void VisitWhileStatement(WhileStatement whileStatement) {
        //    VisitExpression(whileStatement.Condition);
        //    VisitStatement(whileStatement.Body);
        //}

        //public virtual void VisitWithStatement(WithStatement withStatement) {
        //    VisitStatement(withStatement.Body);
        //}

        //public void VisitAdditiveExpression(AdditiveExpression additiveExpression) {
        //    VisitExpression(additiveExpression.Left);
        //    Variant left = ReturnValue;

        //    VisitExpression(additiveExpression.Right);
        //    Variant right = ReturnValue;

        //    // 如果是加， 且有一个类型是 String ，则返回 String 。
        //    if (additiveExpression.Operator == TokenType.Add && (!Variant.IsNumberic(left.Type) || !Variant.IsNumberic(right.Type))) {
        //        ReturnValue = new Variant() { Type = "String", Value = left.Value != null && right.Value != null ? String.Concat(left.Value, right.Value) : null };
        //    } else {
        //        ReturnValue = new Variant() { Type = "Number" };
        //        if (left.Value != null && right.Value != null) {
        //            switch (additiveExpression.Operator) {
        //                case TokenType.Add:
        //                    ReturnValue.Value = left.ToNumber() + right.ToNumber();
        //                    break;
        //                case TokenType.Sub:
        //                    ReturnValue.Value = left.ToNumber() - right.ToNumber();
        //                    break;
        //            }
        //        }
        //    }
        //}

        //public void VisitMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression) {
        //    VisitExpression(multiplicativeExpression.Left);
        //    Variant left = ReturnValue;

        //    VisitExpression(multiplicativeExpression.Right);
        //    Variant right = ReturnValue;
        //    ReturnValue = new Variant() { Type = "Number" };
        //    if (left.Value != null && right.Value != null) {
        //        switch (multiplicativeExpression.Operator) {
        //            case TokenType.Mul:
        //                ReturnValue.Value = left.ToNumber() * right.ToNumber();
        //                break;
        //            case TokenType.Div:
        //                ReturnValue.Value = left.ToNumber() / right.ToNumber();
        //                break;
        //        }
        //    }
        //}

        //public void VisitShiftExpression(ShiftExpression shiftExpression) {
        //    VisitExpression(shiftExpression.Left);
        //    Variant left = ReturnValue;

        //    VisitExpression(shiftExpression.Right);
        //    Variant right = ReturnValue;

        //    ReturnValue = new Variant() { Type = "Number" };
        //    if (left.Value != null && right.Value != null) {
        //        switch (shiftExpression.Operator) {
        //            case TokenType.Shl:
        //                ReturnValue.Value = left.ToInteger() << right.ToInteger();
        //                break;
        //            case TokenType.Shr:
        //                ReturnValue.Value = left.ToInteger() >> right.ToInteger();
        //                break;
        //        }
        //    }
        //}

        //public void VisitRelationalExpression(RelationalExpression relationalExpression) {
        //    VisitExpression(relationalExpression.Left);
        //    Variant left = ReturnValue;

        //    VisitExpression(relationalExpression.Right);
        //    Variant right = ReturnValue;

        //    ReturnValue = new Variant() { Type = "Boolean" };
        //    if (left.Value != null && right.Value != null) {
        //        if (Variant.IsNumberic(left.Type) && Variant.IsNumberic(right.Type)) {
        //            switch (relationalExpression.Operator) {
        //                case TokenType.Gt:
        //                    ReturnValue.Value = left.ToNumber() > right.ToNumber();
        //                    break;
        //                case TokenType.Gte:
        //                    ReturnValue.Value = left.ToNumber() >= right.ToNumber();
        //                    break;
        //                case TokenType.Lt:
        //                    ReturnValue.Value = left.ToNumber() < right.ToNumber();
        //                    break;
        //                case TokenType.Lte:
        //                    ReturnValue.Value = left.ToNumber() <= right.ToNumber();
        //                    break;
        //            }
        //        } else {
        //            switch (relationalExpression.Operator) {
        //                case TokenType.Gt:
        //                    ReturnValue.Value = String.CompareOrdinal(left.Value.ToString(), right.Value.ToString()) > 0;
        //                    break;
        //                case TokenType.Gte:
        //                    ReturnValue.Value = String.CompareOrdinal(left.Value.ToString(), right.Value.ToString()) >= 0;
        //                    break;
        //                case TokenType.Lt:
        //                    ReturnValue.Value = String.CompareOrdinal(left.Value.ToString(), right.Value.ToString()) < 0;
        //                    break;
        //                case TokenType.Lte:
        //                    ReturnValue.Value = String.CompareOrdinal(left.Value.ToString(), right.Value.ToString()) <= 0;
        //                    break;
        //            }
        //        }
        //    }


        //}

        //public void VisitEqualityExpression(EqualityExpression equalityExpression) {
        //    VisitExpression(equalityExpression.Left);
        //    Variant left = ReturnValue;

        //    VisitExpression(equalityExpression.Right);
        //    Variant right = ReturnValue;

        //    ReturnValue = new Variant() { Type = "Boolean" };
        //    if (left.Value != null && right.Value != null) {
        //        switch (equalityExpression.Operator) {
        //            case TokenType.Eq:
        //                ReturnValue.Value = left == right;
        //                break;
        //            case TokenType.EqStrict:
        //                ReturnValue.Value = left == right && left.Type == right.Type;
        //                break;
        //            case TokenType.Ne:
        //                ReturnValue.Value = left != right;
        //                break;
        //            case TokenType.NeStrict:
        //                ReturnValue.Value = left != right || left.Type != right.Type;
        //                break;
        //        }
        //    }
        //}

        //public void VisitBitwiseExpression(BitwiseExpression bitwiseExpression) {
        //    VisitExpression(bitwiseExpression.Left);
        //    Variant left = ReturnValue;

        //    VisitExpression(bitwiseExpression.Right);
        //    Variant right = ReturnValue;

        //    ReturnValue = new Variant() { Type = "Number" };

        //    if (left.Value != null && right.Value != null) {

        //        switch (bitwiseExpression.Operator) {
        //            case TokenType.BitOr:
        //                ReturnValue.Value = left.ToInteger() | right.ToInteger();
        //                break;
        //            case TokenType.BitXor:
        //                ReturnValue.Value = left.ToInteger() ^ right.ToInteger();
        //                break;
        //            case TokenType.BitAnd:
        //                ReturnValue.Value = left.ToInteger() & right.ToInteger();
        //                break;
        //        }
        //    }
        //}

        //public void VisitLogicalExpression(LogicalExpression logicalExpression) {
        //    if (logicalExpression.Operator == TokenType.And) {
        //        Variant lastValue = _lastValue;
        //        VisitExpression(logicalExpression.Left);
        //        Variant left = ReturnValue;

        //        _lastValue = lastValue;
        //        VisitExpression(logicalExpression.Right);
        //        Variant right = ReturnValue;

        //        _lastValue = null;

        //        // 如果无法判断，默认作为 true && 
        //        if (left.Value == null || !left.ToBoolean()) {
        //            ReturnValue = right;
        //        }
        //    } else if (logicalExpression.Operator == TokenType.Or) {
        //        Variant lastValue = _lastValue;
        //        VisitExpression(logicalExpression.Left);

        //        _lastValue = lastValue;
        //        Variant left = ReturnValue;
        //        VisitExpression(logicalExpression.Right);
        //        Variant right = ReturnValue;

        //        _lastValue = null;

        //        // 如果无法判断，默认作为 false ||
        //        if (left.Value != null && left.ToBoolean()) {
        //            ReturnValue = left;
        //        }
        //    } else {
        //        Variant lastValue = _lastValue;
        //        VisitExpression(logicalExpression.Left);
        //        _lastValue = lastValue;
        //        VisitExpression(logicalExpression.Right);

        //        _lastValue = null;
        //    }
        //}

        //public void VisitCommaExpression(CommaExpression commaExpression) {
        //    VisitExpression(commaExpression.Left);
        //    VisitExpression(commaExpression.Right);
        //}

        //public void VisitUserDefinedOperatorExpression(UserDefinedOperatorExpression userDefinedOperatorExpression) {
        //    VisitExpression(userDefinedOperatorExpression.Left);
        //    VisitExpression(userDefinedOperatorExpression.Right);
        //}

        //#endregion

        #region IAstVisitor 成员

        public void VisitAdditiveExpression(AdditiveExpression additiveExpression) {
            throw new NotImplementedException();
        }

        public void VisitArrayLiteral(ArrayLiteral arrayLiteral) {
            throw new NotImplementedException();
        }

        public void VisitAssignmentExpression(AssignmentExpression assignmentExpression) {
            throw new NotImplementedException();
        }

        public void VisitBitwiseExpression(BitwiseExpression bitwiseExpression) {
            throw new NotImplementedException();
        }

        public void VisitBlock(Block blockStatement) {
            throw new NotImplementedException();
        }

        public void VisitBreakStatement(BreakStatement breakStatement) {
            throw new NotImplementedException();
        }

        public void VisitCallNative(CallNative callNative) {
            throw new NotImplementedException();
        }

        public void VisitCaseClause(CaseClause caseLabel) {
            throw new NotImplementedException();
        }

        public void VisitCommaExpression(CommaExpression commaExpression) {
            throw new NotImplementedException();
        }

        public void VisitConditionalExpression(ConditionalExpression conditionalExpression) {
            throw new NotImplementedException();
        }

        public void VisitConstStatement(ConstStatement constStatement) {
            throw new NotImplementedException();
        }

        public void VisitContinueStatement(ContinueStatement continueStatement) {
            throw new NotImplementedException();
        }

        public void VisitDebuggerStatement(DebuggerStatement debuggerStatement) {
            throw new NotImplementedException();
        }

        public void VisitDoWhileStatement(DoWhileStatement doWhileStatement) {
            throw new NotImplementedException();
        }

        public void VisitEmptyStatement(EmptyStatement emptyStatement) {
            throw new NotImplementedException();
        }

        public void VisitEqualityExpression(EqualityExpression equalityExpression) {
            throw new NotImplementedException();
        }

        public void VisitExpressionStatement(ExpressionStatement expressionStatement) {
            throw new NotImplementedException();
        }

        public void VisitFalseLiteral(FalseLiteral falseLiteral) {
            throw new NotImplementedException();
        }

        public void VisitForInStatement(ForInStatement forinStatement) {
            throw new NotImplementedException();
        }

        public void VisitForStatement(ForStatement forStatement) {
            throw new NotImplementedException();
        }

        public void VisitFunctionCallExpression(FunctionCallExpression functionCallExpression) {
            throw new NotImplementedException();
        }

        public void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration) {
            throw new NotImplementedException();
        }

        public void VisitFunctionExpression(FunctionExpression functionExpression) {
            throw new NotImplementedException();
        }

        public void VisitIdentifier(Identifier identifier) {
            throw new NotImplementedException();
        }

        public void VisitIfStatement(IfStatement ifStatement) {
            throw new NotImplementedException();
        }

        public void VisitIncrementOperation(IncrementOperation incrementOperation) {
            throw new NotImplementedException();
        }

        public void VisitIndexCallExpression(IndexCallExpression indexCallExpression) {
            throw new NotImplementedException();
        }

        public void VisitLabelledStatement(LabelledStatement labelledStatement) {
            throw new NotImplementedException();
        }

        public void VisitLogicalExpression(LogicalExpression logicalExpression) {
            throw new NotImplementedException();
        }

        public void VisitMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression) {
            throw new NotImplementedException();
        }

        public void VisitNewExpression(NewExpression newExpression) {
            throw new NotImplementedException();
        }

        public void VisitNullLiteral(NullLiteral nullLiteral) {
            throw new NotImplementedException();
        }

        public void VisitNumberLiteral(NumberLiteral numberLiteral) {
            throw new NotImplementedException();
        }

        public void VisitObjectLiteral(ObjectLiteral objectLiteral) {
            throw new NotImplementedException();
        }

        public void VisitParamedExpression(ParamedExpression paramedExpression) {
            throw new NotImplementedException();
        }

        public void VisitPostfixExpression(PostfixExpression countOperation) {
            throw new NotImplementedException();
        }

        public void VisitProperty(ObjectLiteral.Property property) {
            throw new NotImplementedException();
        }

        public void VisitPropertyCallExpression(PropertyCallExpression propertyCallExpression) {
            throw new NotImplementedException();
        }

        public void VisitRegExpLiteral(RegExpLiteral regExpLiteral) {
            throw new NotImplementedException();
        }

        public void VisitRelationalExpression(RelationalExpression relationalExpression) {
            throw new NotImplementedException();
        }

        public void VisitReturnStatement(ReturnStatement returnStatement) {
            throw new NotImplementedException();
        }

        public void VisitSemicolon(Semicolon semicolon) {
            throw new NotImplementedException();
        }

        public void VisitShiftExpression(ShiftExpression shiftExpression) {
            throw new NotImplementedException();
        }

        public void VisitStringLiteral(StringLiteral stringLiteral) {
            throw new NotImplementedException();
        }

        public void VisitSwitchStatement(SwitchStatement switchStatement) {
            throw new NotImplementedException();
        }

        public void VisitThisLiteral(ThisLiteral thisLiteral) {
            throw new NotImplementedException();
        }

        public void VisitThrowStatement(ThrowStatement throwStatement) {
            throw new NotImplementedException();
        }

        public void VisitTrueLiteral(TrueLiteral trueLiteral) {
            throw new NotImplementedException();
        }

        public void VisitTryCatchFinallyStatement(TryCatchFinallyStatement tryCatchFinallyStatement) {
            throw new NotImplementedException();
        }

        public void VisitTryCatchStatement(TryCatchStatement tryCatchStatement) {
            throw new NotImplementedException();
        }

        public void VisitTryFinallyStatement(TryFinallyStatement tryFinallyStatement) {
            throw new NotImplementedException();
        }

        public void VisitUnaryExpression(UnaryExpression unaryExpression) {
            throw new NotImplementedException();
        }

        public void VisitUndefinedExpression(UndefinedExpression undefinedExpression) {
            throw new NotImplementedException();
        }

        public void VisitUserDefinedOperatorExpression(UserDefinedOperatorExpression userDefinedOperatorExpression) {
            throw new NotImplementedException();
        }

        public void VisitVariableDeclaration(VariableDeclaration variableDeclaration) {
            throw new NotImplementedException();
        }

        public void VisitVariableStatement(VariableStatement variableStatement) {
            throw new NotImplementedException();
        }

        public void VisitWhileStatement(WhileStatement whileStatement) {
            throw new NotImplementedException();
        }

        public void VisitWithStatement(WithStatement withStatement) {
            throw new NotImplementedException();
        }

        #endregion
    }

}
