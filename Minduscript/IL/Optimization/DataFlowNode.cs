using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL.Optimization
{
	public abstract class DataFlowNode
	{
		public object Bind
		{
			get;
		}
		public List<DataFlowNode> Children
		{
			get;
		}
		public List<DataFlowNode> Parent
		{
			get;
		}
		public DataFlowNode(ILOperand v)
		{
			Bind = v;
			Children = new List<DataFlowNode>();
			Parent = new List<DataFlowNode>();
		}
	}
	public class DataFlowInstruction : DataFlowNode
	{
		public new ILInstruction Bind
		{
			get => base.Bind as ILInstruction;
		}
		public DataFlowInstruction(ILInstruction v) : base(v)
		{
		}
	}
	public class DataFlowVariable : DataFlowNode
	{
		public new ILVariable Bind
		{
			get => base.Bind as ILVariable;
		}
		public DataFlowVariable(ILVariable v) : base(v)
		{

		}
	}
}
