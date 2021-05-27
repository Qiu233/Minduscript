using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly.Instructions
{
	public class Draw : Instruction
	{
		public Draw(DrawModes mode, Param arg0, Param arg1, Param arg2, Param arg3, Param arg4, Param arg5) : base(OpCode.DRAW, mode, arg0, arg1, arg2, arg3, arg4, arg5)
		{

		}
		public static Draw Clear(ParamEvaluable r, ParamEvaluable g, ParamEvaluable b)
		{
			return new Draw(DrawModes.clear, r, g, b, 0, 0, 0);
		}
		public static Draw Color(ParamEvaluable r, ParamEvaluable g, ParamEvaluable b, ParamEvaluable a)
		{
			return new Draw(DrawModes.color, r, g, b, a, 0, 0);
		}
		public static Draw Stroke(ParamEvaluable width)
		{
			return new Draw(DrawModes.stroke, width, 0, 0, 0, 0, 0);
		}
		public static Draw Line(ParamEvaluable x1, ParamEvaluable y1, ParamEvaluable x2, ParamEvaluable y2)
		{
			return new Draw(DrawModes.line, x1, y2, x2, y2, 0, 0);
		}
		public static Draw Rect(ParamEvaluable x, ParamEvaluable y, ParamEvaluable width, ParamEvaluable height)
		{
			return new Draw(DrawModes.rect, x, y, width, height, 0, 0);
		}
		public static Draw LineRect(ParamEvaluable x, ParamEvaluable y, ParamEvaluable width, ParamEvaluable height)
		{
			return new Draw(DrawModes.lineRect, x, y, width, height, 0, 0);
		}
		public static Draw Poly(ParamEvaluable x, ParamEvaluable y, ParamEvaluable sides, ParamEvaluable radiuss, ParamEvaluable rotation)
		{
			return new Draw(DrawModes.poly, x, y, sides, radiuss, rotation, 0);
		}
		public static Draw LinePoly(ParamEvaluable x, ParamEvaluable y, ParamEvaluable sides, ParamEvaluable radiuss, ParamEvaluable rotation)
		{
			return new Draw(DrawModes.linePoly, x, y, sides, radiuss, rotation, 0);
		}
		public static Draw LineRect(ParamEvaluable x1, ParamEvaluable y1, ParamEvaluable x2, ParamEvaluable y2, ParamEvaluable x3, ParamEvaluable y3)
		{
			return new Draw(DrawModes.lineRect, x1, y1, x2, y2, x3, y3);
		}
		public static Draw Image(ParamEvaluable x, ParamEvaluable y, ParamResource img, ParamEvaluable size, ParamEvaluable rotation)
		{
			return new Draw(DrawModes.image, x, y, img, size, rotation, 0);
		}
	}
}
