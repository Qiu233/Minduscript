using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public abstract class Stmt_Loop : Stmt_Verbal
	{
		public HashSet<ILInstruction> Continue;
		public Stmt_Loop(SourcePosition src) : base(src)
		{
			Continue = new HashSet<ILInstruction>();
		}
		public Statement Initialization
		{
			get;
			set;
		}
		public Expression Condition
		{
			get;
			set;
		}
		public Statement Iteration
		{
			get;
			set;
		}
		public Statement Code
		{
			get;
			set;
		}
	}
}
