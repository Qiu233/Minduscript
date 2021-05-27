using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Print : Instruction
	{
		public Print(ParamEvaluable text) : base(OpCode.PRINT, text)
		{

		}
	}
}
