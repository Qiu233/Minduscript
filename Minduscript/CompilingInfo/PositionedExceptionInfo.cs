using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.CompilingInfo
{
	public abstract class PositionedExceptionInfo : ExceptionInfo, IHasSourcePosition
	{
		public SourcePosition SourcePosition
		{
			get;
			set;
		}

		public PositionedExceptionInfo(SourcePosition srcPos, string doing, string message) : base(doing, message)
		{
			SourcePosition = srcPos;
		}
	}
}
