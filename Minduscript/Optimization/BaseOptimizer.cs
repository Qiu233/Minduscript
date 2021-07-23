using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization
{
	/// <summary>
	/// Base of il optimizers
	/// </summary>
	public abstract class BaseOptimizer<T>
	{
		public OptimizerContext<T> Context
		{
			get;
		}
		/// <summary>
		/// IL source bound to this optmizer
		/// Will be changed after optmization completed
		/// </summary>
		public T SourceBind
		{
			get => Context.Bind;
		}
		/// <summary>
		/// Run this optimization
		/// </summary>
		public abstract void Run();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="src">IL source to bind to this optimizer</param>
		public BaseOptimizer(OptimizerContext<T> ctx)
		{
			Context = ctx;
		}

		protected virtual void Log(string msg)
		{
			Context.CompilerContext.Log($"[{SourceBind}]{msg}");
		}
		protected virtual void ThrowError(string err)
		{
			Context.CompilerContext.CompilingInfoHandler.ThrowContextError("Optimizing", err);
		}
	}
}
