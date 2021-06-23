using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class UBind : Instruction
	{
		public UBind(ParamEvaluable unit) : base(OpCode.UBIND, unit)
		{

		}
	}
}
