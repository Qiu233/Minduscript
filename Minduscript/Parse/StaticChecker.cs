using Minduscript.Parse.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse
{
	/// <summary>
	/// Provides symbol information
	/// </summary>
	public sealed class SymbolTableContext
	{
		public SymbolTable<string> VarSymbolTable
		{
			get;
			set;
		}
		public SymbolTable<string> FuncSymbolTable
		{
			get;
		}
		public SymbolTable<string> NamespaceSymbolTable
		{
			get;
		}
		public SymbolTable<string> Root
		{
			get;
		}

		private Dictionary<Expr_Variable, Symbol<string>> VarBind
		{
			get;
		}

		public SymbolTableContext()
		{
			NamespaceSymbolTable = new SymbolTable<string>();
			FuncSymbolTable = new SymbolTable<string>();
			VarSymbolTable = new SymbolTable<string>();//root
			Root = VarSymbolTable;

			VarBind = new Dictionary<Expr_Variable, Symbol<string>>();
		}

		public void PushSymbolTable()
		{
			VarSymbolTable = VarSymbolTable.NewSubTable();
		}
		public void PopSymbolTable()
		{
			VarSymbolTable = VarSymbolTable.Parent;
		}
		public bool VarSymbolExist(string ev)
		{
			SymbolTable<string> cur = VarSymbolTable;
			while (cur != null)
			{
				if (cur.ExistSymbol(ev))
					return true;
				cur = cur.Parent;
			}
			return false;
		}
		public bool FuncSymbolExist(string name)
		{
			return FuncSymbolTable.ExistSymbol(name);
		}

		/// <summary>
		/// To bind a Expr_Variable to an existed symbol
		/// </summary>
		/// <param name="ev"></param>
		public void BindVar(Expr_Variable ev)
		{
			SymbolTable<string> cur = VarSymbolTable;

			while (cur != null)
			{
				if (cur.ExistSymbol(ev.Name))
					break;
				cur = cur.Parent;
			}
			if (cur == null)
				return;//failed to bind because the specified variable doesn't exist
			VarBind[ev] = cur.Symbols.First(t => t.Content == ev.Name);
		}

		public bool IsVarBound(Expr_Variable ev)
		{
			return VarBind.ContainsKey(ev);
		}

		public bool TryGetVarBind(Expr_Variable ev, out Symbol<string> symbol)
		{
			return VarBind.TryGetValue(ev, out symbol);
		}
	}
	public class StaticChecker
	{
		private Stmt_Assembly Assembly
		{
			get;
		}
		private ParserContext Context
		{
			get;
		}
		private SymbolTableContext SymbolTableContext
		{
			get;
		}
		private Stmt_Loop CurrentLoop
		{
			get;
			set;
		}
		private bool IsPreCompiling
		{
			get;
			set;
		}
		private Stmt_Function CurrentFunction
		{
			get;
			set;
		}
		private bool IsInFunc
		{
			get => CurrentFunction != null;
		}
		private StaticChecker(Stmt_Assembly body, ParserContext context)
		{
			Assembly = body;
			Context = context;
			SymbolTableContext = new SymbolTableContext();
		}
		private void CheckExpr(Expression expr)
		{
			if (expr is Expr_Variable ev)
			{
				if (!SymbolTableContext.VarSymbolExist(ev.Name))
					Context.ThrowStaticCheckingError(ev.SourcePosition, $"Variable should be declared before using:{ev.Name}");
				else//variable exists
					SymbolTableContext.BindVar(ev);
			}
			else if (expr is Expr_Binary eb)
			{
				CheckExpr(eb.Left);
				CheckExpr(eb.Right);
			}
			else if (expr is Expr_Unitary eu)
			{
				CheckExpr(eu.Value);
			}
			else if (expr is Expr_Mem em)
			{
				CheckExpr(em.Mem);
				CheckExpr(em.Pos);
			}
			else if (expr is Expr_Call ec)
			{
				foreach (var arg in ec.Args)
					CheckExpr(arg);
			}
		}
		private void RegisterVariable(Expr_Variable ev)
		{
			if (SymbolTableContext.VarSymbolExist(ev.Name))
			{
				Context.ThrowStaticCheckingError(ev.SourcePosition, $"Variable definition duplicated:({ev.Name})");
				return;
			}
			SymbolTableContext.VarSymbolTable.AddSymbol(ev.Name);
			SymbolTableContext.BindVar(ev);//default bind
		}
		private void CheckStmt(Statement stmt)
		{
			if (stmt is Stmt_PreCompilation spc)
			{
				if (!IsPreCompiling)
				{
					Context.ThrowStaticCheckingError(spc.SourcePosition, $"Precompiling statement can only be put at the header of file");
					return;//ignore
				}
			}
			else
			{
				IsPreCompiling = false;
			}

			if (stmt is Stmt_Verbal && !(stmt is Stmt_Var) && CurrentFunction == null)
			{
				Context.ThrowStaticCheckingError(stmt.SourcePosition, "Verbal statement except var must be in a function");
				return;//ignore
			}

			if (stmt is Stmt_Block sb)
			{
				SymbolTableContext.PushSymbolTable();
				foreach (var s in sb.Statements)
					CheckStmt(s);
				SymbolTableContext.PopSymbolTable();
			}
			else if (stmt is Stmt_Function sm)
			{
				if (IsInFunc)
				{
					Context.ThrowStaticCheckingError(sm.SourcePosition, $"Function cannot be defined inside a function:({sm.FunctionName})");
					return;//Critical error,stop checking
				}
				if (SymbolTableContext.FuncSymbolExist(sm.FunctionName))
				{
					Context.ThrowStaticCheckingError(sm.SourcePosition, $"Definition of function duplicated in the same file, function:({sm.FunctionName})");
				}

				SymbolTableContext.PushSymbolTable();

				CurrentFunction = sm;
				SymbolTableContext.FuncSymbolTable.AddSymbol(sm.FunctionName);
				foreach (var p in sm.Params)
					RegisterVariable(p);
				foreach (var s in sm.Code.Statements)
					CheckStmt(s);
				CurrentFunction = null;

				SymbolTableContext.PopSymbolTable();
			}
			else if (stmt is Stmt_Using su)
			{
				foreach (var dc in su.Declarations)
				{
					RegisterVariable(dc.Key);
					if (dc.Value != null)
						CheckExpr(dc.Value);
				}
			}
			else if (stmt is Stmt_Var sv)
			{
				if (!IsInFunc && !sv.Declarations.TrueForAll(t => t.Value == null))
				{
					Context.ThrowStaticCheckingError(sv.SourcePosition, $"Variables can only be declared without assignment ouside a function");
				}
				foreach (var dc in sv.Declarations)
				{
					RegisterVariable(dc.Key);
					if (dc.Value != null)
						CheckExpr(dc.Value);
				}
			}
			else if (stmt is Stmt_Assignment sa)
			{
				CheckExpr(sa.Target);
				CheckExpr(sa.Value);
			}
			else if (stmt is Stmt_Return sr)
			{
				CheckExpr(sr.ReturnValue);
				sr.Function = CurrentFunction;
			}
			else if (stmt is Stmt_If si)
			{
				CheckExpr(si.Condition);
				CheckStmt(si.ContentCode);
				CheckEmbededStmt(si.ContentCode);
				CheckStmt(si.Else);
				CheckEmbededStmt(si.Else);
			}
			else if (stmt is Stmt_Loop sl)
			{
				CheckStmt(sl.Initialization);
				CheckExpr(sl.Condition);
				CurrentLoop = sl;
				CheckStmt(sl.Iteration);
				CheckStmt(sl.Code);
				CheckEmbededStmt(sl.Code);
				CurrentLoop = null;
			}
			else if (stmt is Stmt_Break sb1)
			{
				if (CurrentLoop == null)
					Context.ThrowStaticCheckingError(sb1.SourcePosition, $"Break statement can only be used in a while statement");
				sb1.Loop = CurrentLoop;
			}
			else if (stmt is Stmt_Continue sc1)
			{
				if (CurrentLoop == null)
					Context.ThrowStaticCheckingError(sc1.SourcePosition, $"Continue statement can only be used in a while statement");
				sc1.Loop = CurrentLoop;
			}
			else if (stmt is Stmt_Call sc)
			{
				CheckExpr(sc.Call);
			}
			else if (stmt is Stmt_Import si1)
			{
				if (SymbolTableContext.VarSymbolTable != SymbolTableContext.Root)
				{
					Context.ThrowStaticCheckingError(si1.SourcePosition, $"Import statement cannot be put inside any subscope");
					return;//critical error
				}
				if (SymbolTableContext.NamespaceSymbolTable.ExistSymbol(si1.Namespace))
				{
					Context.ThrowStaticCheckingError(si1.SourcePosition, $"Namespace duplicated:{si1.Namespace}");
					return;
				}
				if (si1.Namespace != null)
					SymbolTableContext.NamespaceSymbolTable.AddSymbol(si1.Namespace);
			}
			else if (stmt is Stmt_ASMCall sac)
			{
				foreach (var arg in sac.Args)
					CheckExpr(arg);
			}
		}
		private void CheckEmbededStmt(Statement stmt)
		{
			if (stmt is Stmt_Var || stmt is Stmt_Using)
			{
				Context.ThrowStaticCheckingError(stmt.SourcePosition, $"An embedded statement cannot be any declaration");
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns>Symbol context</returns>
		public SymbolTableContext CheckAll()
		{
			IsPreCompiling = true;
			foreach (var s in Assembly.Header)
				CheckStmt(s);
			IsPreCompiling = false;

			foreach (var s in Assembly.Functions)
				CheckStmt(s);
			return SymbolTableContext;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="asm">Assembly root to check</param>
		/// <param name="context">Parser context</param>
		/// <returns>Symbol context</returns>
		public static SymbolTableContext Check(Stmt_Assembly asm, ParserContext context)
		{
			StaticChecker sc = new StaticChecker(asm, context);
			return sc.CheckAll();
		}
	}
}
