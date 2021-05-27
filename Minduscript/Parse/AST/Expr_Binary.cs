using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_Binary : Expression
	{
		public Expr_Binary(SourcePosition src) : base(src)
		{
		}

		public string Operator
		{
			get;
			set;
		}
		public Expression Left
		{
			get;
			set;
		}
		public Expression Right
		{
			get;
			set;
		}
	}
}
