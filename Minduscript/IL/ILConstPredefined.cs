using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILConstPredefined : ILConst
	{
		public ILConstPredefined(SourcePosition src) : base(src)
		{
		}
		public string Content
		{
			get;
			set;
		}
		public override string Value => Content;
	}
}
