using Minduscript.CompilingInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Lex
{
	public class Lexer
	{
		public static readonly string[] KeyWords = new string[] { 
			"assembly", "function", "import", "as", 
			"using", "global", "if", "else", "while", 
			"for", "break", "continue", "return", 
			"var", "true", "false" };
		public CompilingInfoHandler LexInfoHandler
		{
			get;
		}
		public string File
		{
			get; set;
		}
		public int Line
		{
			get; set;
		}
		public SourcePosition SourcePosition => new SourcePosition(File, Line);
		public int Index
		{
			get; set;
		}
		public char[] Source
		{
			get;
		}
		public Lexer(string code, CompilingInfoHandler errorHandler)
		{
			Line = 0;
			Source = code.ToCharArray();
			LexInfoHandler = errorHandler;
		}
		public Lexer(string code, string file, CompilingInfoHandler errorHandler) : this(code, errorHandler)
		{
			File = file;
		}
		public void Initialize()
		{
			Line = 1;
			Index = 0;
		}
		private bool IndexValid()
		{
			return Index >= 0 && Index < Source.Length;
		}
		private static bool IsLetter(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
		}
		private static bool IsNumber(char c)
		{
			return c >= '0' && c <= '9';
		}
		private static bool IsKeyWord(string c)
		{
			return KeyWords.Contains(c);
		}
		private void ThrowLexingError(SourcePosition src, string message)
		{
			LexInfoHandler.ThrowError(src, "LexingError", message);
		}
		public Token Next()
		{
			while (true)
			{
				if (!IndexValid()) return null;
				if (Source[Index] == ' ' || Source[Index] == '\t' || Source[Index] == '\r')
				{
					Index++;
					continue;
				}
				else if (Source[Index] == '\n')
				{
					Line++;
					Index++;
					continue;
				}
				break;
			}
			Token token = new Token
			{
				SourcePosition = SourcePosition
			};
			StringBuilder content = new StringBuilder();
			if (IsLetter(Source[Index]))//iden or keyword
			{
				while (IndexValid() && (IsLetter(Source[Index]) || IsNumber(Source[Index]) || Source[Index] == '_'))
					content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = IsKeyWord(c) ? TokenType.KEYWORD : TokenType.IDEN;
			}
			else if (IsNumber(Source[Index]))//number
			{
				while (IndexValid() && (IsNumber(Source[Index]) || Source[Index] == '.'))
					content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				if (!float.TryParse(c, out _))
					ThrowLexingError(SourcePosition, "Invalid number format");
				token.Type = TokenType.CONST_NUMBER;
			}
			else if (Source[Index] == '\"')//string const
			{
				Index++;
				while (IndexValid() && Source[Index] != '\"')
				{
					if (Source[Index] == '\r' || Source[Index] == '\n')
					{
						ThrowLexingError(SourcePosition, "String constant truncated");
						Index++;
						continue;
					}
					content.Append(Source[Index++]);
				}
				Index++;
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.CONST_STRING;
			}
			else if (Source[Index] == '=')//set/assignment or equal or strictequal
			{
				while (IndexValid() && Source[Index] == '=')
					content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				if (c.Length > 3)
					ThrowLexingError(SourcePosition, "More than 3 '=' found");
				token.Type = TokenType.OPTR;
			}
			else if (Source[Index] == '<' || Source[Index] == '>')//comparative
			{
				char r = Source[Index];
				content.Append(Source[Index++]);
				if (IndexValid() && (Source[Index] == '=' || Source[Index] == r))
					content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.OPTR;
			}
			else if (Source[Index] == '!')//comparative
			{
				content.Append(Source[Index++]);
				if (IndexValid() && Source[Index] == '=')
					content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.OPTR;
			}
			else if (Source[Index] == '+' || Source[Index] == '-' || Source[Index] == '*' || Source[Index] == '^' || Source[Index] == '%' || Source[Index] == '~')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.OPTR;
			}
			else if (Source[Index] == '/')
			{
				content.Append(Source[Index++]);
				if (IndexValid() && Source[Index] == '/')
					content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.OPTR;
			}
			else if (Source[Index] == '|' || Source[Index] == '&')
			{
				char r = Source[Index];
				content.Append(Source[Index++]);
				if (IndexValid() && Source[Index] == r)
					content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.OPTR;
			}
			else if (Source[Index] == ';')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.SEMICOLON;
			}
			else if (Source[Index] == ',')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.COMMA;
			}
			else if (Source[Index] == '.')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.DOT;
			}
			else if (Source[Index] == '(')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.PARENTHESES_L;
			}
			else if (Source[Index] == ')')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.PARENTHESES_R;
			}
			else if (Source[Index] == '[')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.BRACKET_L;
			}
			else if (Source[Index] == ']')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.BRACKET_R;
			}
			else if (Source[Index] == '{')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.BRACE_L;
			}
			else if (Source[Index] == '}')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.BRACE_R;
			}
			else if (Source[Index] == '$')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.OPTR;
			}
			else if (Source[Index] == '#' || Source[Index] == '@')
			{
				content.Append(Source[Index++]);
				string c = content.ToString();
				token.Value = c;
				token.Type = TokenType.OPTR;
			}
			return token;
		}
	}
}
