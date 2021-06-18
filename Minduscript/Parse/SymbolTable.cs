using Minduscript.Parse.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse
{
	public class Symbol<T>
	{
		public T Content
		{
			get;
		}
		public int Index
		{
			get;
		}
		public Symbol(T name, int index)
		{
			Content = name;
			Index = index;
		}

	}
	public sealed class SymbolTable<T>
	{
		public HashSet<Symbol<T>> Symbols
		{
			get;
		}
		private HashSet<SymbolTable<T>> SubTables
		{
			get;
		}
		public SymbolTable<T> Parent
		{
			get;
			private set;
		}
		public SymbolTable()
		{
			Symbols = new HashSet<Symbol<T>>();
			SubTables = new HashSet<SymbolTable<T>>();
		}
		public bool ExistSymbol(T ev)
		{
			return Symbols.ToList().Exists(t => t.Content.Equals(ev));
		}
		public void AddSymbol(T ev)
		{
			Symbols.Add(new Symbol<T>(ev, Symbols.Count));
		}
		public int CountToRoot()
		{
			return CountParentToRoot() + Symbols.Count;
		}
		public int CountParentToRoot()
		{
			SymbolTable<T> cur = Parent;
			int off = 0;
			while (true)
			{
				if (cur == null)
					break;
				off += cur.Symbols.Count;
				cur = cur.Parent;
			}
			return off;
		}

		public SymbolTable<T> NewSubTable()
		{
			SymbolTable<T> a = new SymbolTable<T>();
			SubTables.Add(a);
			a.Parent = this;
			return a;
		}

		public void RemoveSubTable(SymbolTable<T> t)
		{
			if (!SubTables.Contains(t))
				return;
			t.Parent = null;
			SubTables.Remove(t);
		}
	}
}
