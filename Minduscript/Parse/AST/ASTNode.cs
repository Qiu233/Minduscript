using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public abstract class ASTNode : IHasSourcePosition
	{
		public EntryContext Entry
		{
			get;
		}
		public SourcePosition SourcePosition
		{
			get;
			set;
		}
		public ASTNode(SourcePosition pos)
		{
			SourcePosition = pos;
			Entry = new EntryContext();
		}
	}
}
