using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL.Optimization
{
	/// <summary>
	/// The optimizer assembled from other optimizers
	/// Will run each sub-optimizers in a specific order
	/// </summary>
	public class AssembledOptimizer : BaseOptimizer
	{
		public List<BaseOptimizer> Optimizers
		{
			get;
		}
		public AssembledOptimizer(OptimizerContext ctx) : base(ctx)
		{
			Optimizers = new List<BaseOptimizer>();
		}
		public override void Run()
		{
			foreach (var opt in Optimizers)
				opt.Run();
		}
		public void AddOptimizer<T>() where T : BaseOptimizer
		{
			if (Activator.CreateInstance(typeof(T), Context) is BaseOptimizer instance)
				Optimizers.Add(instance);
		}
		public static AssembledOptimizer GetDefaultOptimizer(OptimizerContext ctx)
		{
			AssembledOptimizer aopt = new AssembledOptimizer(ctx);
			aopt.AddOptimizer<ExpandingOptimizer>();
			aopt.AddOptimizer<FlowOptimizer>();
			aopt.AddOptimizer<RedundencyOptimizer>();
			return aopt;
		}
	}
}
