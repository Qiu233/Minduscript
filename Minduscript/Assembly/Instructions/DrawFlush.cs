using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class DrawFlush : Instruction
	{
		public DrawFlush(ParamEvaluable screen) : base(OpCode.DRAWFLUSH, screen)
		{

		}
	}
}
