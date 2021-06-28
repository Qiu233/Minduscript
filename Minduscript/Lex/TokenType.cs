using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Lex
{
	public enum TokenType
	{
		KEYWORD,
		IDEN,
		RESOURCE,
		CONST_STRING,
		CONST_NUMBER,

		OPTR,

		PARENTHESES_L,
		PARENTHESES_R,
		BRACKET_L,
		BRACKET_R,
		BRACE_L,
		BRACE_R,

		SEMICOLON,
		COMMA,
		DOT,//.
	}
}
