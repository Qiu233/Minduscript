using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class URadar : Instruction
	{
		public URadar(UnitAttributes attr0, UnitAttributes attr1, UnitAttributes attr2, UnitSortAccordance sort, ParamEvaluable unknown, ParamEvaluable order, ParamEvaluable result) : base(OpCode.URADAR, attr0, attr1, attr2, sort, unknown, order, result)
		{

		}
	}
}
