using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Var : Stmt_Verbal
	{
		public Stmt_Var(SourcePosition src) : base(src)
		{
		}
		public List<KeyValuePair<Expr_Variable, Expression>> Declarations
		{
			get;
			set;
		}
	}
}
