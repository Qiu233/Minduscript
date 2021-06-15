using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILResource : ILValue
	{
		public string Name
		{
			get;
			set;
		}
		public ILResource(SourcePosition src) : base(src)
		{
		}
		public override string ToString()
		{
			return $"@{Name}";
		}
	}
}
