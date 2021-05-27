using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly.Instructions
{
	public class Read : Instruction
	{
		public Read(ParamEvaluable resultV, ParamEvaluable memD, ParamEvaluable posV) : base(OpCode.READ, resultV, memD, posV)
		{

		}
	}
}
