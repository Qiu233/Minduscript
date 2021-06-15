using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly
{
	public class ParamResource : ParamEvaluable
	{
		public string Name
		{
			get;
		}
		public ParamResource(string name)
		{
			Name = name;
		}
		public static implicit operator ParamResource(string s)
		{
			return new ParamResource(s);
		}

		public override string GetASMCode()
		{
			return $"@{Name}";
		}
	}
}
