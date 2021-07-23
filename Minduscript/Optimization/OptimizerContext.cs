using Minduscript.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.Optimization
{
	/// <summary>
	/// Provide optimizer's information
	/// </summary>
	public abstract class OptimizerContext<T>
	{
		/// <summary>
		/// Source bound to this optmizer
		/// Will be modified after optmization completed
		/// </summary>
		public T Bind
		{
			get;
		}
		public CompilerContext CompilerContext
		{
			get;
		}
		public Dictionary<string, object> Data
		{
			get;
		}

		public OptimizerContext(T source, CompilerContext compilerContext)
		{
			Bind = source;
			CompilerContext = compilerContext;
			Data = new Dictionary<string, object>();
		}

		public void PutData(string name, object o)
		{
			Data[name] = o;
		}

		public void PutData<E>(string name, E o) where E : class
		{
			Data[name] = o;
		}

		public object GetData(string name)
		{
			return Data[name];
		}

		public E GetData<E>(string name) where E : class
		{
			return Data[name] as E;
		}
	}
}
