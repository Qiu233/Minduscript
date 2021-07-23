using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly
{
	public class CodeTarget
	{
		public Dictionary<string, ParamVariable> Variables
		{
			get;
		}
		public LinkedList<Instruction> Instructions
		{
			get;
		}

		public CodeTarget()
		{
			Instructions = new LinkedList<Instruction>();
			Variables = new Dictionary<string, ParamVariable>();
		}
	}
}
