// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Injection.Internal;
using UnityEngine;

namespace Talespin.Core.Foundation.Injection
{
#if UNITY_EDITOR
	/// <summary>
	/// The DependencyViewerCollector is an editor-only data collector
	/// to keep track of created services.
	/// </summary>
	public static class DependencyViewerCollector
	{
		public struct BreadcrumbItem
		{
			public Type Type;
			public string String;
		}

		public struct ItemData
		{
			public object Item;
			public IDependencyLocator Factory;
			public float CreatedTime;
			public float CreatedFrame;
			public bool Active;
		}

		public static Action OnChangedEvent = delegate { };

		public static IEnumerable<ItemData> Data => data.Values;

		private static readonly Dictionary<object, ItemData> data = new Dictionary<object, ItemData>();
		private static readonly Dictionary<Type, BreadcrumbItem> breadcrumbCache = new Dictionary<Type, BreadcrumbItem>();

		private static readonly string[] baseFileBlacklist = new string[]
		{
			nameof(BaseDependencyLocator),
			nameof(DependencyViewerCollector),
		};

		public static void AddService(IDependencyLocator factory, object item)
		{
			if (item != null)
			{
				data.Remove(item);
				data.Add(item, new ItemData
				{
					Item = item,
					Factory = factory,
					CreatedTime = Time.realtimeSinceStartup,
					CreatedFrame = Time.frameCount,
					Active = item is IScopedDependency ? false : true
				});
				OnChangedEvent.Invoke();
			}
		}

		public static void RemoveService(object item)
		{
			if (item != null && data.Remove(item))
			{
				OnChangedEvent.Invoke();
			}
		}

		public static void Reset()
		{
			data.Clear();
			OnChangedEvent.Invoke();
		}

		public static void SetStatus(object item, bool active)
		{
			if (item != null && data.ContainsKey(item))
			{
				ItemData d = data[item];
				d.Active = active;
				data[item] = d;

				OnChangedEvent.Invoke();
			}
		}

		public static void SetBreadcrumb(Type createdType, Type parent)
		{
			if (!breadcrumbCache.ContainsKey(createdType))
			{
				breadcrumbCache.Add(createdType, new BreadcrumbItem
				{
					Type = parent
				});
			}
			else
			{
				breadcrumbCache[createdType] = new BreadcrumbItem
				{
					Type = parent
				};
			}
		}

		public static void SetInitialBreadcrumb(Type item)
		{
			if (!breadcrumbCache.ContainsKey(item))
			{
				if (typeof(IBootstrappable).IsAssignableFrom(item))
				{
					breadcrumbCache.Add(item, new BreadcrumbItem
					{
						String = "Bootstrap"
					});
				}
				else
				{
					StackTrace stackTrace = new StackTrace(0, true);

					for (int i = 0; i < stackTrace.FrameCount; i++)
					{
						StackFrame frame = stackTrace.GetFrame(i);

						string fileName = frame.GetFileName();
						int startIndex = fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1;
						fileName = fileName.Substring(startIndex).Replace(".cs", "");

						if (!baseFileBlacklist.Contains(fileName))
						{
							breadcrumbCache.Add(item, new BreadcrumbItem
							{
								String = fileName
							});

							break;
						}
					}
				}
			}
		}

		public static ItemData FindDataForItem(object item)
		{
			if (data.ContainsKey(item))
			{
				return data[item];
			}

			return default;
		}

		public static ItemData FindDataForType(Type type)
		{
			foreach (object item in data.Keys)
			{
				if (item.GetType() == type)
				{
					return data[item];
				}
			}

			return default;
		}

		public static List<BreadcrumbItem> GetBreadcrumb(Type type)
		{
			List<BreadcrumbItem> result = new List<BreadcrumbItem>();

			if (breadcrumbCache.ContainsKey(type))
			{
				result.Add(new BreadcrumbItem
				{
					Type = type
				});

				BreadcrumbItem parent = breadcrumbCache[type];

				while (parent.Type != null || !string.IsNullOrEmpty(parent.String))
				{
					result.Add(parent);
					parent = parent.Type != null && breadcrumbCache.ContainsKey(parent.Type) ? breadcrumbCache[parent.Type] : default;
				}

				result.Reverse();
			}

			return result;
		}
	}
#endif
}
