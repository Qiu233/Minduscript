using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Op : Instruction
	{
		public Op(Operators optr, ParamEvaluable result, ParamEvaluable a, ParamEvaluable b) : base(OpCode.OP, optr, result, a, b)
		{

		}
	}
}
