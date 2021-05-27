using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL.Optimization
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
			HashSet<LinkedListNode<ILInstruction>> nops = new HashSet<LinkedListNode<ILInstruction>>();
			while (current != null)
			{
				if(current.Value.Type== ILType.Nop)
				{
					nops.Add(current);
				}
				if (current.Value.Target is ILInstruction ili)
				{
					LinkedListNode<ILInstruction> t = SourceBind.Instructions.Find(ili);
					if (t.Value.Type != ILType.Nop || t.Next == null)
						continue;
					current.Value.Target = t.Next.Value;
				}
				current = current.Next;
			}
			nops.Remove(SourceBind.Instructions.Last);
			foreach (var node in nops)
			{
				SourceBind.Instructions.Remove(node);
			}
		}
	}
}
