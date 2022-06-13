using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_Const : Expression
	{
		public Expr_Const(SourcePosition src) : base(src)
		{
		}
		public string Value
		{
			get;
			set;
		}
	}
}
