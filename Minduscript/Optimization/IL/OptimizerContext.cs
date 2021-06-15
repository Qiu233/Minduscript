using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	/// <summary>
	/// Provide optimizer's information
	/// </summary>
	public class OptimizerContext
	{
		/// <summary>
		/// IL source bound to this optmizer
		/// Will be changed after optmization completed
		/// </summary>
		public ILFunction SourceBind
		{
			get;
		}
		public CompilerContext CompilerContext
		{
			get;
		}

		public OptimizerContext(ILFunction sourceBind, CompilerContext compilerContext)
		{
			SourceBind = sourceBind;
			CompilerContext = compilerContext;
		}
	}
}
