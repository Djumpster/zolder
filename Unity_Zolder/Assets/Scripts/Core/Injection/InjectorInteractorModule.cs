// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System.Text;
using Talespin.Core.Foundation.Extensions;

namespace Talespin.Core.Foundation.Injection
{
	public class InjectorInteractorModule : IInjectorModule
	{
		private IInjectorModule injector;
		private Dictionary<object, List<object>> objectLookup = new Dictionary<object, List<object>>();
		private bool disposed;

		public InjectorInteractorModule(IInjectorModule injector)
		{
			this.injector = injector;
		}

		public IInjectorModule Clone()
		{
			// cloning this makes no sense at all. 
			throw new System.NotImplementedException();
		}

		public void Inject(object o)
		{
			if (o is IInjectorInteractor inj)
			{
				inj.RegisterInjector(new InterfaceInjectorHandle(inj, this));
			}
		}

		public void Remove(object o)
		{
			if (o is IInjectorInteractor inj)
			{
				ClearContext(inj);
			}
		}

		public string Log()
		{
			StringBuilder builder = new StringBuilder();
			foreach (KeyValuePair<object, List<object>> kvp in objectLookup)
			{
				builder.Append(kvp.Key.GetType());
				builder.AppendLine(":");
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					builder.Append(kvp.Value[i].ToString());
					builder.Append(", ");
				}
			}
			builder.Append("\n");
			return builder.ToString();
		}

		public void InjectForContext(object context, object o)
		{
			if (disposed)
			{
				return;
			}
			injector.Inject(o);
			objectLookup.Ensure(context);
			if (!objectLookup[context].Contains(o))
			{
				objectLookup[context].Add(o);
			}
		}

		public void RemoveFromContext(object context, object o)
		{
			if (disposed)
			{
				return;
			}
			injector.Remove(o);
			objectLookup.Ensure(context);
			objectLookup[context].Remove(o);
		}

		public void ClearContext(object inj)
		{
			if (disposed)
			{
				return;
			}

			if (objectLookup.ContainsKey(inj))
			{
				for (int i = 0; i < objectLookup[inj].Count; i++)
				{
					if (inj != objectLookup[inj][i])
					{
						injector.Remove(objectLookup[inj][i]);
					}
				}
				objectLookup[inj].Clear();
				objectLookup.Remove(inj);
			}
		}

		public void Dispose()
		{
			disposed = true;
			foreach (KeyValuePair<object, List<object>> kvp in objectLookup)
			{
				kvp.Value.Clear();
			}
			objectLookup.Clear();
			injector = null;
			objectLookup = null;
		}
	}
}