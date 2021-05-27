using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_If : Stmt_Verbal
	{
		public Stmt_If(SourcePosition src) : base(src)
		{
		}
		public Expression Condition
		{
			get;
			set;
		}
		public Statement ContentCode
		{
			get;
			set;
		}
		public Statement Else
		{
			get;
			set;
		}
	}
}
