using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Assembly : Statement
	{
		public Stmt_Assembly(SourcePosition src) : base(src)
		{
		}
		public string AssemblyName
		{
			get;
			set;
		}
		public List<Stmt_Function> Functions
		{
			get;
			set;
		}
		public List<Statement> Body
		{
			get;
			set;
		}
	}
}
