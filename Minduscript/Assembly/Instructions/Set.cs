using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Set : Instruction
	{
		public Set(ParamEvaluable dst, ParamEvaluable src) : base(OpCode.SET, dst, src)
		{

		}
	}
}
