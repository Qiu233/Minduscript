using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public abstract class ILOperand : IHasSourcePosition
	{
		public SourcePosition SourcePosition
		{
			get;
			set;
		}

		public ILOperand(SourcePosition pos)
		{
			SourcePosition = pos;
		}

		public static explicit operator ILOperand(string s)
		{
			return new ILSymbol(new SourcePosition(null, -1)) { Value = s };
		}
	}
}
