using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly
{
	public class ParamAttribute : Param
	{
		public Enum Attribute
		{
			get;
		}
		public ParamAttribute(Enum attr)
		{
			Attribute = attr;
		}
		public static implicit operator ParamAttribute(Enum s)
		{
			return new ParamAttribute(s);
		}
		public override string GetASMCode()
		{
			return Attribute.ToString();
		}
	}
}
