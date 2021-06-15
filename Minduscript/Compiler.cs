using Minduscript.Assembly;
using Minduscript.Assembly.Instructions;
using Minduscript.CompilingInfo;
using Minduscript.IL;
using Minduscript.Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript
{
	public class Compiler
	{
		public CompilerContext Context
		{
			get;
		}
		public Compiler(CompilingOptions options)
		{
			Context = new CompilerContext(options);
		}
		public void Compile()
		{
			Context.Log("Compiling");
			var asm = Context.GetAssembly(Context.Options.Input);
			var entry = asm.Functions.Where(t => t.Name == "main");
			if (entry.Count() == 0)
			{
				Context.CompilingInfoHandler.ThrowContextError("Compiling", "Found no main function, compilation stopping.");
			}
			else
			{
				string name = $"{asm.Name}.asm";
				using var file = File.Open(Context.Options.Output ?? name, FileMode.Create);
				NativeAssemblyGenerator.Generate(asm, new StreamWriter(file));
				Context.Log($"Compialtion completed, asm file generated:{name}");
			}
		}
	}
}
