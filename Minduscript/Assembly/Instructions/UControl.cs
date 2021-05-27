using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class UControl : Instruction
	{
		public UControl(UControlModes mode, Param arg0, Param arg1, Param arg2, Param arg3, Param arg4) : base(OpCode.UCONTROL, mode, arg0, arg1, arg2, arg3, arg4)
		{

		}
	}
}
