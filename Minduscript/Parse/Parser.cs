using Minduscript.CompilingInfo;
using Minduscript.IL;
using Minduscript.Lex;
using Minduscript.Parse.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0017

namespace Minduscript.Parse
{
	/// <summary>
	/// Parser accepts one assembly as input, which has content code as well as an unique name for identification
	/// The output is an ILAssembly corresponding to the input
	/// Notice that the output has
	/// </summary>
	public class Parser
	{
		public ParserContext ParserContext
		{
			get;
		}
		private bool ForceSemicolon
		{
			get;
			set;
		}
		private CompilingInfoHandler InfoHandler
		{
			get => ParserContext.InfoHandler;
		}
		private Token CurrentToken
		{
			get => ParserContext.CurrentToken;
		}
		private SourcePosition SourcePosition
		{
			get => CurrentToken == null ? ParserContext.Lexer.SourcePosition : CurrentToken.SourcePosition;
		}
		public Parser(string code, string file, CompilerContext compilerContext)
		{
			ParserContext = new ParserContext(code, file, compilerContext);
			ForceSemicolon = true;//default
		}
		private void NextToken()
		{
			ParserContext.NextToken();
		}
		private Token Accept()
		{
			Token t = CurrentToken;
			NextToken();
			return t;
		}
		public void ThrowParsingError(string message)
		{
			InfoHandler.ThrowError(SourcePosition, "ParsingError", message);
		}
		private Token Accept(TokenType type)
		{
			if (CurrentToken == null)
			{
				ThrowParsingError("Unexpected end of stream");
				return null;
			}
			if (CurrentToken.Type != type)
			{
				ThrowParsingError($"Unexpected Token({CurrentToken.Type},\"{CurrentToken.Value}\") Expected Token:({type},*)");
				RestoreFromError();
				return null;
			}
			Token t = CurrentToken;
			NextToken();
			return t;
		}
		private Token Accept(TokenType type, string value)
		{
			if (CurrentToken == null)
			{
				ThrowParsingError("Unexpected end of stream");
				return null;
			}
			if (CurrentToken.Type != type || CurrentToken.Value != value)
			{
				ThrowParsingError($"Unexpected Token({CurrentToken.Type},\"{CurrentToken.Value}\") Expected Token:({type},\"{value}\")");
				RestoreFromError();
				return null;
			}
			Token t = CurrentToken;
			NextToken();
			return t;
		}
		private void TryAccept(TokenType type)
		{
			if (Match(type))
			{
				Accept();
			}
		}
		private void RestoreFromError()
		{
			while (CurrentToken != null && !(
				Match(TokenType.SEMICOLON)
				|| Match(TokenType.BRACE_R)
				))
			{
				Accept();
			}
		}
		private bool Match(TokenType type)
		{
			if (CurrentToken == null)
			{
				return false;
			}
			return CurrentToken.Type == type;
		}
		private bool Match(TokenType type, string value)
		{
			if (CurrentToken == null)
			{
				return false;
			}
			return CurrentToken.Type == type && CurrentToken.Value == value;
		}



