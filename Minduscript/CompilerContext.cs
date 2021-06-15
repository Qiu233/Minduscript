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
	/// <summary>
	/// Keeping information of global compiler
	/// </summary>
	public class CompilerContext
	{
		/// <summary>
		/// full file path to assembly
		/// </summary>
		public Dictionary<string, ILAssembly> AssembledILAssemblies
		{
			get;
		}
		public CompilingOptions Options
		{
			get;
		}
		public List<ILAssembly> ILAssemblies
		{
			get;
		}
		public CompilingInfoHandler CompilingInfoHandler
		{
			get;
		}
		public TextWriter StdOut
		{
			get;
			set;
		}
		private int LogLevel
		{
			get;
			set;
		}
		public void NewLogLevel()
		{
			LogLevel++;
		}
		public void EndLogLevel()
		{
			LogLevel--;
		}
		public CompilerContext(CompilingOptions options)
		{
			StdOut = Console.Out;
			Options = options;
			AssembledILAssemblies = new Dictionary<string, ILAssembly>();
			ILAssemblies = new List<ILAssembly>();
			CompilingInfoHandler = new CompilingInfoHandler();
			CompilingInfoHandler.OnLog += CompilingInfoHandler_OnLog;
		}


		public void Log(string msg)
		{
			if (!Options.Log)
				return;
			CompilingInfoHandler.Log(msg);
		}

		private void CompilingInfoHandler_OnLog(GeneralInfo info)
		{
			StdOut.Write($"({LogLevel})");
			StdOut.WriteLine(info.AssembledMessage);
		}

		public ILAssembly GetAssembly(string inputFile)
		{
			NewLogLevel();
			Log($"Compiling file:({inputFile})");
			string index = Path.GetFullPath(inputFile);
			if (!AssembledILAssemblies.ContainsKey(index))
			{
				var asm = new Parser(File.ReadAllText(inputFile), inputFile, this).CompileToILAssembly();
				if (CompilingInfoHandler.ErrorsCount > 0)
				{
					CompilingInfoHandler.ThrowContextError("Compiling", $"Compilation stopping due to errors");
					throw new CompilerStoppingException("Critical error occured during compilation");
				}
				string name = asm.Name;

				//optimizations
				foreach (var func in asm.Functions)
				{
					Optimization.IL.AssembledOptimizer.
						GetDefaultOptimizer(new Optimization.IL.OptimizerContext(func, this)).
						Run();
				}
				AssembledILAssemblies[index] = asm;
				ILAssemblies.Add(asm);

				if (Options.OutputIL)
				{
					string ilFile = $"{name}.il";
					using StreamWriter sw = new StreamWriter(File.Create(ilFile));
					asm.Output(sw);
					Log($"ILFile generated:({ilFile})");
				}
			}
			EndLogLevel();
			return AssembledILAssemblies[index];
		}
	}
}
