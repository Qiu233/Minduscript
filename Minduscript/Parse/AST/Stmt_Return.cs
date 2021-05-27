using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Return : Stmt_Verbal
	{
		public Stmt_Return(SourcePosition src) : base(src)
		{
		}
		public Expression ReturnValue
		{
			get;
			set;
		}
		public Stmt_Macro Macro
		{
			get;
			set;
		}
	}
}
