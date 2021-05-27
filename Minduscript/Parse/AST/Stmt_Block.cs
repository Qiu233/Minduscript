using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Block : Stmt_Verbal
	{
		public Stmt_Block(SourcePosition src) : base(src)
		{
		}
		public List<Statement> Statements
		{
			get;
			set;
		}
	}
}
