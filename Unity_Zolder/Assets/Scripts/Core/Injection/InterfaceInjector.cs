// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Talespin.Core.Foundation.Injection
{
	public class InterfaceInjector : IInterfaceInjector
	{
		private readonly List<IInjectorModule> modules;

		private bool isDisposed;

		public InterfaceInjector(IEnumerable<IInjectorModule> collection) : this()
		{
			modules.InsertRange(0, collection);
		}

		public InterfaceInjector(IInjectorModule item) : this()
		{
			modules.Insert(0, item);
		}

		public InterfaceInjector()
		{
			modules = new List<IInjectorModule>
			{
				new InjectorInteractorModule(this)
			};
		}

		public IInjectorModule Clone()
		{
			List<IInjectorModule> newModules = new List<IInjectorModule>(modules.Count);

			for (int i = 0; i < modules.Count; i++)
			{
				if (!(modules[i] is InjectorInteractorModule))
				{
					newModules.Add(modules[i].Clone());
				}
			}

			return new InterfaceInjector(newModules);
		}

		public void AddModules(IEnumerable<IInjectorModule> collection)
		{
			if (!isDisposed)
			{
				modules.AddRange(collection);
			}
		}

		public void AddModule(IInjectorModule item)
		{
			if (!isDisposed)
			{
				modules.Add(item);
			}
		}

		public void InsertModules(int index, IEnumerable<IInjectorModule> collection)
		{
			if (!isDisposed)
			{
				modules.InsertRange(index, collection);
			}
		}

		public void InsertModule(int index, IInjectorModule item)
		{
			if (!isDisposed)
			{
				modules.Insert(index, item);
			}
		}

		public int RemoveModule(IInjectorModule module)
		{
			int index = modules.IndexOf(module);

			if (index >= 0)
			{
				modules.Remove(module);
			}

			return index;
		}

		public void Inject(object o)
		{
			if (isDisposed)
			{
				return;
			}

			if (o is GameObject)
			{
				MonoBehaviour[] all = (o as GameObject).GetComponentsInChildren<MonoBehaviour>(true);

				for (int i = 0; i < all.Length; i++)
				{
					if (all[i] && all[i] != null)
					{
						Inject(all[i]);
					}
				}
			}
			else
			{
				for (int i = 0; i < modules.Count; i++)
				{
					modules[i].Inject(o);
				}
			}
		}

		public void Remove(object o)
		{
			if (isDisposed)
			{
				return;
			}

			if (o != null && o is GameObject)
			{
				GameObject go = o as GameObject;

				if (go)
				{
					MonoBehaviour[] all = go.GetComponentsInChildren<MonoBehaviour>(true);

					for (int i = 0; i < all.Length; i++)
					{
						Remove(all[i]);
					}
				}
			}
			else
			{
				for (int i = 0; i < modules.Count; i++)
				{
					modules[i].Remove(o);
				}
			}
		}

		public T GetModule<T>() where T : class, IInjectorModule
		{
			Type type = typeof(T);

			for (int i = 0; i < modules.Count; i++)
			{
				if (type.IsAssignableFrom(modules[i].GetType()))
				{
					return (T)modules[i];
				}
			}

			return default;
		}

		public string Log()
		{
			if (isDisposed)
			{
				return "<Disposed Injector>";
			}

			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < modules.Count; i++)
			{
				builder.Append(modules[i].GetType());
				builder.AppendLine(":");
				builder.AppendLine(modules[i].Log());
			}

			return builder.ToString();
		}

		public void Dispose()
		{
			if (isDisposed)
			{
				return;
			}

			isDisposed = true;

			for (int i = 0; i < modules.Count; i++)
			{
				modules[i].Dispose();
			}

			modules.Clear();
		}
	}
}
