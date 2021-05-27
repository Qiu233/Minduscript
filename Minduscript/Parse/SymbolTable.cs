using Minduscript.Parse.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse
{
	public class SymbolTable<T>
	{
		public Dictionary<T, int> Symbols
		{
			get;
		}
		public SymbolTable()
		{
			Symbols = new Dictionary<T, int>();
		}
		public bool ExistSymbol(T ev)
		{
			return Symbols.Keys.ToList().Exists(t => t.Equals(ev));
		}
		public void AddSymbol(T ev)
		{
			Symbols[ev] = Symbols.Count;
		}
		public int GetSymbolOffset(T ev)
		{
			return Symbols[ev];
		}
	}
}
