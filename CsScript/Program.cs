using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.Text.RegularExpressions;
using System;

namespace CsScript
{
    public class CompileUnit
    {
        public static CompilerResults Compile(string source, bool generateInMemory, bool generateExecutable, string outputAssembly)
        {
            CSharpCodeProvider CScodeProvider = new CSharpCodeProvider();
            ICodeCompiler icodeCompiler = CScodeProvider.CreateCompiler();
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
            CompilerResults compilerResults = icodeCompiler.CompileAssemblyFromSource(compilerParameters, source);
            return compilerResults;
        }
    }

    static class Program
    {
        const string template = "// !CsScript Generated Templete, do not remove\r\nusing System;\r\nusing System.IO;\r\nusing System.Text;\r\nusing System.Text.RegularExpressions;\r\n\r\n" +
                    "namespace ScriptRunner {\r\npublic static class Program {\r\npublic static void Main(string[] args) {\r\n #### \r\n}\r\n}\r\n}\r\n";

        static void Main(string[] argv)
        {
            string filename = null;
            bool create = false;

            foreach (string s in argv)
                if (s.StartsWith("-"))
                    switch (s)
                    {
                        case "-create":
                        case "-c":
                            create = true;
                            break;
                    }
                else
                    if (filename == null) filename = s;

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

            if (!source.StartsWith("// !CsScript Generated Templete"))
                source = template.Replace("####", source);
            
            CompilerResults compilerResults = CompileUnit.Compile(source, true, false, null);
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
                ///如果出错则返回错误文本
                string errorMessage = "";
                foreach (CompilerError compilerError in compilerResults.Errors)
                {
                    errorMessage += compilerError.ErrorText + System.Environment.NewLine;
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
