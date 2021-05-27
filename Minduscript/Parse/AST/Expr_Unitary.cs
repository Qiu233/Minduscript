using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_Unitary : Expression
	{
		public Expr_Unitary(SourcePosition src) : base(src)
		{
		}
		public string Operator
		{
			get;
			set;
		}
		public Expression Value
		{
			get;
			set;
		}
	}
}
