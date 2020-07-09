// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Reflection;

namespace Talespin.Core.Foundation.Serialization
{
	public class CustomDataDrawerHelper
	{
		private IEnumerable<ICustomDataDrawer> drawers;

		public CustomDataDrawerHelper()
		{
			drawers = LoadDrawers();
		}

		public bool HasCustomDrawer(string tag)
		{
			return GetCustomDrawer(tag) != null;
		}

		public bool DrawWithCustomDrawer(DataEntry entry, string tag)
		{
			ICustomDataDrawer drawer = GetCustomDrawer(tag);
			if (drawer == null)
			{
				throw new ArgumentException("[CustomDataDrawerUtils] No custom drawer defined for tag: " + tag);
			}

			return drawer.Draw(entry);
		}

		private IEnumerable<ICustomDataDrawer> LoadDrawers()
		{
#if UNITY_EDITOR
			foreach (Type type in Reflect.AllTypesFrom(typeof(TypeDrawer).Assembly, typeof(ICustomDataDrawer)))
			{
				if (!type.IsAbstract && !type.IsInterface)
				{
					yield return (ICustomDataDrawer)Activator.CreateInstance(type);
				}
			}
#else
			yield break;
#endif
		}

		private ICustomDataDrawer GetCustomDrawer(string tag)
		{
			foreach (ICustomDataDrawer drawer in drawers)
			{
				foreach (string drawerTag in drawer.Tags)
				{
					if (tag.StartsWith(drawerTag))
					{
						return drawer;
					}
				}
			}
			return null;
		}
	}
}
