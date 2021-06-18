using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	/// <summary>
	/// Redundency reducing optimizer
	/// Will delete and/or merge locally redundant instruction snippets
	/// Removing nops
	/// 
	/// </summary>
	public class RedundencyOptimizer : BaseOptimizer
	{
		public RedundencyOptimizer(OptimizerContext ctx) : base(ctx)
		{
		}

		public override void Run()
		{
			Log("Running redundency optimization...");
			LinkedListNode<ILInstruction> current = SourceBind.Instructions.First;
			Dictionary<ILInstruction, ILInstruction> nopToR = new Dictionary<ILInstruction, ILInstruction>();
			HashSet<LinkedListNode<ILInstruction>> nopToD = new HashSet<LinkedListNode<ILInstruction>>();
			while (current != null)
			{
				if (current.Value.Type == ILType.Nop)
				{
					LinkedListNode<ILInstruction> t = current;
					while (t.Next != null && t.Value.Type == ILType.Nop)
						t = t.Next;
					while (current != t)
					{
						nopToR[current.Value] = t.Value;//the inst to dock
						nopToD.Add(current);//will be deleted later
						current = current.Next;
					}
				}
				current = current.Next;
			}
			nopToR.Remove(SourceBind.Instructions.Last.Value);//will not delete the nop if it's the last inst

			LinkedListNode<ILInstruction> inst=SourceBind.Instructions.First;
			while (inst != null)
			{
				if (inst.Value.Target is ILInstruction ili && //is a jmp
					ili.Type == ILType.Nop && //jmp to a nop
					nopToR.TryGetValue(ili, out ILInstruction target)) //to replace
				{
					inst.Value.Target = target;
				}
				inst = inst.Next;
			}

			foreach (var nop in nopToD)
			{
				SourceBind.Instructions.Remove(nop);
			}
		}
	}
}
