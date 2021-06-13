using Minduscript.Assembly;
using Minduscript.Assembly.Instructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILAssembly : IILExecutable
	{
		public string File
		{
			get;
		}
		public string Name
		{
			get;
			set;
		}
		public Dictionary<string, ILAssembly> Namespaces
		{
			get;
		}
		public List<ILFunction> Functions
		{
			get;
		}
		public ILOperandCollection<ILInstruction> Instructions
		{
			get;
		}

		public ILAssembly(string file, string name)
		{
			File = file;
			Name = name;
			Namespaces = new Dictionary<string, ILAssembly>();
			Functions = new List<ILFunction>();
			Instructions = new ILOperandCollection<ILInstruction>();
		}


	}
}
