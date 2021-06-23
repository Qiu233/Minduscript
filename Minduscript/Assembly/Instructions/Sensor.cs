using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Sensor : Instruction
	{
		public Sensor(ParamEvaluable result, ParamEvaluable block, ParamEvaluable attr) : base(OpCode.SENSOR, result, block, attr)
		{

		}
	}
}
