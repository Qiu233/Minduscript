using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization.IL
{
	public class ILOptimizerContext : OptimizerContext<ILFunction>
	{
		public ILOptimizerContext(ILFunction source, CompilerContext compilerContext) : base(source, compilerContext)
		{
		}
	}
}
