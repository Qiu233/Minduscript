using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Stmt_Import : Stmt_PreCompilation
	{
		public Stmt_Import(SourcePosition src) : base(src)
		{
		}
		public string ImportFile
		{
			get;
			set;
		}
		public string Namespace
		{
			get;
			set;
		}
	}
}
