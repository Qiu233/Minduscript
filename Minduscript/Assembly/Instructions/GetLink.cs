using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class GetLink : Instruction
	{
		public GetLink(ParamEvaluable link, ParamEvaluable index) : base(OpCode.GETLINK, link, index)
		{

		}
	}
}
