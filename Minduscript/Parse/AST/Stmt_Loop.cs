using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public abstract class Stmt_Loop : Stmt_Verbal
	{
		public Stmt_Loop(SourcePosition src) : base(src)
		{
		}
		public Expression Condition
		{
			get;
			set;
		}
		public Statement Code
		{
			get;
			set;
		}
	}
}
