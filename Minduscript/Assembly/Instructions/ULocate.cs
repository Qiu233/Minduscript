using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class ULocate : Instruction
	{
		public ULocate(TileTypes target, BuildingGroups group, ParamEvaluable isEnemy, ParamResource ore, ParamEvaluable outX, ParamEvaluable outY, ParamEvaluable found, ParamEvaluable buildingFound) : base(OpCode.ULOCATE, target, group, isEnemy, ore, outX, outY, found, buildingFound)
		{

		}
	}
}
