using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILConstNumber : ILConst
	{
		public ILConstNumber(SourcePosition src) : base(src)
		{
		}
		public float Content
		{
			get;
			set;
		}
		public override string Value => Content.ToString();
	}
}
