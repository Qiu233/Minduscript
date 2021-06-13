using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILGameConst : ILOperand
	{
		public string Name
		{
			get;
			set;
		}
		public ILGameConst(SourcePosition pos) : base(pos)
		{
		}
	}
}
