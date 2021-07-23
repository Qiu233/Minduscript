using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	public class DataFlowOptimizer : ILBaseOptimizer
	{
		public DataFlowOptimizer(ILOptimizerContext ctx) : base(ctx)
		{
		}

		public override void Run()
		{
			Log("Running data flow optimization...");
			if (SourceBind.Instructions.Count == 0)
				return;

		}
	}
}
