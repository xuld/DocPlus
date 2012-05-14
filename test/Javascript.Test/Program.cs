using System;
using System.Collections.Generic;
using System.Text;

namespace DocPlus.Javascript.Test {
    class Program {

//        static void TestCommentParser() {

//            JavaCommentParser jcp = new JavaCommentParser();

//            jcp.Reset();

//            CorePlus.Parser.Javascript.TokenInfo t = new CorePlus.Parser.Javascript.TokenInfo();
//            t.LiteralBuffer.Append(@"
//             *
//             * Summary
//             * @author author.Me
//             * @projectDescription projectDescription a b c
//             * @license lisense a b c
//             * @version 1.0.0.0
//             * @fileOverview fileOverview a b c
//             * @file file a b c
//             * @requires requires a  b c
//             * @config {Type} Name   =   Value
//             * @public
//             * @final
//             * @alias  A.B
//             * @extends A
//             * @extends B
//             * @implements IA
//             * @implements IB
//             * @summary summary
//             * @remark remark
//             * @remark remark2
//             * remark6
//             * @example example {@code {L} <<<code>>>}
//             * @syntax syntax
//             * @category category
//             * @memberOf A
//             * @see see
//             * @seealso seealso
//             * @ignore
//
//             * @since 1.0.0.0
//             * @deprecated 2.0.0.0
//             * @return {Type} Summary
//             * @param {Type} Name Summary
//             * @param {Type} Name = 1 Summary
//             * @param {Type} [Name] Summary
//             * @param {Type} ... Summary {@link#AA} {@p a}
//             * @param Name Summary
//             * @exception {Type} exception
//             * @exception exception
//             * @define {CC} {Boolean}
//             * @define p param
//             * @p {CC} p
//             * @{@}@@@
//             * @*@<@&@/
//             *
//            
//            ");
//            Comment c = jcp.Parse(t);


//            Console.Write(c);

//        }

//        static void ShowGlobal(Variant v) {
//            List<Variant> displayed = new List<Variant>(){v};
//            foreach (var v2 in v) {
//                ShowGlobal(v2.Value, displayed);
//            }
//        }

//        static void ShowGlobal(Variant v, List<Variant> displayed) {
//            if (!displayed.Contains(v)) {
//                displayed.Add(v);

//                Console.ForegroundColor = ConsoleColor.Cyan;
//                Console.Write(v.Name);
//                Console.ForegroundColor = ConsoleColor.Gray;
//                Console.Write("      ");
//                Console.Write(Str.Substring(v.Comment.ToString(), 0, 40));
//                Console.WriteLine();

//                foreach (var v2 in v) {
//                    Console.ForegroundColor = ConsoleColor.Cyan;
//                    Console.Write(v.Name);
//                    Console.Write('.');
//                    ShowGlobal(v2.Value, displayed);
//                }
//            }
//        }

//        static void TestDocAstVistor() {
//            JavaScriptDocParser jdp = new JavaScriptDocParser();
//            jdp.NewDocument();
//            jdp.ParseString(@"
///**
// * @class A
// */
//A = 2;
//
///**
// * @class B
// */
//Class.create(""A.B"", {
//
//  /***/
//  f: 2
//
//});
//
//
///**
// * @class G
// */
//(function(){
//
//  /**
//   * @class W
//   */
//  a = {
//
//  
//      /***/
//     f: 2
//  }
//
//});
//
///**
// *@class H
// */
//apply({
//
//  /***/
//  g: 1
//
//}, {
//
//  /***/
//  e: 5
//
//
//}).call({
//
//   /***/
//  h: 2
//
//});
//
///***/
//g = {
//  
//  /***/
//  e: 3
//
//}
//");
//            ShowGlobal(jdp.Global);
//        }

        static void TestParse() {


        }

        static void TestBuild() {

            Environment.CurrentDirectory = "H:\\测试";

            DocProject proj = new DocProject();
            proj.Items.Add("a.js");
            proj.TargetPath = "a.txt";
            proj.Build();

        }

        static void Main(string[] args) {


            TestBuild();

            Console.Read();

        }
    }
}
