using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Minduscript.Assembly
{
	public abstract class Instruction
	{
		public OpCode OpCode
		{
			get;
		}
		public Param[] Params
		{
			get;
		}
		protected Instruction(OpCode opCode, params Param[] @params)
		{
			OpCode = opCode;
			Params = @params;
		}
		public void Compile(TextWriter tw)
		{
			tw.Write(OpCode.ToString().ToLower());
			tw.Write(' ');
			tw.Write(string.Join<Param>(' ', Params));
		}
	}
}
