using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	public class FlowOptimizer : ILAssembledOptimizer
	{
		public FlowOptimizer(ILOptimizerContext ctx) : base(ctx)
		{
			AddOptimizer<ControlFlowOptimizer>();
			AddOptimizer<DataFlowOptimizer>();
		}
	}
}
