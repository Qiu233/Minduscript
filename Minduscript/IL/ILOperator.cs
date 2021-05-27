using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public enum OperatorType
	{
		Add,
		Sub,
		Mul,
		Div,
		IDiv,
		Mod,
		Equal,
		NotEqual,
		StrictEqual,
		Land,//&&
		Lor,//||
		LNot,//!
		LessThan,
		LessThanEq,
		GreaterThan,
		GreaterThanEq,
		Shl,
		Shr,
		Or,//|
		And,//&
		Xor,//^
		Flip,//~
	}
	public class ILOperator : ILOperand
	{
		public ILOperator(SourcePosition src) : base(src)
		{

		}
		public OperatorType Type
		{
			get;
			set;
		}
		public static readonly IReadOnlyDictionary<string, OperatorType> OperatorsMap = new Dictionary<string, OperatorType>()
		{
			{"+", OperatorType.Add },
			{"-", OperatorType.Sub },
			{"*", OperatorType.Mul },
			{"/", OperatorType.Div },
			{"//", OperatorType.IDiv },
			{"%", OperatorType.Mod },
			{"==", OperatorType.Equal },
			{"===", OperatorType.StrictEqual },
			{"!=", OperatorType.NotEqual },
			{"&&", OperatorType.Land },
			{"||", OperatorType.Lor },
			{"!", OperatorType.LNot },
			{"<", OperatorType.LessThan },
			{"<=", OperatorType.LessThanEq },
			{">", OperatorType.GreaterThan },
			{">=", OperatorType.GreaterThanEq },
			{"<<", OperatorType.Shl },
			{">>", OperatorType.Shr },
			{"|", OperatorType.Or },
			{"&", OperatorType.And },
			{"^", OperatorType.Xor },
			{"~", OperatorType.Flip },
		};
		public static ILOperator GetOPTRFromString(SourcePosition src, string optr)
		{
			ILOperator r = new ILOperator(src)
			{
				Type = OperatorsMap[optr]
			};
			return r;
		}

	}
}
