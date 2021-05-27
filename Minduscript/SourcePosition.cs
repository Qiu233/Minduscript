using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript
{
	public struct SourcePosition
	{
		public string File
		{
			get;
			set;
		}
		public int Line
		{
			get;
			set;
		}
		public string Position => $"({File},{Line})";
		public SourcePosition(string file, int line)
		{
			File = file;
			Line = line;
		}
		public SourcePosition(SourcePosition s)
		{
			File = s.File;
			Line = s.Line;
		}
		public SourcePosition Copy(string file)
		{
			return new SourcePosition(file, Line);
		}
		public SourcePosition Copy(int line)
		{
			return new SourcePosition(File, line);
		}
		public SourcePosition Copy()
		{
			return new SourcePosition(File, Line);
		}
		public override string ToString()
		{
			return Position;
		}
	}
}
