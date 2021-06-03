using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	/// <summary>
	/// Expanding all macro call
	/// Instructions typed param will also be removed
	/// If the optimizee is an ILAssembly, macros in its default namespace will be optimized first
	/// Time complexity: O(n)
	/// </summary>
	public class ExpandingOptimizer : BaseOptimizer
	{
		public ExpandingOptimizer(OptimizerContext ctx) : base(ctx)
		{
		}

		public override void Run()
		{
			Log("Running expanding optimization...");
			if (SourceBind is ILAssembly asm)
				foreach (var macro in asm.Macros)
					AssembledOptimizer.GetDefaultOptimizer(new OptimizerContext(macro, Context.CompilerContext)).Run();//After which the macro get expanded and optmized

			Dictionary<ILInstruction, List<ILInstruction>> ILsToRep = new Dictionary<ILInstruction, List<ILInstruction>>();
			ILOperandCollection<ILInstruction> Params = new ILOperandCollection<ILInstruction>();
			//begin to expand the calls
			foreach (var il in SourceBind.Instructions)
			{
				if (il.Type == ILType.Param)
				{
					Params.AddLast(il);
				}
				else if (il.Type == ILType.Call)//call
				{
					ILsToRep[il] = new List<ILInstruction>();
					ILVariable retV = il.Target as ILVariable;//variable to save the return value
					ILMacro macro = (il.Arg1 as ILMacro).Clone();//clone a macro to call
					var ps = Params.Zip(macro.Params);
					foreach (var (First, Second) in ps)//param v -> set argv = v
					{
						First.Type = ILType.Set;
						First.Arg1 = First.Target;
						First.Target = Second;
					}
					Params.Clear();
					ILsToRep[il].AddRange(macro.Instructions);
					ILsToRep[il].Add(new ILInstruction(il.SourcePosition, ILType.Set, retV, macro.ReturnValue));//set return value
				}
				else if (il.Type == ILType.ASMCall)//call
				{
					Params.Clear();
				}
			}
			//replace ils
			foreach (var (il2r, insts) in ILsToRep)
			{
				var s = SourceBind.Instructions.Find(il2r);
				var cr = s;
				foreach (var inst in insts)
					cr = SourceBind.Instructions.AddAfter(cr, inst);
				SourceBind.Instructions.Remove(s);
			}
		}
	}
}
