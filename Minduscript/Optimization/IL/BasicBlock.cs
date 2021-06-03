using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
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
		public LinkedList<ILInstruction> Instructions
		{
			get;
		}
		public BasicBlock()
		{
			Instructions = new LinkedList<ILInstruction>();
			Previous = new HashSet<BasicBlock>();
		}
	}
}
