using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly
{
	public class ParamString:ParamEvaluable
	{
		public string Value
		{
			get;
		}
		public ParamString(string value)
		{
			Value = value;
		}
		public static implicit operator ParamString(string s)
		{
			return new ParamString(s);
		}
		public override string GetASMCode()
		{
			return $"\"{Value}\"";
		}
	}
}
