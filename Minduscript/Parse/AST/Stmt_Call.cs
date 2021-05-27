using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Call : Stmt_Verbal
	{
		public Stmt_Call(SourcePosition src) : base(src)
		{
		}
		public Expr_Call Call
		{
			get;
			set;
		}
	}
}
