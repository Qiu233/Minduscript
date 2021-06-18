﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class Expr_Variable : Expression
	{
		public Expr_Variable(SourcePosition src) : base(src)
		{
		}
		public string Name
		{
			get;
			set;
		}

	}
}
