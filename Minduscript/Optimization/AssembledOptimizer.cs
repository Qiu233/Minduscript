using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization
{
	/// <summary>
	/// The optimizer assembled from other optimizers
	/// Will run each sub-optimizers in a specific order
	/// </summary>
	public abstract class AssembledOptimizer<T> : BaseOptimizer<T>
	{
		public List<BaseOptimizer<T>> Optimizers
		{
			get;
		}
		public AssembledOptimizer(OptimizerContext<T> ctx) : base(ctx)
		{
			Optimizers = new List<BaseOptimizer<T>>();
		}
		public override void Run()
		{
			foreach (var opt in Optimizers)
				opt.Run();
		}
		public void AddOptimizer<E>() where E : BaseOptimizer<T>
		{
			if (Activator.CreateInstance(typeof(E), Context) is BaseOptimizer<T> instance)
				Optimizers.Add(instance);
		}
	}
}
