using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILSymbol : ILOperand
	{
		public ILSymbol(SourcePosition pos) : base(pos)
		{
		}
		public string Value
		{
			get;
			set;
		}
	}
}
