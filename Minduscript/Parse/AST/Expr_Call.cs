using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_Call : Expression
	{
		public Expr_Call(SourcePosition src) : base(src)
		{
		}
		public string Namespace
		{
			get;
			set;
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
