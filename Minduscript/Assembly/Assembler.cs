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
		private Dictionary<string, ParamVariable> Variables
		{
			get;
		}
		public ParamVariable this[string name]
		{
			get
			{
				if (Variables.ContainsKey(name))
					return Variables[name];
				return (Variables[name] = new ParamVariable(name));
			}
		}
		private LinkedList<Instruction> Instructions
		{
			get;
		}
		public int CurrentInstructionIndex
		{
			get => Instructions.Count;
		}
		public Assembler()
		{
			Instructions = new LinkedList<Instruction>();
			Variables = new Dictionary<string, ParamVariable>();
		}
		public LinkedListNode<Instruction> Assemble<T>(T inst) where T : Instruction
		{
			return Instructions.AddLast(inst);
		}
		public void AssembleToASMCode(TextWriter tw)
		{
			foreach (var inst in Instructions)
			{
				inst.Compile(tw);
				tw.WriteLine();
			}
		}
	}
}
