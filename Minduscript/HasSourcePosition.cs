using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript
{
	public interface IHasSourcePosition
	{
		public SourcePosition SourcePosition
		{
			get;
			set;
		}
	}
}
