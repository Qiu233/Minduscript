using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public interface IILCallable
	{
		public ILOperandCollection<ILVariable> Params
		{
			get;
		}
	}
}
