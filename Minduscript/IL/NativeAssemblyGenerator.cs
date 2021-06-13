using Minduscript.Assembly;
using Minduscript.Assembly.Instructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class NativeAssemblyGenerator
	{
		private int VarIndex
		{
			get;
			set;
		}
		private Assembler Assembler
		{
			get;
		}
		private IReadOnlyCollection<ILInstruction> Instructions
		{
			get;
		}
		private Dictionary<ILVariable, ParamVariable> Variables
		{
			get;
		}
		private Dictionary<ILInstruction, List<Instruction>> Jumps
		{
			get;
		}
		private Dictionary<ILInstruction, int> InstMap
		{
			get;
		}
		private NativeAssemblyGenerator(IReadOnlyCollection<ILInstruction> insts)
		{
			Instructions = insts;
			Assembler = new Assembler();
			Variables = new Dictionary<ILVariable, ParamVariable>();
			Jumps = new Dictionary<ILInstruction, List<Instruction>>();
			InstMap = new Dictionary<ILInstruction, int>();
		}
		private static Operators GetOptr(ILOperator optr)
		{
			return optr.Type switch
			{
				OperatorType.Add => Operators.add,
				OperatorType.Sub => Operators.sub,
				OperatorType.Mul => Operators.mul,
				OperatorType.Div => Operators.div,
				OperatorType.IDiv => Operators.idiv,
				OperatorType.Mod => Operators.mod,
				OperatorType.Equal => Operators.equal,
				OperatorType.NotEqual => Operators.notEqual,
				OperatorType.StrictEqual => Operators.strictEqual,
				OperatorType.Land => Operators.land,
				OperatorType.LessThan => Operators.lessThan,
				OperatorType.LessThanEq => Operators.lessThanEq,
				OperatorType.GreaterThan => Operators.greaterThan,
				OperatorType.GreaterThanEq => Operators.greaterThanEq,
				OperatorType.Shl => Operators.shl,
				OperatorType.Shr => Operators.shr,
				OperatorType.Or => Operators.or,
				OperatorType.And => Operators.and,
				OperatorType.Xor => Operators.xor,
				OperatorType.Flip => Operators.flip,
				_ => throw new AssemblerException("Undefined operator:"),
			};
		}
		private List<Instruction> GetJmps(ILInstruction v)
		{
			if (Jumps.ContainsKey(v))
				return Jumps[v];
			return (Jumps[v] = new List<Instruction>());
		}
		private ParamVariable GetVar(ILVariable v)
		{
			if (Variables.ContainsKey(v))
				return Variables[v];
			return (Variables[v] = Assembler[$"var_{VarIndex++}"]);
		}
		private ParamEvaluable GetEvaluable(ILValue operand)
		{
			if (operand is ILConst ilc)
			{
				if (ilc.Value is string s)
					return new ParamString(s);
				else
					return new ParamConstant((float)ilc.Value);
			}
			else if (operand is ILVariable ilv)
			{
				return GetVar(ilv);
			}
			return null;
		}
		private ParamResource GetResource(ILGameConst operand)
		{
			return new ParamResource(operand.Name);
		}
		private T GetAttribute<T>(ILGameConst c) where T : struct
		{
			return Enum.Parse<T>(c.Name.ToString());
		}

		private Instruction ASMCall(string name, List<ILInstruction> ps)
		{
			return name switch
			{
				"read" => new Read(GetVar(
					ps[0].Target as ILVariable),
					GetEvaluable(ps[1].Target as ILValue),
					GetEvaluable(ps[2].Target as ILValue)),
				"write" => new Read(
					GetEvaluable(ps[0].Target as ILValue),
					GetEvaluable(ps[1].Target as ILValue),
					GetEvaluable(ps[2].Target as ILValue)),
				"draw" => new Draw(
					GetAttribute<DrawModes>(ps[0].Target as ILGameConst),
					GetEvaluable(ps[1].Target as ILValue),
					GetEvaluable(ps[2].Target as ILValue),
					GetEvaluable(ps[3].Target as ILValue),
					GetEvaluable(ps[4].Target as ILValue),
					GetEvaluable(ps[5].Target as ILValue),
					GetEvaluable(ps[6].Target as ILValue)),
				"print" => new Print(GetEvaluable(ps[0].Target as ILValue)),
				"drawflush" => new DrawFlush(GetEvaluable(ps[0].Target as ILValue)),
				"printflush" => new PrintFlush(GetEvaluable(ps[0].Target as ILValue)),
				"getlink" => new GetLink(GetVar(ps[0].Target as ILVariable), GetEvaluable(ps[1].Target as ILValue)),
				"control" => new Control(
					GetAttribute<ControlModes>(ps[0].Target as ILGameConst),
					GetEvaluable(ps[1].Target as ILValue),
					GetEvaluable(ps[2].Target as ILValue),
					GetEvaluable(ps[3].Target as ILValue),
					GetEvaluable(ps[4].Target as ILValue),
					GetEvaluable(ps[5].Target as ILValue)),
				"radar" => new Radar(
					GetAttribute<UnitAttributes>(ps[0].Target as ILGameConst),
					GetAttribute<UnitAttributes>(ps[1].Target as ILGameConst),
					GetAttribute<UnitAttributes>(ps[2].Target as ILGameConst),
					GetAttribute<UnitSortAccordance>(ps[3].Target as ILGameConst),
					GetEvaluable(ps[4].Target as ILValue),
					GetEvaluable(ps[5].Target as ILValue),
					GetVar(ps[6].Target as ILVariable)),
				"sensor" => new Sensor(
					GetVar(ps[0].Target as ILVariable),
					GetEvaluable(ps[1].Target as ILValue),
					GetResource(ps[2].Target as ILGameConst)),
				"set" => new Set(
					GetVar(ps[0].Target as ILVariable),
					GetEvaluable(ps[1].Target as ILValue)),
				"op" => new Op(
					GetAttribute<Operators>(ps[0].Target as ILGameConst),
					GetVar(ps[1].Target as ILVariable),
					GetEvaluable(ps[2].Target as ILValue),
					GetEvaluable(ps[3].Target as ILValue)),
				"end" => new End(),
				"ubind" => new UBind(
					GetResource(ps[0].Target as ILGameConst)),
				"ucontrol" => new UControl(
					GetAttribute<UControlModes>(ps[0].Target as ILGameConst),
					GetVar(ps[1].Target as ILVariable),
					GetEvaluable(ps[2].Target as ILValue),
					GetEvaluable(ps[3].Target as ILValue),
					GetEvaluable(ps[4].Target as ILValue),
					GetEvaluable(ps[5].Target as ILValue)),
				"uradar" => new URadar(
					GetAttribute<UnitAttributes>(ps[0].Target as ILGameConst),
					GetAttribute<UnitAttributes>(ps[1].Target as ILGameConst),
					GetAttribute<UnitAttributes>(ps[2].Target as ILGameConst),
					GetAttribute<UnitSortAccordance>(ps[3].Target as ILGameConst),
					GetEvaluable(ps[4].Target as ILValue),
					GetEvaluable(ps[5].Target as ILValue),
					GetVar(ps[6].Target as ILVariable)),
				"ulocate" => new ULocate(
					GetAttribute<TileTypes>(ps[0].Target as ILGameConst),
					GetAttribute<BuildingGroups>(ps[1].Target as ILGameConst),
					GetEvaluable(ps[2].Target as ILValue),
					GetResource(ps[3].Target as ILGameConst),
					GetVar(ps[4].Target as ILVariable),
					GetVar(ps[5].Target as ILVariable),
					GetVar(ps[6].Target as ILVariable),
					GetVar(ps[7].Target as ILVariable)),
				_ => null,
			};
		}
		public void CompileToASM(TextWriter tw)
		{
			ILOperandCollection<ILInstruction> Params = new ILOperandCollection<ILInstruction>();
			ILOperandCollection<ILInstruction> P2D = new ILOperandCollection<ILInstruction>();
			foreach (var il in Instructions)
			{
				InstMap.Add(il, Assembler.CurrentInstructionIndex);
				switch (il.Type)
				{
					case ILType.Nop:
						//jump - 1 notEqual x false
						Assembler.Assemble(new Jump(-1, Conditions.notEqual, 0, 0));
						break;
					case ILType.ASMCall:
						Assembler.Assemble(ASMCall((il.Target as ILConst).Value.ToString(), Params.ToList()));
						Params.Clear();
						break;
					case ILType.Using:
						Assembler.Assemble(new Set(GetVar(il.Target as ILVariable), Assembler[(il.Arg1 as ILGameConst).Name]));
						break;
					case ILType.Set:
						Assembler.Assemble(new Set(GetVar(il.Target as ILVariable), GetEvaluable(il.Arg1 as ILValue)));
						break;
					case ILType.Assignment:
						Assembler.Assemble(
							new Op(GetOptr(il.Arg3 as ILOperator),
							GetVar(il.Target as ILVariable),
							GetEvaluable(il.Arg1 as ILValue),
							GetEvaluable(il.Arg2 as ILValue)));
						break;
					case ILType.Mem_Read:
						Assembler.Assemble(new Read(GetVar(il.Target as ILVariable), GetVar(il.Arg1 as ILVariable), GetEvaluable(il.Arg2 as ILValue)));
						break;
					case ILType.Mem_Write:
						Assembler.Assemble(new Write(GetEvaluable(il.Target as ILValue), GetVar(il.Arg1 as ILVariable), GetEvaluable(il.Arg2 as ILValue)));
						break;
					case ILType.End:
						Assembler.Assemble(new End());
						break;
					case ILType.Jmp:
						GetJmps(il.Target as ILInstruction).
							Add(Assembler.Assemble(
								new Jump(null as ParamJmpTarget, Conditions.always, 0, 0)
								).Value);
						break;
					case ILType.Je:
						GetJmps(il.Target as ILInstruction).
							Add(Assembler.Assemble(
								new Jump(null as ParamJmpTarget, Conditions.equal, GetEvaluable(il.Arg1 as ILValue), GetEvaluable(il.Arg2 as ILValue))
								).Value);
						break;
					case ILType.Jne:
						GetJmps(il.Target as ILInstruction).
							Add(Assembler.Assemble(
								new Jump(null as ParamJmpTarget, Conditions.notEqual, GetEvaluable(il.Arg1 as ILValue), GetEvaluable(il.Arg2 as ILValue))
								).Value);
						break;
					case ILType.Jl:
						GetJmps(il.Target as ILInstruction).
							Add(Assembler.Assemble(
								new Jump(null as ParamJmpTarget, Conditions.lessThan, GetEvaluable(il.Arg1 as ILValue), GetEvaluable(il.Arg2 as ILValue))
								).Value);
						break;
					case ILType.Jle:
						GetJmps(il.Target as ILInstruction).
							Add(Assembler.Assemble(
								new Jump(null as ParamJmpTarget, Conditions.lessThanEq, GetEvaluable(il.Arg1 as ILValue), GetEvaluable(il.Arg2 as ILValue))
								).Value);
						break;
					case ILType.Jg:
						GetJmps(il.Target as ILInstruction).
							Add(Assembler.Assemble(
								new Jump(null as ParamJmpTarget, Conditions.greaterThan, GetEvaluable(il.Arg1 as ILValue), GetEvaluable(il.Arg2 as ILValue))
								).Value);
						break;
					case ILType.Jge:
						GetJmps(il.Target as ILInstruction).
							Add(Assembler.Assemble(
								new Jump(null as ParamJmpTarget, Conditions.greaterThanEq, GetEvaluable(il.Arg1 as ILValue), GetEvaluable(il.Arg2 as ILValue))
								).Value);
						break;
					case ILType.Jse:
						GetJmps(il.Target as ILInstruction).
							Add(Assembler.Assemble(
								new Jump(null as ParamJmpTarget, Conditions.strictEqual, GetEvaluable(il.Arg1 as ILValue), GetEvaluable(il.Arg2 as ILValue))
								).Value);
						break;
					case ILType.Param:
						Params.AddLast(il);
						P2D.AddLast(il);
						break;
					case ILType.Call:
						Params.Clear();
						break;
					default:
						throw new AssemblerException("Instructions of param and call cannot be passed to final assembler");
				}
			}
			foreach (var (il, j) in Jumps)
			{
				foreach (var rc in j)
				{
					var t = rc as Jump;
					t.Params[0] = InstMap[il];
				}
			}
			Assembler.AssembleToASMCode(tw);
			tw.Flush();
		}
		public static void Generate(ILAssembly asm, TextWriter tw)
		{
			new NativeAssemblyGenerator(asm.Instructions).CompileToASM(tw);
		}
	}
}
