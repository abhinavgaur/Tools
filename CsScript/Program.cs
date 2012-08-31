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
        public const string template = "// !CsScript Generated, do not remove\r\n";
        public const string default_using = "using System;\r\nusing System.IO;\r\nusing System.Text;\r\nusing System.Text.RegularExpressions;\r\nusing System.Collections.Generic;\r\n";
        public const string default_main = "namespace ScriptRunner {\r\npublic static partial class Program {\r\npublic static void Main(string[] args) {\r\n #### \r\n}\r\n}\r\n}\r\n";
        public const string default_functions = "namespace ScriptRunner {\r\npublic static partial class Program {\r\n #### \r\n}\r\n}\r\n";

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
            string new_source = "#line 2" + Environment.NewLine;
            List<string> references_list = new List<string>();
            using_text = ""; references = new string[0]; functions = "";
            string[] source_lines = source.Split('\n');
            for (int i = 0; i < source_lines.Length; ++i)
            {
                int offset = 0;
                string s = source_lines[i].Trim();
                if (s.StartsWith("#"))
                {
                    if (s.EndsWith(";")) s = s.Substring(0, s.Length - 1);
                    switch (s.Split(' ')[0])
                    {
                        case "#reference":
                            references_list.Add(s.Substring("#reference ".Length));
                            continue;
                        case "#using":
                            string using_line = "using " + s.Substring("#using ".Length) + ";";
                            if (default_using.Contains(using_line)) continue;
                            using_text += using_line + Environment.NewLine;
                            continue;
                        case "#include":

                            continue;
                        case "#function":
                            functions += "#line " + (i + 2) + Environment.NewLine;
                            for (++i; i < source_lines.Length; ++i)
                                if (source_lines[i].Trim().StartsWith("#endfunction"))
                                    break;
                                else
                                    functions += source_lines[i] + Environment.NewLine;
                            functions += "#line " + (i + 3) + Environment.NewLine;
                            continue;
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
                System.IO.File.WriteAllText(filename, CompileUnit.template);
                return;
            }

            if (!System.IO.File.Exists(filename))
            {
                filename += ".cssc";
                if (!System.IO.File.Exists(filename))
                {
                    Console.WriteLine("Error 2: File not found.");
                    return;
                }
            }

            string source = System.IO.File.ReadAllText(filename);

            string using_text = "", functions = ""; string[] references = null;
           
            if (!fullsource && !source.StartsWith(CompileUnit.template))
            {
                CompileUnit.ParseSource(ref source, out using_text, out references, out functions);
                source = CompileUnit.default_using + using_text + Environment.NewLine + CompileUnit.default_main.Replace("####", source);
                source += CompileUnit.default_functions.Replace("####", functions);
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
                    Console.WriteLine(ex.InnerException);
                    return;
                }
            }
            else
            {
                //如果出错则返回错误文本
                foreach (CompilerError compilerError in compilerResults.Errors)
                {
                    compilerError.FileName = filename;
                    Console.WriteLine(compilerError);
                }    
                return;
            }


        }
    }
}
