using Minduscript.Parse.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse
{
	public class StaticChecker
	{
		private Stmt_Assembly Assembly
		{
			get;
		}
		private Stack<SymbolTable<string>> SymbolTables
		{
			get;
			set;
		}
		private SymbolTable<string> MacroSymbolTable
		{
			get;
		}
		private SymbolTable<string> NamespaceSymbolTable
		{
			get;
		}
		private ParserContext Context
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
		private Stmt_Macro CurrentMacro
		{
			get;
			set;
		}
		private bool IsInMacro
		{
			get => CurrentMacro != null;
		}
		private void PushSymbolTable()
		{
			SymbolTables.Push(new SymbolTable<string>());
		}
		private void PopSymbolTable()
		{
			SymbolTables.Pop();
		}
		private bool SymbolExist(string ev)
		{
			foreach (var s in SymbolTables)
			{
				if (s.ExistSymbol(ev))
					return true;
			}
			return false;
		}
		private bool MacroSymbolExist(string name)
		{
			return MacroSymbolTable.ExistSymbol(name);
		}
		private int SymbolOffset(string ev)
		{
			int off = 1;
			foreach (var s in SymbolTables.Reverse())
			{
				if (s.ExistSymbol(ev))
				{
					return off + s.GetSymbolOffset(ev);
				}
				off += s.Symbols.Count;
			}
			return -1;
		}
		private StaticChecker(Stmt_Assembly body, ParserContext context)
		{
			Assembly = body;
			NamespaceSymbolTable = new SymbolTable<string>();
			MacroSymbolTable = new SymbolTable<string>();
			SymbolTables = new Stack<SymbolTable<string>>();
			Context = context;
			PushSymbolTable();
		}
		private void CheckExpr(Expression expr)
		{
			if (expr is Expr_Variable ev)
			{
				if (!SymbolExist(ev.Name))
					Context.ThrowStaticCheckingError(ev.SourcePosition, $"Variable should be declared before using:{ev.Name}");
				ev.IsLocal = IsInMacro;
				ev.Offset = SymbolOffset(ev.Name);//set the offset
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
			if (SymbolExist(ev.Name))
			{
				Context.ThrowStaticCheckingError(ev.SourcePosition, $"Variable definition duplicated:({ev.Name})");
				return;
			}
			SymbolTables.Peek().AddSymbol(ev.Name);
			ev.IsLocal = IsInMacro;
			ev.Offset = SymbolOffset(ev.Name);//set the offset
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

			if (stmt is Stmt_Block sb)
			{
				PushSymbolTable();
				foreach (var s in sb.Statements)
					CheckStmt(s);
				PopSymbolTable();
			}
			else if (stmt is Stmt_Macro sm)
			{
				if (IsInMacro)
				{
					Context.ThrowStaticCheckingError(sm.SourcePosition, $"Macro cannot be defined inside a macro:({sm.MacroName})");
					return;//Critical error,stop checking
				}
				if (MacroSymbolExist(sm.MacroName))
				{
					Context.ThrowStaticCheckingError(sm.SourcePosition, $"Definition of macro duplicated in the same file, macro:({sm.MacroName})");
				}

				var tmpTables = SymbolTables;
				SymbolTables = new Stack<SymbolTable<string>>();

				PushSymbolTable();

				CurrentMacro = sm;
				sm.ReturnValue.Offset = 0;
				MacroSymbolTable.AddSymbol(sm.MacroName);
				foreach (var p in sm.Params)
					RegisterVariable(p);
				foreach (var s in sm.Code.Statements)
					CheckStmt(s);
				CurrentMacro = null;

				PopSymbolTable();

				SymbolTables = tmpTables;
			}
			else if (stmt is Stmt_Using su)
			{
				RegisterVariable(su.Target);
			}
			else if (stmt is Stmt_Var sv)
			{
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
				/*if (!IsInMacro)
					Context.ThrowStaticCheckingError(sr.SourcePosition, $"Return statement can only be used in a macro");*/
				CheckExpr(sr.ReturnValue);
				sr.Macro = CurrentMacro;
			}
			else if (stmt is Stmt_If si)
			{
				CheckExpr(si.Condition);
				CheckStmt(si.ContentCode);
				CheckEmbededStmt(si.ContentCode);
				CheckStmt(si.Else);
				CheckEmbededStmt(si.Else);
			}
			else if (stmt is Stmt_While sw)
			{
				CheckExpr(sw.Condition);
				CurrentLoop = sw;
				CheckStmt(sw.Code);
				CheckEmbededStmt(sw.Code);
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
				if (SymbolTables.Count > 1)
				{
					Context.ThrowStaticCheckingError(si1.SourcePosition, $"Import statement cannot be put inside any subscope");
					return;//critical error
				}
				if (NamespaceSymbolTable.ExistSymbol(si1.Namespace))
				{
					Context.ThrowStaticCheckingError(si1.SourcePosition, $"Namespace duplicated:{si1.Namespace}");
					return;
				}
				if (si1.Namespace != null)
					NamespaceSymbolTable.AddSymbol(si1.Namespace);
			}
			else if(stmt is Stmt_ASMCall sac)
			{
				foreach (var arg in sac.Args)
					CheckExpr(arg);
			}
		}
		private void CheckEmbededStmt(Statement stmt)
		{
			if (stmt is Stmt_PreCompilation || stmt is Stmt_Var || stmt is Stmt_Using)
			{
				Context.ThrowStaticCheckingError(stmt.SourcePosition, $"An embedded statement cannot be precompilation or declaration");
			}
		}
		public void CheckAll()
		{
			foreach (var s in Assembly.Macros)
				CheckStmt(s);
			IsPreCompiling = true;
			foreach (var s in Assembly.Body)
				CheckStmt(s);
			IsPreCompiling = false;
		}
		public static void Check(Stmt_Assembly asm, ParserContext context)
		{
			StaticChecker sc = new StaticChecker(asm, context);
			sc.CheckAll();
		}
	}
}
