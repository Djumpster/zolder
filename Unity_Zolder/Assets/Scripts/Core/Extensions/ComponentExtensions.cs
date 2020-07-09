// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class ComponentExtensions
	{
		#region GetComponentInParents
		public static Component GetComponentInParents(this Component orig, System.Type componentType)
		{
			return orig.gameObject.GetComponentInParents(componentType);
		}

		public static T GetComponentInParents<T>(this Component orig) where T : Component
		{
			return orig.gameObject.GetComponentInParents<T>();
		}

		public static Component GetComponentInParents(this Component orig, System.Type componentType, System.Func<Component, bool> condition)
		{
			return orig.gameObject.GetComponentInParents(componentType, condition);
		}

		public static T GetComponentInParents<T>(this Component orig, System.Func<Component, bool> condition) where T : Component
		{
			return orig.gameObject.GetComponentInParents<T>(condition);
		}
		#endregion

		#region GetComponentInChildren
		public static Component GetComponentInChildren(this Component orig, System.Type componentType, bool includeInactive)
		{
			return orig.gameObject.GetComponentInChildren(componentType, includeInactive);
		}

		public static T GetComponentInChildren<T>(this Component orig, bool includeInactive) where T : Component
		{
			return orig.gameObject.GetComponentInChildren<T>(includeInactive);
		}
		#endregion

		public static bool HasParent(this Component orig, Transform possibleParent)
		{
			return orig.gameObject.HasParent(possibleParent);
		}
	}
}
