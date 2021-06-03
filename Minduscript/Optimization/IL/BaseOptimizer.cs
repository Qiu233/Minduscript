using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	/// <summary>
	/// Base of il optimizers
	/// </summary>
	public abstract class BaseOptimizer
	{
		public OptimizerContext Context
		{
			get;
		}
		/// <summary>
		/// IL source bound to this optmizer
		/// Will be changed after optmization completed
		/// </summary>
		public IILExecutable SourceBind
		{
			get => Context.SourceBind;
		}
		/// <summary>
		/// Run this optimization
		/// </summary>
		public abstract void Run();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="src">IL source to bind to this optimizer</param>
		public BaseOptimizer(OptimizerContext ctx)
		{
			Context = ctx;
		}

		protected void Log(string msg)
		{
			Context.CompilerContext.Log($"[{SourceBind.Name}]{msg}");
		}
		protected void ThrowError(string err)
		{
			Context.CompilerContext.CompilingInfoHandler.ThrowContextError("Optimizing", err);
		}
	}
}
