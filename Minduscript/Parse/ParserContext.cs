using Minduscript.CompilingInfo;
using Minduscript.IL;
using Minduscript.Lex;
using Minduscript.Parse.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse
{
	/// <summary>
	/// Containing parser's context information
	/// </summary>
	public class ParserContext
	{
		public string Code
		{
			get;
		}
		public string File
		{
			get => Lexer.File;
		}
		public int Line
		{
			get => CurrentToken.SourcePosition.Line;
		}
		public Lexer Lexer
		{
			get;
		}
		public Token CurrentToken
		{
			get;
			set;
		}
		public CompilerContext CompilerContext
		{
			get;
		}
		public CompilingInfoHandler InfoHandler
		{
			get => CompilerContext.CompilingInfoHandler;
		}
		public Stmt_Assembly Root
		{
			get;
			set;
		}
		public ParserContext(string code, string file, CompilerContext compilerContext)
		{
			CompilerContext = compilerContext;
			Lexer = new Lexer(code, file, InfoHandler);
		}
		public void NextToken()
		{
			CurrentToken = Lexer.Next();
		}
		public void ThrowParsingError(SourcePosition pos, string message)
		{
			InfoHandler.ThrowError(pos, "Parsing", message);
		}
		public void ThrowILGeneratingError(SourcePosition pos, string message)
		{
			InfoHandler.ThrowError(pos, "ILGenerating", message);
		}
		public void ThrowStaticCheckingError(SourcePosition pos, string message)
		{
			InfoHandler.ThrowError(pos, "StaticChecking", message);
		}
		/// <summary>
		/// Reseting the context
		/// </summary>
		public void Initialize()
		{
			Root = null;
			Lexer.Initialize();
			NextToken();
		}
	}
}
