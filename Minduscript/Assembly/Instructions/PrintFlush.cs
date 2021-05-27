using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class PrintFlush : Instruction
	{
		public PrintFlush(ParamEvaluable messageBox) : base(OpCode.PRINTFLUSH, messageBox)
		{

		}
	}
}
