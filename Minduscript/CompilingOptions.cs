using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Minduscript
{
	/// <summary>
	/// compiler's options
	/// </summary>
	public class CompilingOptions
	{
		[Option('i', "input", Required = true, HelpText = "File to compile")]
		public string Input
		{
			get;
			set;
		}
		[Option('o', "output", Required = false, HelpText = "File to output the result")]
		public string Output
		{
			get;
			set;
		}
		[Option("il", Default = false, Required = false, HelpText = "Whether to generate il files")]
		public bool OutputIL
		{
			get;
			set;
		}
		[Option('l', "log", Default = false, Required = false, HelpText = "Whether to log details")]
		public bool Log
		{
			get;
			set;
		}
	}
}
