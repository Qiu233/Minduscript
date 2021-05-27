using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Write : Instruction
	{
		public Write(ParamEvaluable resultV, ParamEvaluable memD, ParamEvaluable posV) : base(OpCode.WRITE, resultV, memD, posV)
		{

		}
	}
}
