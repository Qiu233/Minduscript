using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Parse.AST
{
	public class EntryContext
	{
		public HashSet<ILInstruction> ETrue;
		public HashSet<ILInstruction> EFalse;
		public HashSet<ILInstruction> Header;
		public HashSet<ILInstruction> Next;
		public EntryContext()
		{
			ETrue = new HashSet<ILInstruction>();
			EFalse = new HashSet<ILInstruction>();
			Header = new HashSet<ILInstruction>();
			Next = new HashSet<ILInstruction>();
		}
		public static HashSet<ILInstruction> MergeSet(ref HashSet<ILInstruction> a, ref HashSet<ILInstruction> b)
		{
			a.UnionWith(b);
			b = a;
			return a;
		}
		public static void Merge(EntryContext a, EntryContext b)
		{
			MergeSet(ref a.ETrue, ref b.ETrue);
			MergeSet(ref a.EFalse, ref b.EFalse);
			MergeSet(ref a.Header, ref b.Header);
			MergeSet(ref a.Next, ref b.Next);
		}
	}
}
