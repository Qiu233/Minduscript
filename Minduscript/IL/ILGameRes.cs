using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public abstract class ILGameRes : ILOperand
	{
		public string Name
		{
			get;
			set;
		}
		public ILGameRes(SourcePosition pos) : base(pos)
		{
		}
	}
}
