using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public class ILInstruction : ILOperand
	{
		public ILType Type
		{
			get;
			set;
		}
		/// <summary>
		/// length:4
		/// </summary>
		public ILOperand[] Operands
		{
			get;
		}
		public ILOperand Target
		{
			get => Operands[0];
			set => Operands[0] = value;
		}
		public ILOperand Arg1
		{
			get => Operands[1];
			set => Operands[1] = value;
		}
		public ILOperand Arg2
		{
			get => Operands[2];
			set => Operands[2] = value;
		}
		public ILOperand Arg3
		{
			get => Operands[3];
			set => Operands[3] = value;
		}
		public ILInstruction(SourcePosition pos, ILType type, ILOperand arg0 = null, ILOperand arg1 = null, ILOperand arg2 = null, ILOperand arg3 = null) : base(pos)
		{
			Type = type;
			Operands = new ILOperand[4];
			Operands[0] = arg0;
			Operands[1] = arg1;
			Operands[2] = arg2;
			Operands[3] = arg3;
		}
		public override string ToString()
		{
			return $"{Type} {string.Join(" ", Operands.Select(t => t == null ? "null" : t.ToString()))}";
		}
	}
}
