using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.CompilingInfo
{
	public abstract class ExceptionInfo : GeneralInfo
	{
		public ExceptionInfo(string type, string message) : base(type, message)
		{
		}
	}
}
