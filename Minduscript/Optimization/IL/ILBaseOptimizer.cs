using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	public abstract class ILBaseOptimizer : BaseOptimizer<ILFunction>
	{
		protected ILBaseOptimizer(OptimizerContext<ILFunction> ctx) : base(ctx)
		{
		}
		protected override void ThrowError(string err)
		{
			Context.CompilerContext.CompilingInfoHandler.ThrowContextError("ILOptimizing", err);
		}
	}
}
