using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	/// <summary>
	/// Variables' name have no meaning than a sole identification for one variable
	/// </summary>
	public class ILVariable : ILValue
	{
		public bool IsGlobal
		{
			get;
			set;
		}
		public ILVariable(SourcePosition src) : base(src)
		{
		}
	}
}
