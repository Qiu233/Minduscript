using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	/// <summary>
	/// Expanding all function calls
	/// Instructions typed param and ret will be removed
	/// Time complexity: O(n)
	/// </summary>
	public class ExpandingOptimizer : ILBaseOptimizer
	{
		public ExpandingOptimizer(ILOptimizerContext ctx) : base(ctx)
		{
		}

		public override void Run()
		{
			Log("Running expanding optimization...");

			Dictionary<ILInstruction, List<ILInstruction>> ILsToRep = new Dictionary<ILInstruction, List<ILInstruction>>();
			ILOperandCollection<ILInstruction> Params = new ILOperandCollection<ILInstruction>();
			//begin to expand the calls
			foreach (var il in SourceBind.Instructions)
			{
				if (il.Type == ILType.Param)
				{
					Params.AddLast(il);
				}
				else if (il.Type == ILType.ASMCall)//call
				{
					Params.Clear();
				}
				else if (il.Type == ILType.Call)//call
				{
					ILsToRep[il] = new List<ILInstruction>();
					ILVariable retV = il.Target as ILVariable;//variable to save the return value
					ILFunction function = (il.Arg1 as ILFunction).Clone();//clone a function to call
					var ps = Params.Zip(function.Params);
					foreach (var (First, Second) in ps)//param v -> set argv = v
					{
						First.Type = ILType.Set;
						First.Arg1 = First.Target;
						First.Target = Second;
					}
					Params.Clear();
					LinkedListNode<ILInstruction> last = function.Instructions.Last;
					if (last.Value.Type != ILType.Nop)
					{
						last = function.Instructions.AddLast(new ILInstruction(last.Value.SourcePosition, ILType.Nop));
					}
					foreach (var ins in function.Instructions)
					{
						ILsToRep[il].Add(ins);
						if (ins.Type == ILType.Ret)
						{
							ins.Type = ILType.Set;
							ins.Arg1 = ins.Target;
							ins.Target = retV;
							ILsToRep[il].Add(new ILInstruction(ins.SourcePosition, ILType.Jmp, last.Value));
						}
					}
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
