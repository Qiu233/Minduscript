using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class LookUp : Instruction
	{
		public LookUp(GameElements mode, ParamEvaluable var, ParamEvaluable ID) : base(OpCode.LOOKUP, mode, var, ID)
		{
		}
	}
}
