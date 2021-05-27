using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public abstract class Expression : ASTNode
	{
		protected Expression(SourcePosition src) : base(src)
		{
		}
	}
}
