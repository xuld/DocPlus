//using System;
//using System.Collections.Generic;
//using System.Text;
//using Py.Logging;
//using Py.Core;
//using System.IO;

//namespace DocPlus.Parser.Javascript { 
//    public static class DocParseHelper {



//        static void Show2(INativeObject obj, string globals, List<INativeObject> processed) {

//            if (processed.Contains(obj))
//                return;

//            processed.Add(obj);

//            foreach (Property value in obj) {

//                string name = globals + "." + value.Name;

//                if (value.Enumerable) {
//                    Logger.Write("{0} = {1}", name, value.Value.Type);
//                }

//                Show2(value.Value, name, processed);
//            }
//        }

//        static void Show(INativeObject obj, string globals, List<INativeObject> processed) {

//            if (processed.Contains(obj)) {

//                if (obj.Comment == null || !obj.Comment.System) {
//                    foreach (Property value in obj) {
//                        Console.Write(globals);
//                        Console.Write(".");
//                        Console.Write(value.Name);
//                        Console.WriteLine(" = [略]    ");
//                    }

//                }

//                return;
//            }

//            processed.Add(obj);

//            foreach (Property value in obj) {

//                string name = globals + "." + value.Name;

//                if (value.Enumerable && value.Comment != null && !value.Comment.Ignore && !value.Comment.System) {
//                    Console.ForegroundColor = ConsoleColor.Green;
//                    Console.Write(name);
//                    Console.ResetColor();



//                    Console.Write(" = ");
//                    Console.Write(value.Comment.ToString());
//                    Console.WriteLine();
//                } else if (value.Comment == null || !value.Comment.System) {

//                    //   if (false) {
//                    Console.ForegroundColor = ConsoleColor.Yellow;
//                    Console.Write(name);
//                    Console.ResetColor();



//                    Console.Write(" = ");
//                    Console.Write(value.Value.Type);
//                    Console.WriteLine();

//                    // }
//                }

//                if ((value.Comment != null && (value.Comment.Ignore || value.Comment.System) && value.Value is NativeActivition) || value.Name == "constructor")
//                    continue;

//                Show(value.Value, name, processed);
//            }
//        }

//        static void ShowClear(INativeObject obj, string globals, List<INativeObject> processed) {

//            if (processed.Contains(obj)) {

//                //if (obj.Comment == null || !obj.Comment.System) {
//                //    foreach (Property value in obj) {
//                //        Console.Write(globals);
//                //        Console.Write(".");
//                //        Console.Write(value.Name);
//                //        Console.WriteLine(" = [略]    ");
//                //    }

//                //}

//                return;
//            }

//            processed.Add(obj);

//            foreach (Property value in obj) {

//                string name = globals + "." + value.Name;

//                if (value.IsNormal && !value.Internal)  {
//                    Console.ForegroundColor = ConsoleColor.Green;
//                    Console.Write(name);
//                    Console.ResetColor();



//                    Console.Write(" = ");
//                    Console.Write(value.Comment.ToString());
//                    Console.WriteLine();
//                } else{
                   
//                }

//                if (!String.IsNullOrEmpty(value.Name) && value.Name[0] == '@')
//                    continue;

//                if ((value.Comment != null && (value.Comment.Ignore || value.Comment.System) && value.Value is NativeActivition) || value.Name == "constructor") {
//                    Console.ForegroundColor = ConsoleColor.Yellow;
//                    ShowClear(value.Value, name, processed);
//                    Console.ResetColor();
//                    continue;
//                }

//                ShowClear(value.Value, name, processed);
//            }
//        }

//        public static void ShowClear(INativeObject obj) {

//            Logger.Start("变量");

//            ShowClear(obj, String.Empty, new List<INativeObject>());

//            Logger.End();
//        }

//        public static void Show(INativeObject obj) {

//            Logger.Start("变量");

//            Show(obj, String.Empty, new List<INativeObject>());

//            Logger.End();
//        }


//        public static void Show(Document doc, string filePath = "doc3.txt") {



//            string AA = FileHelper.GetFullPath(filePath);

//            FileStream s = new FileStream(AA, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

//            s.SetLength(0);

//            using (Writer w = new Writer(s)) {

//                doc.Write(w);

//            }

//          Console.Write(FileHelper.Read(AA, Encoding.Default));



//        }


//        public static Document Parse(string code) {






//            JavaScriptDocParser parser = new JavaScriptDocParser();

//            parser.ParseString(code);


//            return parser.Document;

//        }

//    }
//}
