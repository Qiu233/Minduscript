using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly
{
	public enum Operators
	{
		add, sub, mul, div, idiv, mod, pow,
		equal, notEqual, land, lessThan, lessThanEq, greaterThan, greaterThanEq, strictEqual,
		shl, shr, or, and, xor, not, flip,
		max, min, angle, len, noise, abs, log, log10,
		sin, cos, tan, floor, ceil, sqrt, rand
	}
}
