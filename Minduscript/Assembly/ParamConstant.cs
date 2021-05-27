using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly
{
	public class ParamConstant : ParamEvaluable
	{
		public float Value
		{
			get;
		}
		public ParamConstant(float value)
		{
			Value = value;
		}
		public static implicit operator ParamConstant(float s)
		{
			return new ParamConstant(s);
		}
		public override string GetASMCode()
		{
			return Value.ToString();
		}
	}
}
