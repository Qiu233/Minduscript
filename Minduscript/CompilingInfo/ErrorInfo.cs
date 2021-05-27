using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.CompilingInfo
{
	/// <summary>
	/// for both parsing and lexing
	/// </summary>
	public class ErrorInfo : PositionedExceptionInfo
	{
		public ErrorInfo(SourcePosition srcPos, string doing, string message) : base(srcPos, doing, message)
		{
		}
		public override string AssembledMessage => $"[{Doing}]Error occured at {SourcePosition}:{{{Message}}}";
	}
}
