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
			var asm = Context.GetAssembly(Context.Options.Input);
			using var file = File.Open(Context.Options.Output ?? $"{asm.Name}.asm", FileMode.Create);
			NativeAssemblyGenerator.Generate(asm, new StreamWriter(file));
		}
	}
}
