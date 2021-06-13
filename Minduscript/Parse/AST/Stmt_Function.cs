using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Function : Statement
	{
		public Stmt_Function(SourcePosition src) : base(src)
		{
		}
		public string FunctionName
		{
			get;
			set;
		}
		public List<Expr_Variable> Params
		{
			get;
			set;
		}
		public Expr_Variable ReturnValue
		{
			get;
			set;
		}
		public Stmt_Block Code
		{
			get;
			set;
		}
	}
}
