using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly
{
	public abstract class Param
	{
		public static implicit operator Param(float f)
		{
			return new ParamConstant(f.ToString());
		}
		public static implicit operator Param(string f)
		{
			return new ParamString(f);
		}
		public static implicit operator Param(Enum f)
		{
			return new ParamAttribute(f);
		}
		public override string ToString()
		{
			return GetASMCode();
		}
		public abstract string GetASMCode();
	}
}
