using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Radar : Instruction
	{
		public Radar(UnitAttributes attr0, UnitAttributes attr1, UnitAttributes attr2, UnitSortAccordance sort, ParamEvaluable detector, ParamEvaluable order, ParamEvaluable result) : base(OpCode.RADAR, attr0, attr1, attr2, sort, detector, order, result)
		{

		}
	}
}
