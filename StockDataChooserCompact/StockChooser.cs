using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class MyClass
{
	public static void RunSnippet()
	{
		string[] filenames = Directory.GetFiles(@"..\..\Data\Price\");
		Random rand = new Random();
		for (int i = 0; i < 500; ++i) {
			int id = rand.Next(filenames.Length);
			string filename = filenames[id];
			File.Copy(filename, @"..\..\Data\TEMP\" + filename.Substring(filename.LastIndexOf('\\') + 1), true);
		}
	}
	
	public static void RunSnippet2()
	{
		string lines = "";
		string input = "";
		do {
			input = Console.ReadLine();
			lines += input + Environment.NewLine;
		} while (input != "");
		
		Regex reg = new Regex(@"\d{6}|\\item");
		char item = (char)('a');
		
		foreach(Match m in reg.Matches(lines)) {
			if (m.ToString().Equals(@"\item")) {
				item = (char)(item + 1);
				continue;
			}
			string filename = m.ToString() + ".txt";
			
			File.Copy(@"..\..\Data\Price\" + filename, @"..\..\Data\TEMP\" + item + "-" + filename, true);
		}
		
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
		finally
		{
			Console.Write("Press any key to continue...");
			Console.ReadKey();
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