using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_Res:Expression
	{
		public string Name
		{
			get;
			set;
		}
		public Expr_Res(SourcePosition src) : base(src)
		{
		}
	}
}
