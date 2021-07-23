using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	public class ILAssembledOptimizer : AssembledOptimizer<ILFunction>
	{
		public ILAssembledOptimizer(OptimizerContext<ILFunction> ctx) : base(ctx)
		{
		}
		public static ILAssembledOptimizer GetDefaultOptimizer(ILOptimizerContext ctx)
		{
			ILAssembledOptimizer aopt = new ILAssembledOptimizer(ctx);
			aopt.AddOptimizer<ExpandingOptimizer>();
			aopt.AddOptimizer<FlowOptimizer>();
			aopt.AddOptimizer<RedundencyOptimizer>();
			return aopt;
		}
	}
}
