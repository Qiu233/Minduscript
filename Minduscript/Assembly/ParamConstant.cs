using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly
{
	public class ParamConstant : ParamEvaluable
	{
		public string Value
		{
			get;
		}
		public ParamConstant(string value)
		{
			Value = value;
		}
		public static implicit operator ParamConstant(float s)
		{
			return new ParamConstant(s.ToString());
		}
		public override string GetASMCode()
		{
			return Value.ToString();
		}
	}
}
