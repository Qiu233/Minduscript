using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Wait : Instruction
	{
		public Wait(ParamEvaluable time) : base(OpCode.WAIT, time)
		{
		}
	}
}
