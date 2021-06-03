using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	public class FlowOptimizer : BaseOptimizer
	{
		private List<ILInstruction> HeaderILs
		{
			get;
		}
		/// <summary>
		/// Including entry and exit
		/// </summary>
		private LinkedList<BasicBlock> Blocks
		{
			get;
		}
		private Dictionary<ILInstruction, BasicBlock> HeaderToBlock
		{
			get;
		}
		private BasicBlock Entry
		{
			get;
		}
		private BasicBlock Exit
		{
			get;
		}
		/// <summary>
		/// for constant accesssing time
		/// </summary>
		private HashSet<BasicBlock> Accessible
		{
			get;
		}
		public FlowOptimizer(OptimizerContext ctx) : base(ctx)
		{
			HeaderILs = new List<ILInstruction>();
			Blocks = new LinkedList<BasicBlock>();
			HeaderToBlock = new Dictionary<ILInstruction, BasicBlock>();
			Entry = new BasicBlock();
			Exit = new BasicBlock();
			Accessible = new HashSet<BasicBlock>();
		}
		private BasicBlock NewBlock()
		{
			BasicBlock b = new BasicBlock();
			Blocks.AddLast(b);
			return b;
		}


		private static bool IsJmps(ILType type)
		{
			return type >= ILType.Jmp && type <= ILType.Jse;
		}

		private void FindHeaderILs()
		{
			LinkedListNode<ILInstruction> s = SourceBind.Instructions.First;
			while (s != null)
			{
				var il = s.Value;
				if (IsJmps(il.Type) || il.Type == ILType.End)
				{
					HeaderILs.Add(il.Target as ILInstruction);
					if (s.Next != null)
						HeaderILs.Add(s.Next.Value);
				}
				s = s.Next;
			}
		}

		private static void ConnectStraight(BasicBlock previous, BasicBlock next)
		{
			next.Previous.Add(previous);
			previous.NextStraight = next;
		}
		private static void ConnectConditioned(BasicBlock previous, BasicBlock next)
		{
			next.Previous.Add(previous);
			previous.NextConditioned = next;
		}

		private void BuildControlFlowBlocks()
		{
			Blocks.Clear();
			Blocks.AddLast(Entry);
			//build line
			BasicBlock current = NewBlock();
			ConnectStraight(Entry, current);
			LinkedListNode<ILInstruction> s = SourceBind.Instructions.First;
			while (s != null)
			{
				if (HeaderILs.Contains(s.Value))
				{
					BasicBlock t = HeaderToBlock[current.Instructions.First.Value] = current;
					current = NewBlock();
					ConnectStraight(t, current);
				}
				current.Instructions.AddLast(s.Value);
				s = s.Next;
			}
			ConnectStraight(current, Exit);
			HeaderToBlock[current.Instructions.First.Value] = current;
			Blocks.AddLast(Exit);
		}
		private void BuildControlFlowJmps()
		{
			foreach (var y in Blocks)
			{
				if (y.Instructions.Count == 0)//signs
					continue;
				var last = y.Instructions.Last;
				if (last.Value.Type == ILType.End)//directly jump to exit
				{
					ConnectStraight(y, Exit);
					continue;
				}
				if (!IsJmps(last.Value.Type))
					continue;
				if (last.Value.Type == ILType.Jmp)
				{
					var t = HeaderToBlock[last.Value.Target as ILInstruction];
					if (t == y.NextStraight)//redundant jmp
						y.Instructions.Remove(last);//remove
					else//always jump
						ConnectStraight(y, t);//change
				}
				else
				{
					BasicBlock t = HeaderToBlock[last.Value.Target as ILInstruction];
					ConnectConditioned(y, t);
					if (y.NextConditioned == y.NextStraight)//only one exit
					{
						y.Instructions.Remove(last);//remove the jmp
						continue;
					}
				}
			}
		}



		private void BuildControlFlow()
		{
			FindHeaderILs();
			BuildControlFlowBlocks();
			BuildControlFlowJmps();
		}

		private void SetAccessible(BasicBlock block)
		{
			if (block == null || Accessible.Contains(block))
				return;
			Accessible.Add(block);
			SetAccessible(block.NextConditioned);
			SetAccessible(block.NextStraight);
		}

		/// <summary>
		/// Removing the inaccessible blocks
		/// O(n)
		/// </summary>
		private void RemoveInaccessibleBlocks()
		{
			SetAccessible(Entry);
			var ina = Blocks.Where(t => !Accessible.Contains(t));
			foreach (var block in ina)
			{
				Blocks.Remove(block);//O(1)
			}
		}

		public override void Run()
		{
			Log("Running flow optimization...");
			if (SourceBind.Instructions.Count == 0)
				return;
			BuildControlFlow();
			RemoveInaccessibleBlocks();


			//rebuild body
			SourceBind.Instructions.Clear();
			foreach (var block in Blocks)
			{
				foreach (var il in block.Instructions)
					SourceBind.Instructions.AddLast(il);
			}

		}
	}
}
