using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public static class InstAssembly
	{
		private static void AddMacro(string name, IReadOnlyList<ILInstruction> insts, IReadOnlyList<ILVariable> ps)
		{
			ILMacro m = new ILMacro(new SourcePosition());
			m.Name = name;
			foreach (var ins in insts)
				m.Instructions.AddLast(ins);
			foreach (var p in ps)
				m.Params.AddLast(p);
			Inst.Macros.Add(m);
		}
		static InstAssembly()
		{
			List<ILInstruction> ils = new List<ILInstruction>();
			List<ILVariable> ps = new List<ILVariable>();
			Inst = new ILAssembly(null, "inst");
			#region draw
			{
				ils.Clear();
				ps.Clear();
				ils.Add(new ILInstruction(new SourcePosition(), ILType.Param, new ILAttribute(new SourcePosition()) { Name = "clear" }));
				for (int i = 0; i < 6; i++)
				{
					ILVariable v = new ILVariable(new SourcePosition());
					ps.Add(v);
					ils.Add(new ILInstruction(new SourcePosition(), ILType.Param, v));
				}
				ils.Add(new ILInstruction(new SourcePosition(), ILType.ASMCall, new ILConst(new SourcePosition()) { Value = "draw" }));
				AddMacro("draw_clear", ils, ps);
			}
			#endregion
		}
		public static ILAssembly Inst
		{
			get;
		}
	}
}
