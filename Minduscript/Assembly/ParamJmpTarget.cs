using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly
{
	public class ParamJmpTarget : Param
	{
		public Instruction Target
		{
			get;
		}
		public ParamJmpTarget(Instruction target)
		{
			Target = target;
		}

		public override string GetASMCode()
		{
			throw new AssemblerException("Undetermined target index");
		}
	}
}
