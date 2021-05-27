using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	[Obsolete("Temporarily obsoleted")]
	public class Expr_GameRes : Expression
	{
		public Expr_GameRes(SourcePosition src) : base(src)
		{
		}
		public string Name
		{
			get;
			set;
		}
	}
}
