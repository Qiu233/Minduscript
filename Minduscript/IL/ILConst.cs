using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public abstract class ILConst : ILValue
	{
		/// <summary>
		/// string or number
		/// </summary>
		public abstract string Value
		{
			get;
		}
		public ILConst(SourcePosition src) : base(src)
		{

		}
		public override int GetHashCode()
		{
			return Value == null ? "null".GetHashCode() : Value.GetHashCode();
		}
	}
}
