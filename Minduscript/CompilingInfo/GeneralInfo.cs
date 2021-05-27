using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.CompilingInfo
{
	public class GeneralInfo
	{
		public string Doing
		{
			get;
		}
		public string Message
		{
			get;
		}
		public virtual string AssembledMessage => Message;

		public GeneralInfo(string doing, string msg)
		{
			Doing = doing;
			Message = msg;
		}
	}
}
