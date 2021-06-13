using Minduscript.CompilingInfo;
using Minduscript.IL;
using Minduscript.Parse.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse
{
	public class ILAssemblyGenerator
	{
		private Stmt_Assembly MSAssembly
		{
			get;
			set;
		}
		private ParserContext Context
		{
			get;
		}
		private ILOperandCollection<ILInstruction> ILInstructions
		{
			get;
			set;
		}
		private string CurrentFunction
		{
			get;
			set;
		}
		private ILAssembly Target
		{
			get;
		}
		private Dictionary<int, ILVariable> GlobalVariables
		{
			get;
		}
		private Dictionary<int, ILVariable> LocalVariables
		{
			get;
			set;
		}
		private Dictionary<int, ILVariable> TempVariables
		{
			get;
			set;
		}
		private int TempVarIndex
		{
			get;
			set;
		}
		private Stack<int> TempVarStack
		{
			get;
		}
		private ILVariable GetNewTempVar(SourcePosition src)
		{
			if (TempVariables.ContainsKey(TempVarIndex))
				return TempVariables[TempVarIndex++];
			return (TempVariables[TempVarIndex++] = new ILVariable(src));
		}
		private void PushTempVar()
		{
			TempVarStack.Push(TempVarIndex);
		}
		private void PopTempVar()
		{
			TempVarIndex = TempVarStack.Pop();
		}
		private ILAssemblyGenerator(Stmt_Assembly assembly, ParserContext context)
		{
			MSAssembly = assembly;
			Context = context;
			Target = new ILAssembly(context.File, assembly.AssemblyName);
			GlobalVariables = new Dictionary<int, ILVariable>();
			TempVariables = new Dictionary<int, ILVariable>();
			ILInstructions = new ILOperandCollection<ILInstruction>();//default
			TempVarStack = new Stack<int>();
		}
		private ILVariable GetGlobalVariable(Expr_Variable ev)
		{
			if (GlobalVariables.ContainsKey(ev.Offset))
				return GlobalVariables[ev.Offset];
			ILVariable iv = new ILVariable(ev.SourcePosition);
			GlobalVariables[ev.Offset] = iv;
			return iv;
		}
		private ILVariable GetLocalVariable(Expr_Variable ev)
		{
			if (LocalVariables.ContainsKey(ev.Offset))
				return LocalVariables[ev.Offset];
			ILVariable iv = new ILVariable(ev.SourcePosition);
			LocalVariables[ev.Offset] = iv;
			return iv;
		}
		public ILFunction GenerateFunction(Stmt_Function sm)
		{
			CurrentFunction = sm.FunctionName;
			var tmpVariables = LocalVariables;
			var tmpTempVariables = TempVariables;
			var tmpInsts = ILInstructions;


			ILFunction m = new ILFunction(sm.SourcePosition);
			LocalVariables = new Dictionary<int, ILVariable>();
			TempVariables = new Dictionary<int, ILVariable>();
			ILInstructions = m.Instructions;

			m.Name = sm.FunctionName;
			m.ReturnValue = GetLocalVariable(sm.ReturnValue);
			foreach (var p in sm.Params)
			{
				var v = GetLocalVariable(p);
				LocalVariables[p.Offset] = v;
				m.Params.AddLast(v);
			}

			ILInstruction header = new ILInstruction(sm.SourcePosition, ILType.Nop);
			m.Instructions.AddLast(header);

			GenerateStatement(sm.Code);

			ILInstruction next = new ILInstruction(m.Instructions.Last().SourcePosition, ILType.Nop);
			m.Instructions.AddLast(next);

			BackPatch(sm.Entry.Header, header);
			BackPatch(sm.Entry.Next, next);

			ILInstructions = tmpInsts;
			TempVariables = tmpTempVariables;
			LocalVariables = tmpVariables;
			CurrentFunction = null;

			return m;
		}
		private ILValue GenerateExpressionR(Expression expr, ILVariable defaultTargetVar = null)
		{
			if (expr is Expr_ConstNumber ecn)
			{
				return CreateConst(ecn.Value, ecn.SourcePosition.Line);
			}
			else if (expr is Expr_ConstString ecs)
			{
				return CreateConst(ecs.Value, ecs.SourcePosition.Line);
			}
			else if (expr is Expr_Variable ev)
			{
				return CurrentFunction == null ? GetGlobalVariable(ev) : GetLocalVariable(ev);
			}
			else if (expr is Expr_Binary eb)
			{
				var ill = GenerateExpressionR(eb.Left);
				var ilr = GenerateExpressionR(eb.Right);
				ILVariable target = defaultTargetVar ?? GetNewTempVar(eb.SourcePosition);
				ILInstructions.AddLast(new ILInstruction(eb.SourcePosition, ILType.Assignment, target, ill, ilr, ILOperator.GetOPTRFromString(eb.SourcePosition, eb.Operator)));
				return target;
			}
			else if (expr is Expr_Mem em)
			{
				ILVariable target = defaultTargetVar ?? GetNewTempVar(em.SourcePosition);
				ILValue mem = GenerateExpressionR(em.Mem);
				ILValue pos = GenerateExpressionR(em.Pos);
				ILInstructions.AddLast(new ILInstruction(em.SourcePosition, ILType.Mem_Read, target, mem, pos));
				return target;
			}
			else if (expr is Expr_Call ec)
			{
				List<ILValue> ps = new List<ILValue>();
				foreach (var arg in ec.Args)
					ps.Add(GenerateExpressionR(arg));
				ILVariable retV = defaultTargetVar ?? GetNewTempVar(ec.SourcePosition);
				foreach (var p in ps)
					ILInstructions.AddLast(new ILInstruction(p.SourcePosition, ILType.Param, p));
				ILFunction func = null;
				if (ec.Namespace == null)
				{
					var ms = Target.Functions.Where(t => t.Name == ec.Function).ToList();
					if (ms.Count == 0)
					{
						Context.ThrowILGeneratingError(ec.SourcePosition, $"Function cannot be called before being defined:({ec.Function})");
						return CreateConst(0, ec.SourcePosition.Line);
					}
					func = ms[0];
				}
				else//a.b
				{
					if (!Target.Namespaces.ContainsKey(ec.Namespace))
					{
						Context.ThrowILGeneratingError(ec.SourcePosition, $"No such namespace:({ec.Namespace})");
						return CreateConst(0, ec.SourcePosition.Line);
					}
					var ms = Target.Namespaces[ec.Namespace].Functions.Where(t => t.Name == ec.Function).ToList();
					if (ms.Count == 0)
					{
						Context.ThrowILGeneratingError(ec.SourcePosition, $"No such function ({ec.Function}) in namespace:({ec.Namespace})");
						return CreateConst(0, ec.SourcePosition.Line);
					}
					func = ms[0];
				}
				if (ps.Count != func.Params.Count)
				{
					Context.ThrowILGeneratingError(ec.SourcePosition, $"Number of args do not matches, calling function ({ec.Function}), existed:{ps.Count}, required:{func.Params.Count}");
					return CreateConst(0, ec.SourcePosition.Line);
				}
				ILInstructions.AddLast(new ILInstruction(ec.SourcePosition, ILType.Call, retV, func));
				return retV;
			}
			return null;
		}
		private void GenerateExpressionL(Expression exprL, Expression exprR)
		{
			if (exprL is Expr_Variable ev)
			{
				ILVariable ilv = GenerateExpressionR(ev) as ILVariable;
				ILValue rv = GenerateExpressionR(exprR, ilv);
				if (ilv != rv)
					ILInstructions.AddLast(new ILInstruction(ev.SourcePosition, ILType.Set, ilv, rv));
			}
			else if (exprL is Expr_Mem em)
			{
				ILValue rv = GenerateExpressionR(exprR);
				ILValue mem = GenerateExpressionR(em.Mem);
				ILValue pos = GenerateExpressionR(em.Pos);
				ILInstructions.AddLast(new ILInstruction(em.SourcePosition, ILType.Mem_Write, rv, mem, pos));
			}
		}
		private void GenerateFunctions()
		{
			foreach (var s in MSAssembly.Functions)
			{
				ILFunction func = GenerateFunction(s);
				Target.Functions.Add(func);
				Context.CompilerContext.Log($"Function generation completed:({func.Name})");
			}
		}
		private ILConst CreateConst(object v, int line)
		{
			return new ILConst(new SourcePosition(Context.File, line)) { Value = v };
		}
		private static void BackPatch(HashSet<ILInstruction> a, ILInstruction target)
		{
			foreach (var e in a)
			{
				e.Target = target;
			}
		}
		private EntryContext GenerateBooleanCondition(Expression expr)
		{
			EntryContext Entry = new EntryContext();
			if (expr is Expr_Unitary eu)
			{
				if (eu.Operator == "!")
				{
					EntryContext c = GenerateBooleanCondition(eu.Value);
					EntryContext.MergeSet(ref Entry.ETrue, ref c.EFalse);
					EntryContext.MergeSet(ref Entry.EFalse, ref c.ETrue);
				}
			}
			else if (expr is Expr_Binary eb && (eb.Operator == "||" || eb.Operator == "&&"))
			{
				switch (eb.Operator)
				{
					case "||":
						{
							EntryContext cl = GenerateBooleanCondition(eb.Left);
							ILInstruction jl = new ILInstruction(eb.SourcePosition, ILType.Nop);
							ILInstructions.AddLast(jl);
							EntryContext cr = GenerateBooleanCondition(eb.Right);
							BackPatch(cl.EFalse, jl);
							//true
							EntryContext.MergeSet(ref cl.ETrue, ref cr.ETrue);
							EntryContext.MergeSet(ref Entry.ETrue, ref cl.ETrue);
							//false
							EntryContext.MergeSet(ref Entry.EFalse, ref cr.EFalse);
						}
						break;
					case "&&":
						{
							EntryContext cl = GenerateBooleanCondition(eb.Left);
							ILInstruction jl = new ILInstruction(eb.SourcePosition, ILType.Nop);
							ILInstructions.AddLast(jl);
							EntryContext cr = GenerateBooleanCondition(eb.Right);
							BackPatch(cl.ETrue, jl);
							//false
							EntryContext.MergeSet(ref cl.EFalse, ref cr.EFalse);
							EntryContext.MergeSet(ref Entry.EFalse, ref cl.EFalse);
							//true
							EntryContext.MergeSet(ref Entry.ETrue, ref cr.ETrue);
						}
						break;
					default:
						break;
				}
			}
			else
			{
				if (expr is Expr_Binary eb1 &&
					(eb1.Operator == ">" || eb1.Operator == "<" ||
					eb1.Operator == ">=" || eb1.Operator == "<=" ||
					eb1.Operator == "==" || eb1.Operator == "!=" ||
					eb1.Operator == "==="))//comparator
				{
					ILValue left = GenerateExpressionR(eb1.Left);
					ILValue right = GenerateExpressionR(eb1.Right);
					ILInstruction jt = new ILInstruction(eb1.SourcePosition, GetJmpTypeFromOPTR(eb1.Operator), null, left, right);
					ILInstruction jf = new ILInstruction(eb1.SourcePosition, ILType.Jmp, null);
					Entry.ETrue.Add(jt);
					Entry.EFalse.Add(jf);
					ILInstructions.AddLast(jt);
					ILInstructions.AddLast(jf);
				}
				else
				{
					ILValue ilv = GenerateExpressionR(expr);
					ILInstruction jt = new ILInstruction(expr.SourcePosition, ILType.Je, null, ilv, CreateConst(1, expr.SourcePosition.Line));
					ILInstruction jf = new ILInstruction(expr.SourcePosition, ILType.Jmp, null);
					Entry.ETrue.Add(jt);
					Entry.EFalse.Add(jf);
					ILInstructions.AddLast(jt);
					ILInstructions.AddLast(jf);
				}
			}
			return Entry;
		}
		private static ILType GetJmpTypeFromOPTR(string optr)
		{
			return optr switch
			{
				">" => ILType.Jg,
				">=" => ILType.Jge,
				"<" => ILType.Jl,
				"<=" => ILType.Jle,
				"==" => ILType.Je,
				"!=" => ILType.Jne,
				"===" => ILType.Jse,
				_ => ILType.Jmp,
			};
		}
		private void GenerateStatement(Statement stmt)
		{
			PushTempVar();
			if (stmt is Stmt_Block sb)
			{
				foreach (var s in sb.Statements)
					GenerateStatement(s);
			}
			else if (stmt is Stmt_Call sc)
			{
				GenerateExpressionR(sc.Call);
			}
			else if (stmt is Stmt_Using su)
			{
				ILInstructions.AddLast(
					new ILInstruction(su.SourcePosition, ILType.Using,
					GenerateExpressionR(su.Target),
					new ILGameConst(su.Resource.SourcePosition) { Name = su.Resource.Name }));
			}
			else if (stmt is Stmt_Var sv)
			{
				foreach (var dc in sv.Declarations)
				{
					if (dc.Value == null)
						continue;
					GenerateExpressionL(dc.Key, dc.Value);
				}
			}
			else if (stmt is Stmt_Return sr)
			{
				if (sr.ReturnValue != null)
					GenerateExpressionL(sr.Function.ReturnValue, sr.ReturnValue);
				ILInstruction ret = new ILInstruction(sr.SourcePosition, ILType.Jmp, null);
				ILInstructions.AddLast(ret);
				(sr.Function == null ? MSAssembly.Entry : sr.Function.Entry).Next.Add(ret);
			}
			else if (stmt is Stmt_Assignment sa)
			{
				GenerateExpressionL(sa.Target, sa.Value);
			}
			else if (stmt is Stmt_If sif)
			{
				ILInstruction header = new ILInstruction(sif.SourcePosition, ILType.Nop);
				ILInstructions.AddLast(header);

				{
					EntryContext ec = GenerateBooleanCondition(sif.Condition);
					EntryContext.Merge(sif.Entry, ec);
				}

				ILInstruction jt = new ILInstruction(sif.SourcePosition, ILType.Nop);
				ILInstructions.AddLast(jt);

				GenerateStatement(sif.ContentCode);//content

				ILInstruction jn = new ILInstruction(sif.SourcePosition, ILType.Jmp);
				ILInstructions.AddLast(jn);
				sif.Entry.Next.Add(jn);

				ILInstruction jf = new ILInstruction(sif.SourcePosition, ILType.Nop);
				ILInstructions.AddLast(jf);

				if (sif.Else != null)
					GenerateStatement(sif.Else);//else
				ILInstruction next = new ILInstruction(sif.SourcePosition, ILType.Nop);
				ILInstructions.AddLast(next);

				BackPatch(sif.Entry.ETrue, jt);
				BackPatch(sif.Entry.EFalse, jf);
				BackPatch(sif.Entry.Header, header);
				BackPatch(sif.Entry.Next, next);
			}
			else if (stmt is Stmt_While sw)
			{
				ILInstruction header = new ILInstruction(sw.SourcePosition, ILType.Nop);
				ILInstructions.AddLast(header);

				{
					EntryContext ec = GenerateBooleanCondition(sw.Condition);
					EntryContext.Merge(sw.Entry, ec);
				}
				ILInstruction jt = new ILInstruction(sw.SourcePosition, ILType.Nop);
				ILInstructions.AddLast(jt);

				GenerateStatement(sw.Code);//content

				ILInstruction jn = new ILInstruction(sw.SourcePosition, ILType.Jmp, header);//go to the header
				ILInstructions.AddLast(jn);

				ILInstruction next = new ILInstruction(sw.SourcePosition, ILType.Nop);
				ILInstructions.AddLast(next);

				EntryContext.MergeSet(ref sw.Entry.EFalse, ref sw.Entry.Next);
				BackPatch(sw.Entry.ETrue, jt);
				BackPatch(sw.Entry.Next, next);
				BackPatch(sw.Entry.Header, header);
			}
			else if (stmt is Stmt_Break sb1)
			{
				ILInstruction jn = new ILInstruction(sb1.SourcePosition, ILType.Jmp);//go to the next
				ILInstructions.AddLast(jn);
				sb1.Loop.Entry.Next.Add(jn);
			}
			else if (stmt is Stmt_Continue sc1)
			{
				ILInstruction jn = new ILInstruction(sc1.SourcePosition, ILType.Jmp);//go to the header
				ILInstructions.AddLast(jn);
				sc1.Loop.Entry.Header.Add(jn);
			}
			else if (stmt is Stmt_ASMCall sac)
			{
				List<ILOperand> ps = new List<ILOperand>();
				foreach (var arg in sac.Args)
				{
					if (arg is Expr_GameConst egc)
					{
						ps.Add(new ILGameConst(egc.SourcePosition) { Name = egc.Content });
					}
					else
					{
						ps.Add(GenerateExpressionR(arg));
					}
				}
				foreach (var p in ps)
					ILInstructions.AddLast(new ILInstruction(p.SourcePosition, ILType.Param, p));
				ILInstructions.AddLast(new ILInstruction(sac.SourcePosition, ILType.ASMCall, new ILConst(new SourcePosition()) { Value = sac.Function }));
			}
			PopTempVar();
		}
		private void GenerateImports()
		{
			foreach (var s in MSAssembly.Body)
			{
				if (s is Stmt_Import si)
				{
					string file = si.ImportFile;

					if (si.Namespace != null)
						Context.CompilerContext.Log($"Importing from {file} to namespace:({si.Namespace})");
					else
						Context.CompilerContext.Log($"Importing from {file}");
					if (!File.Exists(file))
					{
						Context.ThrowILGeneratingError(si.SourcePosition, $"No such file:({file})");
						continue;//critical error
					}
					ILAssembly asm = Context.CompilerContext.GetAssembly(file);
					string ns = si.Namespace ?? asm.Name;
					if (Target.Namespaces.ContainsKey(ns))
					{
						Context.ThrowILGeneratingError(si.SourcePosition, $"Namespace duplicated:({ns})");
						continue;//critical error
					}
					Target.Namespaces[ns] = asm;
				}
			}
		}
		private void GenerateAssemblyBody()
		{
			var header = new ILInstruction(MSAssembly.SourcePosition, ILType.Nop);
			ILInstructions.AddLast(header);

			foreach (var s in MSAssembly.Body)
				GenerateStatement(s);

			var next = new ILInstruction(MSAssembly.SourcePosition, ILType.Nop);
			ILInstructions.AddLast(next);

			BackPatch(MSAssembly.Entry.Header, header);
			BackPatch(MSAssembly.Entry.Next, next);
		}
		private ILAssembly Generate()
		{
			Context.CompilerContext.Log($"Begin to generate ILAssembly from file:（{Target.File}）");
			Context.CompilerContext.Log("Importing libs...");
			GenerateImports();
			Context.CompilerContext.Log("Generating functions...");
			GenerateFunctions();
			Context.CompilerContext.Log("Checking functions...");
			Context.CompilerContext.Log("Generating ILBody...");
			GenerateAssemblyBody();
			foreach (var inst in ILInstructions)
				Target.Instructions.AddLast(inst);
			Context.CompilerContext.Log($"ILAssembly generation completed:({Target.File})");
			return Target;
		}
		public static ILAssembly Generate(Stmt_Assembly assembly, ParserContext context)
		{
			ILAssemblyGenerator ilg = new ILAssemblyGenerator(assembly, context);
			return ilg.Generate();
		}
	}
}
