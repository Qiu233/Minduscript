using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly
{
	public abstract class ParamEvaluable : Param
	{
		public override string ToString()
		{
			return GetASMCode();
		}
		public static implicit operator ParamEvaluable(float s)
		{
			return new ParamConstant(s.ToString());
		}
	}
}
