using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Jump : Instruction
	{
		public Jump(ParamJmpTarget target, Conditions condition, ParamEvaluable v1, ParamEvaluable v2) : base(OpCode.JUMP, target, condition, v1, v2)
		{

		}
		public Jump(ParamConstant target, Conditions condition, ParamEvaluable v1, ParamEvaluable v2) : base(OpCode.JUMP, target, condition, v1, v2)
		{

		}
	}
}
