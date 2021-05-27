using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_ConstNumber : Expression
	{
		public Expr_ConstNumber(SourcePosition src) : base(src)
		{
		}
		public float Value
		{
			get;
			set;
		}
	}
}
