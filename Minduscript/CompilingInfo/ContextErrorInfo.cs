using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.CompilingInfo
{
	public class ContextErrorInfo : ExceptionInfo
	{
		public ContextErrorInfo(string type, string msg) : base(type, msg)
		{
		}

		public override string AssembledMessage => $"[{Doing}]Error occured:{{{Message}}}";
	}
}
