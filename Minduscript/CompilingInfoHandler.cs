using Minduscript.CompilingInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript
{
	/// <summary>
	/// Handler of compiling information
	/// </summary>
	public class CompilingInfoHandler
	{
		public event Action<GeneralInfo> OnLog = null;
		public int MaxErrorsCount
		{
			get;
			set;
		}
		public IReadOnlyList<GeneralInfo> CompilingInfos
		{
			get;
		}
		public IEnumerable<ErrorInfo> Errors
		{
			get => CompilingInfos.Where(t => t is ErrorInfo).Select(t => t as ErrorInfo);
		}
		public IEnumerable<WarningInfo> Warnings
		{
			get => CompilingInfos.Where(t => t is WarningInfo).Select(t => t as WarningInfo);
		}
		public int ErrorsCount
		{
			get;
			private set;
		}
		public int WarningsCount
		{
			get;
			private set;
		}

		public void Log(GeneralInfo info)
		{
			OnLog(info);
		}

		public void ThrowError(SourcePosition pos, string doing, string msg)
		{
			Throw(new ErrorInfo(pos, doing, msg));
		}
		public void ThrowWarning(SourcePosition pos, string doing, string msg)
		{
			Throw(new WarningInfo(pos, doing, msg));
		}
		public void ThrowContextError(string doing, string msg)
		{
			Throw(new ContextErrorInfo(doing, msg));
		}
		public void Throw(ExceptionInfo info)
		{
			Log(info);
		}
		public void Log(string msg)
		{
			Log(new GeneralInfo(null, msg));
		}
		public CompilingInfoHandler()
		{
			CompilingInfos = new List<GeneralInfo>();
			OnLog = new Action<GeneralInfo>(CompilingInfoHandler_OnThrow);
			MaxErrorsCount = 20;
		}
		private void AddInfo(GeneralInfo info)
		{
			var infos = CompilingInfos as List<GeneralInfo>;
			infos.Add(info);
			if (info is ErrorInfo)
				ErrorsCount++;
			else if (info is WarningInfo)
				WarningsCount++;
		}
		private void CompilingInfoHandler_OnThrow(GeneralInfo info)
		{
			if (ErrorsCount > 20)
			{
				Log("Number of errors spilled");
				return;
			}
			AddInfo(info);
		}
	}
}
