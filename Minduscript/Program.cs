using System;
using System.IO;
using Minduscript.Assembly;
using Minduscript.Assembly.Instructions;
using Minduscript.CompilingInfo;
using Minduscript.Lex;
using Minduscript.Parse;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Minduscript
{
	class Program
	{
		static void Main(string[] args)
		{
			CompilingOptions options = null;
			CommandLine.Parser.Default.ParseArguments<CompilingOptions>(args).WithParsed(o => { options = o; }).WithNotParsed(es =>
			{
				Console.WriteLine("Critical error occured, compilation stopping");
				Environment.Exit(1);
			});
			if (!File.Exists(options.Input))
			{
				Console.WriteLine($"Input file not found:{options.Input}, compilation stopping");
				Environment.Exit(1);
			}
			Compiler compiler = new Compiler(options);
			try
			{
				compiler.Compile();
			}
			catch (CompilerStoppingException)
			{
			}
		}
	}
}
