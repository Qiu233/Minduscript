using System;
using System.Collections.Generic;
using System.Text;

namespace Minduscript.Assembly
{
	public class ParamVariable : ParamEvaluable
	{
		public string Identifier
		{
			get;
		}
		public ParamVariable(string iden)
		{
			Identifier = iden;
		}
		public override string GetASMCode()
		{
			return Identifier;
		}
	}
}
