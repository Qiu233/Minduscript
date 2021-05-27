using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILConst : ILValue
	{
		/// <summary>
		/// string or number
		/// </summary>
		public object Value
		{
			get;
			set;
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
