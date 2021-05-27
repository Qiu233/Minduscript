using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Assignment : Stmt_Verbal
	{
		public Stmt_Assignment(SourcePosition src) : base(src)
		{
		}
		public Expression Target
		{
			get;
			set;
		}
		public Expression Value
		{
			get;
			set;
		}
	}
}
