using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILFunction : ILOperand, IILCallable
	{
		public string Name
		{
			get;
			set;
		}
		public ILVariable ReturnValue
		{
			get;
			set;
		}

		public ILOperandCollection<ILInstruction> Instructions
		{
			get;
		}

		public ILOperandCollection<ILVariable> Params
		{
			get;
		}

		public ILFunction(SourcePosition src) : base(src)
		{
			Instructions = new ILOperandCollection<ILInstruction>();
			Params = new ILOperandCollection<ILVariable>();
		}
	}
}
