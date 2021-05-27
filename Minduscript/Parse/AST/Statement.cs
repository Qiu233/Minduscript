using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public abstract class Statement : ASTNode
	{
		protected Statement(SourcePosition src) : base(src)
		{
		}
	}
}
