using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL.Optimization
{
	public static class ILOperandCloningHelper
	{
		/// <summary>
		/// Clone from an operand
		/// Instruction operand will be marked in a map and itself will be returned
		/// </summary>
		/// <param name="v"></param>
		/// <param name="varMap"></param>
		/// <returns></returns>
		private static ILOperand CloneOprand(ILOperand v, Dictionary<ILVariable, ILVariable> varMap)//O(1)
		{
			if (v is ILConst ilc)
				return new ILConst(ilc.SourcePosition) { Value = ilc.Value };
			else if (v is ILInstruction ilins)
				return ilins;//temporarily
			else if (v is ILMacro ilm)//macro ref
				return ilm;
			else if (v is ILComponent ilcomp)
				return new ILComponent(ilcomp.SourcePosition) { Name = ilcomp.Name };
			else if (v is ILAttribute ilattr)
				return new ILAttribute(ilattr.SourcePosition) { Name = ilattr.Name };
			else if (v is ILResource ilr)
				return new ILResource(ilr.SourcePosition) { Name = ilr.Name };
			else if (v is ILOperator ilOptr)
				return new ILOperator(ilOptr.SourcePosition) { Type = ilOptr.Type };
			else if (v is ILVariable ilv)
			{
				if (varMap.ContainsKey(ilv))
					return varMap[ilv];
				return (varMap[ilv] = new ILVariable(ilv.SourcePosition));//keep the same name
			}
			return null;
		}
		/// <summary>
		/// Completely clone instructions
		/// Will only append at the last of dst
		/// </summary>
		/// <param name="src">instructions to clone from</param>
		/// <param name="dst">to save the result</param>
		/// <param name="variableMap">variables mapped from clonee's to result's</param>
		public static void CloneTo(this ILOperandCollection<ILInstruction> src, ILOperandCollection<ILInstruction> dst, out Dictionary<ILVariable, ILVariable> variableMap)//O(n)
		{
			Dictionary<ILVariable, ILVariable> VarMap = new Dictionary<ILVariable, ILVariable>();
			variableMap = VarMap;
			Dictionary<ILInstruction, ILInstruction> ILMap = new Dictionary<ILInstruction, ILInstruction>();
			foreach (var il in src)
			{
				dst.AddLast(ILMap[il] =
					new ILInstruction(il.SourcePosition, il.Type,
					CloneOprand(il.Target, VarMap),
					CloneOprand(il.Arg1, VarMap),
					CloneOprand(il.Arg2, VarMap),
					CloneOprand(il.Arg3, VarMap)));
			}
			foreach (var il in dst)//reset pointers
			{
				if (il.Target is ILInstruction ilins)//jmp/return inst
					il.Target = ILMap[ilins];
			}
		}

		/// <summary>
		/// Completely clone from a macro with nothing changed
		/// </summary>
		/// <param name="macro">macro to clone</param>
		/// <returns></returns>
		public static ILMacro Clone(this ILMacro macro)//O(n)
		{
			ILMacro target = new ILMacro(macro.SourcePosition)
			{
				Name = macro.Name
			};
			macro.Instructions.CloneTo(target.Instructions, out Dictionary<ILVariable, ILVariable> vMap);
			foreach (var p in macro.Params)
				target.Params.AddLast(vMap[p]);
			target.ReturnValue = CloneOprand(macro.ReturnValue, vMap) as ILVariable;
			return target;
		}
	}
}
