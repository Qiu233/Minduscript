using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_GameConst : Expression
	{
		public string Content
		{
			get;
			set;
		}
		public Expr_GameConst(SourcePosition src) : base(src)
		{
		}
	}
}
