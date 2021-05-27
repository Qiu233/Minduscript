using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript
{
	public class CompilerStoppingException : Exception
	{
		public CompilerStoppingException(string reason):base(reason)
		{
			
		}
	}
}
