using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	/// <summary>
	/// Resource's Name is fixed while Variable's is not
	/// The name will be passed on to final asm code
	/// </summary>
	public class ILResource : ILOperand
	{
		public string Name
		{
			get;
		}
		public ILResource(SourcePosition src, string name) : base(src)
		{
			Name = name;
		}
	}
}
