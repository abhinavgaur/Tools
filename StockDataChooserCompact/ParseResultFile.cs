using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class MyClass
{
	static Dictionary<string, string> codeToName = new Dictionary<string, string>();
	static StreamWriter sw = new StreamWriter(@"D:\Documents\学校\作业\毕业设计\Data\results22_readable.txt", false, Encoding.Unicode);
	static string codeNameFile = @"D:\Documents\学校\作业\毕业设计\Data\allA_名称.txt",
		inputFile = @"D:\Documents\学校\作业\毕业设计\Data\results22.txt",
		dataDirectory = @"D:\Documents\学校\作业\毕业设计\Data\";
	static string[] classes = new string[50]; static int[] classesCount = new int[50];
	static string[] stocks = new string[1];
	
	public static void print() {	
		for(int i = 0; i < 50; ++i) {
			if (classesCount[i] <= 0) continue;
			string s = classes[i];
			sw.WriteLine("" + i + "(" + classesCount[i] + ")\t" + s);
		}
		for(int i=0;i<50;++i) { classes[i]=""; classesCount[i]=0; }
	}
	
	public static void ParseFile(string[] lines) {
		int state = 0, p = 0;
		foreach(string line in lines) {
			if (line == "") continue;
			if (line.StartsWith("get")) {
				print();
				state = 0; p = 0;
				string set = line.Substring(line.IndexOf("('") + 2); set = set.Substring(0, set.IndexOf('\''));
				string year = line.Substring(line.IndexOf(", '") + 3); year = year.Substring(0, year.IndexOf('\''));
				stocks = Directory.GetFiles(dataDirectory + @"\" + set + @"\data\", "*-*.txt");
				sw.WriteLine("\r\nSET: " + set + "-" + year); continue;
			}
			if (line.EndsWith(":")) {
				print();
				state++; p = 0;
				sw.WriteLine(line); continue;
			}
			int id = 0;
			if (int.TryParse(line, out id)) {
				string code = stocks[p].Substring(stocks[p].LastIndexOf('-') + 1); code = code.Substring(0, code.LastIndexOf('.'));
				if (codeToName.ContainsKey(code)) code += codeToName[code]; else code += "\t";
				classes[id] += code + "\t"; classesCount[id]++;
				p++;
			}
		}
		print();
	}
	
	public static void RunSnippet()
	{
		foreach(string line in File.ReadAllLines(codeNameFile, Encoding.GetEncoding("GBK"))) {
			if (line.IndexOf(',') < 0) continue;
			codeToName.Add(line.Split(',')[0], line.Split(',')[1].Trim());
		}
		
		ParseFile(File.ReadAllLines(inputFile));
		
		sw.Close();
	}
	
	#region Helper methods
	
	public static void Main()
	{
		try
		{
			RunSnippet();
		}
		catch (Exception e)
		{
			string error = string.Format("---\nThe following error occurred while executing the snippet:\n{0}\n---", e.ToString());
			Console.WriteLine(error);
		}
	}

	private static void WL(object text, params object[] args)
	{
		Console.WriteLine(text.ToString(), args);	
	}
	
	private static void RL()
	{
		Console.ReadLine();	
	}
	
	private static void Break() 
	{
		System.Diagnostics.Debugger.Break();
	}

	#endregion
}