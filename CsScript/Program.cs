using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace CsScript
{
    public class CompileUnit
    {
        public static CompilerParameters BuildCompileParameters(bool generateInMemory, bool generateExecutable, string outputAssembly)
        {
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateInMemory = generateInMemory;
            compilerParameters.GenerateExecutable = generateExecutable;
            compilerParameters.IncludeDebugInformation = true;
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            if (outputAssembly != null)
            {
                compilerParameters.OutputAssembly = outputAssembly;
            }
            return compilerParameters;
        }

        public static CompilerResults Compile(string source, CompilerParameters compilerParameters)
        {
            CSharpCodeProvider CScodeProvider = new CSharpCodeProvider();
            ICodeCompiler icodeCompiler = CScodeProvider.CreateCompiler();
            CompilerResults compilerResults = icodeCompiler.CompileAssemblyFromSource(compilerParameters, source);
            return compilerResults;
        }

        public static void ParseSource(ref string source, out string using_text, out string[] references, out string functions)
        {
            string new_source = "";
            List<string> references_list = new List<string>();
            using_text = ""; references = new string[0]; functions = "";
            string[] source_lines = source.Split('\n');
            for (int i = 0; i < source_lines.Length; ++i)
            {
                int offset = 0;
                string s = source_lines[i].Trim();
                if (string.IsNullOrEmpty(s)) continue;
                if (s.StartsWith("#"))
                {
                    if (s.EndsWith(";")) s = s.Substring(0, s.Length - 1);
                    switch (s.Split(' ')[0])
                    {
                        case "#reference":
                            references_list.Add(s.Substring("#reference ".Length));
                            continue;
                        case "#using":
                            using_text += "using " + s.Substring("#using ".Length) + ";" + Environment.NewLine;
                            continue;
                        case "#include":

                            continue;
                        case "#function":
                            int function_braces = 0;
                            do
                            {
                                ++i;
                                int j;
                                for (j = 0; j < source_lines[i].Length; ++j)
                                {
                                    if (source_lines[i][j] == '{') ++function_braces;
                                    else if (source_lines[i][j] == '}')
                                    {
                                        --function_braces;
                                        if (function_braces == 0) { ++j; break; }
                                    }
                                }
                                functions += source_lines[i].Substring(0, j);
                                offset = j + 1;
                            } while (function_braces > 0);
                            s = source_lines[i];
                            break;
                        default:
                            break;
                    }
                }

                if (offset < s.Length) s = s.Substring(offset); else s = "";
                new_source += s + Environment.NewLine;
            }

            references = references_list.ToArray();
            source = new_source;
        }        
    }

    static class Program
    {
        const string template = "// !CsScript Generated, do not remove\r\n";
        const string default_using = "using System;\r\nusing System.IO;\r\nusing System.Text;\r\nusing System.Text.RegularExpressions;\r\n\r\n";
        const string default_main = "namespace ScriptRunner {\r\npublic static partial class Program {\r\npublic static void Main(string[] args) {\r\n #### \r\n}\r\n}\r\n}\r\n";
        const string default_functions = "namespace ScriptRunner {\r\npublic static partial class Program {\r\n #### \r\n}\r\n}\r\n";

        static void Main(string[] argv)
        {

            #region PARSE COMMAND LINE ARGS
            string filename = null;
            bool create = false, fullsource = false;

            foreach (string s in argv)
                if (s.StartsWith("-"))
                    switch (s)
                    {
                        case "-create":
                        case "-c":
                            create = true;
                            break;
                        case "-full":
                        case "-f":
                            fullsource = true;
                            break;
                    }
                else
                    if (filename == null) filename = s;
            #endregion

            if (create)
            {
                System.IO.File.WriteAllText(filename, template);
                return;
            }

            if (!System.IO.File.Exists(filename))
            {
                Console.WriteLine("Error 2: File not found.");
                return;
            }

            string source = System.IO.File.ReadAllText(filename);

            string using_text = "", functions = ""; string[] references = null;
           
            if (!fullsource && !source.StartsWith(template))
            {
                CompileUnit.ParseSource(ref source, out using_text, out references, out functions); 
                source = default_using + using_text + Environment.NewLine + default_main.Replace("####", source);
                source += default_functions.Replace("####", functions);
            }

            CompilerParameters compilerParameters = CompileUnit.BuildCompileParameters(true, false, null);
            foreach(string refs in references)
                compilerParameters.ReferencedAssemblies.Add(refs);    

            CompilerResults compilerResults = CompileUnit.Compile(source, compilerParameters);
            string[] child_argv = new string[Math.Max(0, argv.Length - 1)];
            for (int i = 1; i < argv.Length; ++i)
                child_argv[i - 1] = argv[i];

            if (compilerResults.Errors.Count == 0)
            {
                ///如果错误数为0则进行调用
                Assembly asm = compilerResults.CompiledAssembly;
                Type type = asm.GetTypes()[0];
                MethodInfo methodInfo = type.GetMethod("Main");
                //object obj = System.Activator.CreateInstance(type);
                try
                {
                    object result = methodInfo.Invoke(null, new object[]{child_argv});
                    //type.InvokeMember("Main", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, argv);
                    //Console.WriteLine(result.ToString());
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return;
                }
            }
            else
            {
                //如果出错则返回错误文本
                Console.WriteLine(source);

                string errorMessage = "";
                foreach (CompilerError compilerError in compilerResults.Errors)
                {
                    errorMessage += compilerError/*.ErrorText*/ + System.Environment.NewLine;
                }
                Console.WriteLine(errorMessage);
#if DEBUG
            Console.ReadLine();
#endif               
                return;
            }


        }
    }
}
