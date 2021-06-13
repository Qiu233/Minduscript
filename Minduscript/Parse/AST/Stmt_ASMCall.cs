using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_ASMCall : Stmt_Verbal
	{
		public Stmt_ASMCall(SourcePosition src) : base(src)
		{
		}
		public string Macro
		{
			get;
			set;
		}
		public List<Expression> Args
		{
			get;
			set;
		}
	}
}
