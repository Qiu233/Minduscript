using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public interface IILExecutable
	{
		public ILOperandCollection<ILInstruction> Instructions
		{
			get;
		}
		public string Name
		{
			get;
			set;
		}
	}
}
