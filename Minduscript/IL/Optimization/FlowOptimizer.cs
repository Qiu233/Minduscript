using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL.Optimization
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
					BasicBlock t = HeaderToBlock[current.Instructions.First.Value.Bind] = current;
					current = NewBlock();
					ConnectStraight(t, current);
				}
				current.Instructions.AddLast(new DataFlowInstruction(s.Value));
				s = s.Next;
			}
			ConnectStraight(current, Exit);
			HeaderToBlock[current.Instructions.First.Value.Bind] = current;
			Blocks.AddLast(Exit);
		}
		private void BuildControlFlowJmps()
		{
			foreach (var y in Blocks)
			{
				if (y.Instructions.Count == 0)//signs
					continue;
				var last = y.Instructions.Last;
				if (last.Value.Bind.Type == ILType.End)//directly jump to exit
				{
					ConnectStraight(y, Exit);
					continue;
				}
				if (!IsJmps(last.Value.Bind.Type))
					continue;
				if (last.Value.Bind.Type == ILType.Jmp)
				{
					var t = HeaderToBlock[last.Value.Bind.Target as ILInstruction];
					if (t == y.NextStraight)//redundant jmp
						y.Instructions.Remove(last);//remove
					else//always jump
						ConnectStraight(y, t);//change
				}
				else
				{
					BasicBlock t = HeaderToBlock[last.Value.Bind.Target as ILInstruction];
					ConnectConditioned(y, t);
					if (y.NextConditioned == y.NextStraight)//only one exit
					{
						y.Instructions.Remove(last);//remove the jmp
						continue;
					}
				}
			}
		}

		private static void BuildDataFlowInBlock(BasicBlock block)
		{
			Dictionary<ILVariable, DataFlowNode> Nodes = block.DataFlow;
			Nodes.Clear();
			foreach (var il in block.Instructions)
			{
				foreach (var v in il.Bind.Operands)
				{
					if (v is ILVariable ilv)
					{
						DataFlowVariable vi = new DataFlowVariable(ilv);
						block.Variables.Add(vi);
						Nodes[ilv] = vi;//init
					}
				}
			}
			foreach (var il in block.Instructions)
			{
				var target = il.Bind.Target as ILVariable;
				var arg1 = il.Bind.Arg1 as ILVariable;
				var arg2 = il.Bind.Arg2 as ILVariable;
				switch (il.Bind.Type)
				{
					case ILType.Using:
						Nodes[target].Parent.Add(il);
						il.Children.Add(Nodes[target]);
						Nodes[target] = il;
						break;
					case ILType.Set:
						if (arg1 != null)
						{
							il.Children.Add(Nodes[arg1]);
							Nodes[arg1].Parent.Add(il);
						}
						Nodes[target].Parent.Add(il);
						il.Children.Add(Nodes[target]);
						Nodes[target] = il;
						break;
					case ILType.Assignment:
						if (arg1 != null)
						{
							il.Children.Add(Nodes[arg1]);
							Nodes[arg1].Parent.Add(il);
						}
						if (arg2 != null)
						{
							il.Children.Add(Nodes[arg2]);
							Nodes[arg2].Parent.Add(il);
						}
						Nodes[target].Parent.Add(il);
						il.Children.Add(Nodes[target]);
						Nodes[target] = il;
						break;
					case ILType.Param:
						if (arg1 != null)
						{
							il.Children.Add(Nodes[arg1]);
							Nodes[arg1].Parent.Add(il);
						}
						Nodes[target].Parent.Add(il);
						il.Children.Add(Nodes[target]);
						Nodes[target] = il;
						break;
					case ILType.Mem_Read:
						if (arg1 != null)
						{
							il.Children.Add(Nodes[arg1]);
							Nodes[arg1].Parent.Add(il);
						}
						if (arg2 != null)
						{
							il.Children.Add(Nodes[arg2]);
							Nodes[arg2].Parent.Add(il);
						}
						Nodes[target].Parent.Add(il);
						il.Children.Add(Nodes[target]);
						Nodes[target] = il;
						break;
					case ILType.Mem_Write:
						if (target != null)
						{
							il.Children.Add(Nodes[target]);
							Nodes[target].Parent.Add(il);
						}
						if (arg1 != null)
						{
							il.Children.Add(Nodes[arg1]);
							Nodes[arg1].Parent.Add(il);
						}
						if (arg2 != null)
						{
							il.Children.Add(Nodes[arg2]);
							Nodes[arg2].Parent.Add(il);
						}
						break;
					case ILType.Call:
						Nodes[target].Parent.Add(il);
						il.Children.Add(Nodes[target]);
						Nodes[target] = il;
						break;
					case ILType.Je:
					case ILType.Jne:
					case ILType.Jl:
					case ILType.Jle:
					case ILType.Jg:
					case ILType.Jge:
					case ILType.Jse:
						if (arg1 != null)
						{
							il.Children.Add(Nodes[arg1]);
							Nodes[arg1].Parent.Add(il);
						}
						if (arg2 != null)
						{
							il.Children.Add(Nodes[arg2]);
							Nodes[arg2].Parent.Add(il);
						}
						break;
					case ILType.Nop:
					case ILType.End:
					case ILType.Jmp:
					default:
						break;
				}
			}
		}

		private void BuildControlFlow()
		{
			FindHeaderILs();
			BuildControlFlowBlocks();
			BuildControlFlowJmps();
			foreach (var block in Blocks)
				BuildDataFlowInBlock(block);//building the data flow
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

		private static void UseInBlock(BasicBlock block, ILVariable va)
		{
			if (va == null)
				return;
			block.USE.Add(va);
			block.DEF.Remove(va);
		}


		private static void DefInBlock(BasicBlock block, ILVariable va)
		{
			if (va == null)
				return;
			block.DEF.Add(va);
			block.USE.Remove(va);
		}

		private static void BuildUSEAndDEFInBlock(LinkedListNode<BasicBlock> block)
		{
			BasicBlock blk = block.Value;
			blk.IN.Clear();
			blk.OUT.Clear();
			LinkedListNode<DataFlowInstruction> il = blk.Instructions.Last;
			while (il != null)
			{
				switch (il.Value.Bind.Type)
				{
					case ILType.Using:
						DefInBlock(blk, il.Value.Bind.Target as ILVariable);
						break;
					case ILType.Set:
						DefInBlock(blk, il.Value.Bind.Target as ILVariable);
						UseInBlock(blk, il.Value.Bind.Arg1 as ILVariable);
						break;
					case ILType.Assignment:
						DefInBlock(blk, il.Value.Bind.Target as ILVariable);
						UseInBlock(blk, il.Value.Bind.Arg1 as ILVariable);
						UseInBlock(blk, il.Value.Bind.Arg2 as ILVariable);
						break;
					case ILType.Param:
						UseInBlock(blk, il.Value.Bind.Target as ILVariable);
						break;
					case ILType.Mem_Read:
						DefInBlock(blk, il.Value.Bind.Target as ILVariable);
						UseInBlock(blk, il.Value.Bind.Arg1 as ILVariable);
						UseInBlock(blk, il.Value.Bind.Arg2 as ILVariable);
						break;
					case ILType.Mem_Write:
						UseInBlock(blk, il.Value.Bind.Target as ILVariable);
						UseInBlock(blk, il.Value.Bind.Arg1 as ILVariable);
						UseInBlock(blk, il.Value.Bind.Arg2 as ILVariable);
						break;
					case ILType.Call:
						DefInBlock(blk, il.Value.Bind.Target as ILVariable);
						break;
					case ILType.Je:
					case ILType.Jne:
					case ILType.Jl:
					case ILType.Jle:
					case ILType.Jg:
					case ILType.Jge:
					case ILType.Jse:
						UseInBlock(blk, il.Value.Bind.Arg1 as ILVariable);
						UseInBlock(blk, il.Value.Bind.Arg2 as ILVariable);
						break;
					case ILType.End:
					case ILType.Jmp:
					case ILType.Nop:
					default:
						break;
				}
				il = il.Previous;
			}
		}

		private static void BuildINAndOUTInBlock(LinkedListNode<BasicBlock> block)
		{
			block.Value.OUT.Clear();
			if ((block.Value.NextConditioned != null))
				block.Value.OUT.UnionWith(block.Value.NextConditioned.IN);
			if ((block.Value.NextStraight != null))
				block.Value.OUT.UnionWith(block.Value.NextStraight.IN);

			block.Value.IN.Clear();
			block.Value.IN.UnionWith(block.Value.USE);
			block.Value.IN.UnionWith(block.Value.OUT.Except(block.Value.DEF));
		}

		/// <summary>
		/// <para>IN(B)=USE(B)+(OUT(B)-DEF(B))</para>
		/// <para>OUT(B)=SUM[IN(S)] where S in B.Next</para>
		/// </summary>
		private void BuildDataFlowGlobal()
		{
			if (SourceBind is ILMacro ilm)
				Exit.IN.Add(ilm.ReturnValue);
			LinkedListNode<BasicBlock> block = Blocks.Last;//exit
			while ((block = block.Previous) != null)//to entry
			{
				BuildDataFlowLocal(block);
			}
		}
		private static void BuildDataFlowLocal(LinkedListNode<BasicBlock> block)
		{
			BuildUSEAndDEFInBlock(block);//building uses and defs
			BuildINAndOUTInBlock(block);//building ins and outs
		}

		private void RemoveDeadInstruction(LinkedListNode<BasicBlock> block, DataFlowInstruction v)
		{
			if (v.Parent.Count > 0)
				return;
			block.Value.Instructions.Find(v);
			v.Bind.Type = ILType.Nop;
			foreach (var node in v.Children)
				node.Parent.Remove(v);
			foreach (var node in v.Children)
				if (node is DataFlowInstruction dfi)
					RemoveDeadInstruction(block, dfi);
		}

		private void OptimizeDataFlowLocal(LinkedListNode<BasicBlock> block)
		{
			HashSet<DataFlowInstruction> v2d = new HashSet<DataFlowInstruction>();
			foreach (var (v, n) in block.Value.DataFlow)
				if (!block.Value.OUT.Contains(v) && block.Value.DEF.Contains(v))//not used later
					v2d.Add(n as DataFlowInstruction);
			foreach (var root in v2d)
				RemoveDeadInstruction(block, root);
			
		}

		private void OptimizeDataFlowGlobal()
		{
			LinkedListNode<BasicBlock> block = Blocks.Last;//exit
			while (block != null)//to entry
			{
				OptimizeDataFlowLocal(block);
				BuildDataFlowLocal(block);//rebuild
				block = block.Previous;
			}
		}

		public override void Run()
		{
			Log("Running flow optimization...");
			if (SourceBind.Instructions.Count == 0)
				return;
			BuildControlFlow();
			RemoveInaccessibleBlocks();
			BuildDataFlowGlobal();
			OptimizeDataFlowGlobal();


			//rebuild body
			SourceBind.Instructions.Clear();
			foreach (var block in Blocks)
			{
				foreach (var il in block.Instructions)
					SourceBind.Instructions.AddLast(il.Bind);
			}

		}
	}
}
