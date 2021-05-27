using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILOperandCollection<T> : LinkedList<T> where T : ILOperand
	{
	}
}
