using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public static class ILOutputHelper
	{
		public static void Output(this ILAssembly asm, StreamWriter sw)
		{
			new ILOutputer(sw).OutputASM(asm);
		}


		private class ILExecutionContext
		{
			public ILFunction Executable
			{
				get;
				set;
			}
			public int TabsCount
			{
				get;
				set;
			}
			private Dictionary<ILVariable, string> Vars
			{
				get;
			}
			private int VarIndex
			{
				get;
				set;
			}
			public string this[ILVariable v]
			{
				get
				{
					if (Vars.ContainsKey(v))
						return Vars[v];
					return (Vars[v] = $"var_{VarIndex++}");
				}
			}
			public ILExecutionContext(ILFunction func, int tabsCount)
			{
				Executable = func;
				TabsCount = tabsCount;
				Vars = new Dictionary<ILVariable, string>();
				VarIndex = 0;
			}
		}
		private class ILOutputer
		{
			private Stack<ILExecutionContext> Contexts
			{
				get;
			}
			private ILExecutionContext Context
			{
				get => Contexts.Peek();
			}
			private StreamWriter Out
			{
				get;
			}
			public ILOutputer(StreamWriter sw)
			{
				Out = sw;
				Contexts = new Stack<ILExecutionContext>();
			}
			private void PushContext(ILFunction ile)
			{
				Contexts.Push(new ILExecutionContext(ile, Contexts.Count == 0 ? 0 : Context.TabsCount + 1));
			}
			private void PopContext()
			{
				Contexts.Pop();
			}
			public void OutputASM(ILAssembly asm)
			{
				foreach (var func in asm.Functions)
				{
					OutputFunction(func);
				}
			}
			public void OutputFunction(ILFunction func)
			{
				PushContext(func);
				OutputHeaderTabs(); Out.Write($"Function ({Context[func.ReturnValue]}) {func.Name}({string.Join(",", from v in func.Params select Context[v])}):");
				OutputHeaderTabs(); Out.WriteLine($"# At {func.SourcePosition}");
				Context.TabsCount++;
				OutputHeaderTabs(); Out.WriteLine("# Body:");
				OutputILs();
				PopContext();
			}
			private void OutputHeaderTabs()
			{
				for (int i = 0; i < Context.TabsCount; i++)
					Out.Write('\t');
			}
			private void OutputILs()
			{
				var ils = Context.Executable.Instructions;
				foreach (var il in ils)
				{
					OutputIL(il);
					Out.WriteLine();
				}
				Out.WriteLine();
			}
			private void GetOperand(ILOperand operand)
			{
				if (operand is ILVariable ilv)
					Out.Write(Context[ilv]);
				else if (operand is ILConst ilc)
				{
					if (ilc.Value is string)
						Out.Write($"\"{ilc.Value}\"");
					else
						Out.Write(ilc.Value.ToString());
				}
				else if (operand is ILInstruction ili)
					Out.Write(GetProcessedIndex(ili.GetHashCode()));
				else if (operand is ILFunction ilm)
					Out.Write(ilm.Name);
				else if (operand is ILGameConst ilgc)
					Out.Write(ilgc.Name);
				else if (operand is ILOperator ilo)
					Out.Write(ilo.Type.ToString());
				else if (operand is ILResource ilr)
					Out.Write(ilr.ToString());
			}
			private string GetProcessedIndex(int index)
			{
				if (Context.Executable is ILFunction)
					return string.Format("M{0:X8}", index);
				return string.Format("G{0:X8}", index);
			}
			private void OutputIL(ILInstruction il)
			{
				OutputHeaderTabs();
				Out.Write(GetProcessedIndex(il.GetHashCode()));
				Out.Write('\t');
				switch (il.Type)
				{
					case ILType.Nop:
						Out.Write("nop");
						break;
					case ILType.ASMCall:
						Out.Write("asmcall ");
						GetOperand(il.Target);
						break;
					case ILType.Using:
						Out.Write("using ");
						GetOperand(il.Target);
						Out.Write(" = ");
						GetOperand(il.Arg1);
						break;
					case ILType.Set:
						GetOperand(il.Target);
						Out.Write(" = ");
						GetOperand(il.Arg1);
						break;
					case ILType.Assignment:
						GetOperand(il.Target);
						Out.Write(" = ");
						GetOperand(il.Arg1);
						Out.Write(' ');
						GetOperand(il.Arg3);
						Out.Write(' ');
						GetOperand(il.Arg2);
						break;
					case ILType.Param:
						Out.Write("param ");
						GetOperand(il.Target);
						break;
					case ILType.Call:
						GetOperand(il.Target);
						Out.Write(" = ");
						Out.Write("call ");
						GetOperand(il.Arg1);
						Out.Write(' ');
						GetOperand(il.Arg2);
						break;
					case ILType.Mem_Read:
						GetOperand(il.Target);
						Out.Write(" = ");
						GetOperand(il.Arg1);
						Out.Write('[');
						GetOperand(il.Arg2);
						Out.Write(']');
						break;
					case ILType.Mem_Write:
						GetOperand(il.Arg1);
						Out.Write('[');
						GetOperand(il.Arg2);
						Out.Write(']');
						Out.Write(" = ");
						GetOperand(il.Target);
						break;
					case ILType.Jmp:
						Out.Write("jmp ");
						GetOperand(il.Target);
						break;
					case ILType.Je:
						Out.Write("jmp ");
						GetOperand(il.Target);
						Out.Write(" if ");
						GetOperand(il.Arg1);
						Out.Write(" == ");
						GetOperand(il.Arg2);
						break;
					case ILType.Jne:
						Out.Write("jmp ");
						GetOperand(il.Target);
						Out.Write(" if ");
						GetOperand(il.Arg1);
						Out.Write(" != ");
						GetOperand(il.Arg2);
						break;
					case ILType.Jl:
						Out.Write("jmp ");
						GetOperand(il.Target);
						Out.Write(" if ");
						GetOperand(il.Arg1);
						Out.Write(" < ");
						GetOperand(il.Arg2);
						break;
					case ILType.Jle:
						Out.Write("jmp ");
						GetOperand(il.Target);
						Out.Write(" if ");
						GetOperand(il.Arg1);
						Out.Write(" <= ");
						GetOperand(il.Arg2);
						break;
					case ILType.Jg:
						Out.Write("jmp ");
						GetOperand(il.Target);
						Out.Write(" if ");
						GetOperand(il.Arg1);
						Out.Write(" > ");
						GetOperand(il.Arg2);
						break;
					case ILType.Jge:
						Out.Write("jmp ");
						GetOperand(il.Target);
						Out.Write(" if ");
						GetOperand(il.Arg1);
						Out.Write(" >= ");
						GetOperand(il.Arg2);
						break;
					case ILType.Jse:
						Out.Write("jmp ");
						GetOperand(il.Target);
						Out.Write(" if ");
						GetOperand(il.Arg1);
						Out.Write(" === ");
						GetOperand(il.Arg2);
						break;
					default:
						break;
				}
			}
		}

	}
}
