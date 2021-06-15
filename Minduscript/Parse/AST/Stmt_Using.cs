using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Using : Stmt_Verbal
	{
		public Stmt_Using(SourcePosition src) : base(src)
		{
		}

		public List<KeyValuePair<Expr_Variable, Expr_GameConst>> Declarations
		{
			get;
			set;
		}
	}
}
