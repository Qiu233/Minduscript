using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public abstract class Stmt_Verbal : Statement
	{
		public Stmt_Verbal(SourcePosition src) : base(src)
		{
		}
	}
}
