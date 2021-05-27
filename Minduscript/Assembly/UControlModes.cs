using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly
{
	public enum UControlModes
	{
		idle, stop, move, approach,
		boost, pathfind, target, targetp,
		itemDrop, itemTake, payDrop, payTake, mine, flag, build, getBlock, within
	}
}
