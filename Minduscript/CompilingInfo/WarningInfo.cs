using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.CompilingInfo
{
	public class WarningInfo : PositionedExceptionInfo
	{
		public WarningInfo(SourcePosition srcPos, string doing, string message) : base(srcPos, doing, message)
		{
		}
		public override string AssembledMessage => $"[{Doing}]Warning occured at {SourcePosition}:{{{Message}}}";
	}
}
