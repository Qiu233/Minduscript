using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Control : Instruction
	{
		public Control(ControlModes mode, ParamEvaluable block, Param arg0, Param arg1, Param arg2, Param arg3) : base(OpCode.CONTROL, mode, block, arg0, arg1, arg2, arg3)
		{

		}
		public static Control Shoot(ParamEvaluable block, ParamEvaluable x, ParamEvaluable y, ParamEvaluable shoot)
		{
			return new Control(ControlModes.shoot, block, x, y, shoot, 0);
		}
		public static Control ShootP(ParamEvaluable block, ParamEvaluable unit, ParamEvaluable shoot)
		{
			return new Control(ControlModes.shootp, block, unit, shoot, 0, 0);
		}
		public static Control Configure(ParamEvaluable block, ParamEvaluable value)
		{
			return new Control(ControlModes.configure, block, value, 0, 0, 0);
		}
		public static Control Color(ParamEvaluable block, ParamEvaluable r, ParamEvaluable g, ParamEvaluable b)
		{
			return new Control(ControlModes.color, block, r, g, b, 0);
		}
	}
}
