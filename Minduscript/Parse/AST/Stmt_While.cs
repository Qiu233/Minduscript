using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_While : Stmt_Loop
	{
		public Stmt_While(SourcePosition src) : base(src)
		{
		}
	}
}
