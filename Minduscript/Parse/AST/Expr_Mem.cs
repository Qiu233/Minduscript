using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_Mem : Expression
	{
		public Expr_Mem(SourcePosition src) : base(src)
		{
		}
		public Expression Mem
		{
			get;
			set;
		}
		public Expression Pos
		{
			get;
			set;
		}
	}
}
