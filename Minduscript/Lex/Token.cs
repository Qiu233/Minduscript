using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Lex
{
	public class Token:IHasSourcePosition
	{
		public SourcePosition SourcePosition
		{
			get;
			set;
		}
		public TokenType Type
		{
			get; set;
		}
		public string Value
		{
			get; set;
		}

	}
}
