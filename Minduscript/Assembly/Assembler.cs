using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Assembly
{
	public class Assembler
	{
		private CodeTarget Target
		{
			get;
		}
		public ParamVariable this[string name]
		{
			get
			{
				if (Target.Variables.ContainsKey(name))
					return Target.Variables[name];
				return (Target.Variables[name] = new ParamVariable(name));
			}
		}
		public int CurrentInstructionIndex
		{
			get => Target.Instructions.Count;
		}
		public Assembler()
		{
			Target = new CodeTarget();
		}
		public LinkedListNode<Instruction> Assemble<T>(T inst) where T : Instruction
		{
			return Target.Instructions.AddLast(inst);
		}
		public void AssembleToASMCode(TextWriter tw)
		{
			foreach (var inst in Target.Instructions)
			{
				inst.Compile(tw);
				tw.WriteLine();
			}
		}
	}
}
