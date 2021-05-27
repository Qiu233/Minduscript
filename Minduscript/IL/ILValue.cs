using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public abstract class ILValue : ILOperand
	{
		public ILValue(SourcePosition src) : base(src)
		{

		}
	}
}