		private Expression E()//||
		{
			Expression left = E2();
			Expr_Binary eb = E11(left);
			return eb ?? left;
		}
		private Expr_Binary E11(Expression left)
		{
			if (Match(TokenType.OPTR, "||"))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = "||";
				Accept();
				eb.Left = left;
				eb.Right = E2();
				return E11(eb) ?? eb;
			}
			return null;
		}
		private Expression E2()//&&
		{
			Expression left = E3();
			Expr_Binary eb = E21(left);
			return eb ?? left;
		}
		private Expr_Binary E21(Expression left)
		{
			if (Match(TokenType.OPTR, "&&"))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = "&&";
				Accept();
				eb.Left = left;
				eb.Right = E3();
				return E21(eb) ?? eb;
			}
			return null;
		}
		private Expression E3()//|
		{
			Expression left = E4();
			Expr_Binary eb = E31(left);
			return eb ?? left;
		}
		private Expr_Binary E31(Expression left)
		{
			if (Match(TokenType.OPTR, "|"))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = "|";
				Accept();
				eb.Left = left;
				eb.Right = E4();
				return E31(eb) ?? eb;
			}
			return null;
		}
		private Expression E4()//^
		{
			Expression left = E5();
			Expr_Binary eb = E41(left);
			return eb ?? left;
		}
		private Expr_Binary E41(Expression left)
		{

			if (Match(TokenType.OPTR, "^"))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = "^";
				Accept();
				eb.Left = left;
				eb.Right = E5();
				return E41(eb) ?? eb;
			}
			return null;
		}
		private Expression E5()//&
		{
			Expression left = E6();
			Expr_Binary eb = E51(left);
			return eb ?? left;
		}
		private Expr_Binary E51(Expression left)
		{
			if (Match(TokenType.OPTR, "&"))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = "&";
				Accept();
				eb.Left = left;
				eb.Right = E6();
				return E51(eb) ?? eb;
			}
			return null;
		}
		private Expression E6()//== !=
		{
			Expression left = E7();
			Expr_Binary eb = E61(left);
			return eb ?? left;
		}
		private Expr_Binary E61(Expression left)
		{
			if (Match(TokenType.OPTR, "==") || Match(TokenType.OPTR, "!="))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = Accept().Value;
				eb.Left = left;
				eb.Right = E7();
				return E61(eb) ?? eb;
			}
			return null;
		}
		private Expression E7()//== !=
		{
			Expression left = E8();
			Expr_Binary eb = E71(left);
			return eb ?? left;
		}
		private Expr_Binary E71(Expression left)
		{
			if (Match(TokenType.OPTR, "<") || Match(TokenType.OPTR, ">") || Match(TokenType.OPTR, ">=") || Match(TokenType.OPTR, "<="))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = Accept().Value;
				eb.Left = left;
				eb.Right = E8();
				return E71(eb) ?? eb;
			}
			return null;
		}
		private Expression E8()//<< >>
		{
			Expression left = E9();
			Expr_Binary eb = E81(left);
			return eb ?? left;
		}
		private Expr_Binary E81(Expression left)
		{
			if (Match(TokenType.OPTR, "<<") || Match(TokenType.OPTR, ">>"))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = Accept().Value;
				Accept();
				eb.Left = left;
				eb.Right = E9();
				return E81(eb) ?? eb;
			}
			return null;
		}
		private Expression E9()//+ -
		{
			Expression left = E10();
			Expr_Binary eb = E91(left);
			return eb ?? left;
		}
		private Expr_Binary E91(Expression left)
		{
			if (Match(TokenType.OPTR, "+") || Match(TokenType.OPTR, "-"))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = Accept().Value;
				eb.Left = left;
				eb.Right = E10();
				return E91(eb) ?? eb;
			}
			return null;
		}
		private Expression E10()//* / %
		{
			Expression left = E11();
			Expr_Binary eb = E101(left);
			return eb ?? left;
		}
		private Expr_Binary E101(Expression left)
		{
			if (Match(TokenType.OPTR, "*") || Match(TokenType.OPTR, "/") || Match(TokenType.OPTR, "%"))
			{
				Expr_Binary eb = new Expr_Binary(SourcePosition);
				eb.Operator = Accept().Value;
				eb.Left = left;
				eb.Right = E11();
				return E101(eb) ?? eb;
			}
			return null;
		}
		private Expression E11()//! ~
		{
			if (Match(TokenType.OPTR, "!") || Match(TokenType.OPTR, "~"))
			{
				Expr_Unitary eu = new Expr_Unitary(SourcePosition);
				eu.Operator = Accept().Value;
				eu.Value = E12();
				return eu;
			}
			return E12();
		}
		private Expression E12()//memread
		{
			Expression f = F();
			if (Match(TokenType.BRACKET_L))
			{
				Expr_Mem em = new Expr_Mem(SourcePosition);
				em.Mem = f;
				Accept();//[
				em.Pos = E();
				Accept(TokenType.BRACKET_R);
				return f;
			}
			return f;
		}

		private List<Expression> GetAttributes()
		{
			List<Expression> es = new List<Expression>();
			while (!Match(TokenType.OPTR, "#"))
			{
				if (Match(TokenType.COMMA))
				{
					Accept();//,
				}
				if (Match(TokenType.OPTR, "$"))
				{
					Accept();//$
					Token iden = Accept(TokenType.IDEN);
					es.Add(new Expr_GameConst(iden.SourcePosition) { Content = iden.Value });
				}
				else
					es.Add(E());
			}
			return es;
		}

		private Expression F()//()
		{
			if (Match(TokenType.PARENTHESES_L))
			{
				Accept();
				Expression e = E();
				Accept(TokenType.PARENTHESES_R);
				return e;
			}
			else if (Match(TokenType.RESOURCE))
			{
				Expr_Res res = new Expr_Res(SourcePosition) { Name = Accept().Value };
				return res;
			}
			else if (Match(TokenType.IDEN))//call/variable
			{
				Token iden = Accept(TokenType.IDEN);
				if (Match(TokenType.PARENTHESES_L))//call
				{
					Expr_Call ec = new Expr_Call(SourcePosition);
					ec.Function = iden.Value;
					Accept();//(
					ec.Args = GetArgs();
					Accept(TokenType.PARENTHESES_R);//)
					return ec;
				}
				else if (Match(TokenType.BRACKET_L))//mem
				{
					Expr_Mem em = new Expr_Mem(SourcePosition);
					em.Mem = GetVariable(iden.Value);
					Accept();//[
					em.Pos = E();
					Accept(TokenType.BRACKET_R);//]
					return em;
				}
				else if (Match(TokenType.DOT))//.
				{
					Accept();//.
					Token subName = Accept(TokenType.IDEN);
					Expr_Call ec = new Expr_Call(SourcePosition);
					ec.Namespace = iden.Value;
					ec.Function = subName.Value;
					Accept();//(
					ec.Args = GetArgs();
					Accept(TokenType.PARENTHESES_R);//)
					return ec;
				}
				else//variable
				{
					Expr_Variable ev = new Expr_Variable(SourcePosition);
					ev.Name = iden.Value;
					return ev;
				}
			}
			else if (Match(TokenType.CONST_STRING))
			{
				Expr_ConstString ecs = new Expr_ConstString(SourcePosition);
				ecs.Value = Accept().Value;
				return ecs;
			}
			else if (Match(TokenType.CONST_NUMBER))
			{
				Expr_ConstNumber ecn = new Expr_ConstNumber(SourcePosition)
				{
					Value = float.Parse(Accept().Value)
				};
				return ecn;
			}
			else if (Match(TokenType.KEYWORD, "true"))
			{
				Accept();
				Expr_ConstNumber ecn = new Expr_ConstNumber(SourcePosition)
				{
					Value = 1
				};
				return ecn;
			}
			else if (Match(TokenType.KEYWORD, "false"))
			{
				Accept();
				Expr_ConstNumber ecn = new Expr_ConstNumber(SourcePosition)
				{
					Value = 0
				};
				return ecn;
			}
			return null;
		}
		private Expr_Variable GetVariable(string iden)
		{
			return new Expr_Variable(SourcePosition) { Name = iden };
		}
		private Stmt_PreCompilation GetPreCompilationStatement()
		{
			if (Match(TokenType.KEYWORD, "import"))//import "example.ms";
			{
				Stmt_Import s = new Stmt_Import(SourcePosition);
				Accept();//import
				s.ImportFile = Accept(TokenType.CONST_STRING).Value;
				if (Match(TokenType.KEYWORD, "as"))//import "example.ms" as example;
				{
					Accept();//as
					s.Namespace = Accept(TokenType.IDEN).Value;
				}
				return s;
			}
			return null;
		}
		private List<Expr_Variable> GetParams()
		{
			List<Expr_Variable> vs = new List<Expr_Variable>();
			while (!Match(TokenType.PARENTHESES_R))
			{
				if (Match(TokenType.IDEN))
				{
					vs.Add(GetVariable(Accept(TokenType.IDEN).Value));
				}
				else if (Match(TokenType.COMMA))
				{
					Accept();
					vs.Add(GetVariable(Accept(TokenType.IDEN).Value));
				}
			}
			return vs;
		}
		private List<Expression> GetArgs()
		{
			List<Expression> es = new List<Expression>();
			while (!Match(TokenType.PARENTHESES_R))
			{
				if (Match(TokenType.COMMA))
				{
					Accept();
					es.Add(E());
				}
				else
				{
					es.Add(E());
				}
			}
			return es;
		}
		private Stmt_Verbal GetVerbalStatement()
		{
			if (Match(TokenType.BRACE_L))
			{
				return GetBlock();
			}
			else if (Match(TokenType.KEYWORD, "if"))
			{
				Stmt_If si = new Stmt_If(SourcePosition);
				Accept();
				Accept(TokenType.PARENTHESES_L);
				si.Condition = E();
				Accept(TokenType.PARENTHESES_R);
				si.ContentCode = GetStatement();
				if (Match(TokenType.KEYWORD, "else"))
				{
					Accept();
					si.Else = GetStatement();
				}
				return si;
			}
			else if (Match(TokenType.KEYWORD, "while"))
			{
				Stmt_While sw = new Stmt_While(SourcePosition);
				Accept();
				Accept(TokenType.PARENTHESES_L);
				sw.Condition = E();
				Accept(TokenType.PARENTHESES_R);
				sw.Code = GetStatement();
				return sw;
			}
			else if (Match(TokenType.KEYWORD, "for"))
			{
				Accept();//for
				Stmt_For sf = new Stmt_For(SourcePosition);
				Accept(TokenType.PARENTHESES_L);
				sf.Initialization = GetStatement();//init
				sf.Condition = E();//condition
				Accept(TokenType.SEMICOLON);

				var tmp = ForceSemicolon;
				ForceSemicolon = false;
				sf.Iteration = GetStatement();
				ForceSemicolon = tmp;

				Accept(TokenType.PARENTHESES_R);
				sf.Code = GetStatement();

				return sf;
			}
			else if (Match(TokenType.KEYWORD, "break"))
			{
				Stmt_Break sb = new Stmt_Break(SourcePosition);
				Accept();
				return sb;
			}
			else if (Match(TokenType.KEYWORD, "continue"))
			{
				Stmt_Continue sc = new Stmt_Continue(SourcePosition);
				Accept();
				return sc;
			}
			else if (Match(TokenType.KEYWORD, "return"))
			{
				Stmt_Return sr = new Stmt_Return(SourcePosition);
				Accept();
				if (!Match(TokenType.SEMICOLON))//return value;
					sr.ReturnValue = E();
				return sr;
			}
			else if (Match(TokenType.KEYWORD, "var"))
			{
				Accept();//var
				Stmt_Var sv = new Stmt_Var(SourcePosition);
				sv.Declarations = new List<KeyValuePair<Expr_Variable, Expression>>();
				do
				{
					string name = Accept(TokenType.IDEN).Value;
					Expression value = null;
					if (Match(TokenType.OPTR, "="))
					{
						Accept();
						value = E();
					}
					sv.Declarations.Add(new KeyValuePair<Expr_Variable, Expression>(new Expr_Variable(SourcePosition) { Name = name }, value));
					if (!Match(TokenType.COMMA))
						break;
					Accept();
				} while (true);
				return sv;
			}
			else if (Match(TokenType.KEYWORD, "using"))//using main_cell = cell1;
			{
				Accept();
				Stmt_Using s = new Stmt_Using(SourcePosition);
				s.Declarations = new List<KeyValuePair<Expr_Variable, Expr_GameConst>>();

				do
				{
					Token iden = Accept(TokenType.IDEN);
					Expr_GameConst value = new Expr_GameConst(iden.SourcePosition);
					value.Content = iden.Value;
					if (Match(TokenType.OPTR, "="))
					{
						Accept();
						Token alia = Accept(TokenType.IDEN);
						value = new Expr_GameConst(alia.SourcePosition);
						value.Content = alia.Value;
					}
					s.Declarations.Add(new KeyValuePair<Expr_Variable, Expr_GameConst>(
						new Expr_Variable(SourcePosition)
						{
							Name = iden.Value
						}, value));
					if (!Match(TokenType.COMMA))
						break;
					Accept();//,
				} while (true);
				return s;
			}
			else if (Match(TokenType.IDEN))
			{
				Expression target = F();
				if (target is Expr_Call ec)//call
				{
					Stmt_Call sc = new Stmt_Call(ec.SourcePosition);
					sc.Call = ec;
					return sc;
				}
				else if (Match(TokenType.OPTR, "="))//assignment
				{
					Stmt_Assignment sa = new Stmt_Assignment(SourcePosition);
					sa.Target = target;
					Accept();//=
					sa.Value = E();
					return sa;
				}
			}
			else if (Match(TokenType.OPTR, "#"))
			{
				Accept();
				Token iden = Accept(TokenType.IDEN);
				Stmt_ASMCall ec = new Stmt_ASMCall(SourcePosition);

				ec.Function = iden.Value;
				ec.Args = GetAttributes();
				Accept(TokenType.OPTR, "#");

				return ec;
			}
			return null;
		}
		private Stmt_Block GetBlock()
		{
			Stmt_Block block = new Stmt_Block(SourcePosition)
			{
				Statements = new List<Statement>()
			};
			Accept(TokenType.BRACE_L);
			Statement s;
			while ((s = GetStatement()) != null)
				block.Statements.Add(s);
			Accept(TokenType.BRACE_R);
			return block;
		}
		private Stmt_Function GetFunction()
		{
			if (Match(TokenType.KEYWORD, "function"))
			{
				Stmt_Function function = new Stmt_Function(SourcePosition);
				Accept();
				function.FunctionName = Accept(TokenType.IDEN).Value;
				Accept(TokenType.PARENTHESES_L);
				function.Params = GetParams();
				Accept(TokenType.PARENTHESES_R);
				function.Code = GetBlock();
				return function;
			}
			return null;
		}
		private Statement GetStatement()
		{
			Statement result;
			if ((result = GetPreCompilationStatement()) != null)
			{
				Accept(TokenType.SEMICOLON);
			}
			else if ((result = GetFunction()) != null)
			{
			}
			else if ((result = GetVerbalStatement()) != null)
			{
				if (!(result is Stmt_Loop || result is Stmt_If || result is Stmt_Block || result is Stmt_ASMCall))
				{
					if (ForceSemicolon)
						Accept(TokenType.SEMICOLON);
					else
						TryAccept(TokenType.SEMICOLON);
				}
			}
			else
			{
			}
			return result;
		}


		private Stmt_Assembly ParseAssembly()
		{
			ParserContext.Initialize();
			Stmt_Assembly sa = new Stmt_Assembly(SourcePosition);
			ParserContext.Root = sa;
			sa.Functions = new List<Stmt_Function>();
			sa.Header = new List<Statement>();

			if (!Match(TokenType.KEYWORD, "assembly"))
			{
				ThrowParsingError("Assembly name must be declared at the header of file");
			}
			else
			{
				Accept(TokenType.KEYWORD, "assembly");
				sa.AssemblyName = Accept(TokenType.IDEN).Value;
				Accept(TokenType.SEMICOLON);
			}

			//parsing body
			Statement s;
			while ((s = GetStatement()) != null)
			{
				if (s is Stmt_Function sm)
					sa.Functions.Add(sm);
				else
					sa.Header.Add(s);
			}
			return sa;
		}


		public ILAssembly CompileToILAssembly()
		{
			Stmt_Assembly stmts = ParseAssembly();//first
			return ILAssemblyGenerator.Generate(stmts, StaticChecker.Check(stmts, ParserContext), ParserContext);
		}
	}
}
