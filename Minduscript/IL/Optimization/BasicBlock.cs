using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL.Optimization
{
	public class BasicBlock
	{
		public HashSet<BasicBlock> Previous
		{
			get;
		}
		public BasicBlock NextStraight
		{
			get;
			set;
		}
		public BasicBlock NextConditioned
		{
			get;
			set;
		}
		/// <summary>
		/// Variables alive at the entry
		/// </summary>
		public HashSet<ILVariable> IN
		{
			get;
		}
		/// <summary>
		/// Variables alive at the exit
		/// </summary>
		public HashSet<ILVariable> OUT
		{
			get;
		}
		public HashSet<ILVariable> USE
		{
			get;
		}
		public HashSet<ILVariable> DEF
		{
			get;
		}
		public LinkedList<DataFlowInstruction> Instructions
		{
			get;
		}
		public HashSet<DataFlowVariable> Variables
		{
			get;
		}
		public Dictionary<ILVariable, DataFlowNode> DataFlow
		{
			get;
		}
		public BasicBlock()
		{
			Instructions = new LinkedList<DataFlowInstruction>();
			Variables = new HashSet<DataFlowVariable>();
			DataFlow = new Dictionary<ILVariable, DataFlowNode>();
			Previous = new HashSet<BasicBlock>();
			IN = new HashSet<ILVariable>();
			OUT = new HashSet<ILVariable>();
			USE = new HashSet<ILVariable>();
			DEF = new HashSet<ILVariable>();
		}
	}
}
