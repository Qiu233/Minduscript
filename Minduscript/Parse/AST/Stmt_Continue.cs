using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Continue : Stmt_Verbal
	{
		public Stmt_Continue(SourcePosition src) : base(src)
		{
		}
		/// <summary>
		/// will be set after static check
		/// </summary>
		public Stmt_Loop Loop
		{
			get;
			set;
		}
	}
}
