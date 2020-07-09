// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class GameObjectExtensions
	{
		public static T FindInterface<T>(this GameObject go) where T : class
		{
			return FindInterfaces<T>(go).FirstOrDefault();
		}

		public static IEnumerable<T> FindInterfaces<T>(this GameObject go) where T : class
		{
			foreach (MonoBehaviour behaviour in go.GetComponents<MonoBehaviour>())
			{
				if (behaviour)
				{
					if (typeof(T).IsAssignableFrom(behaviour.GetType()))
					{
						yield return behaviour as T;
					}
				}
			}
		}

		public static void SetLayerRecursively(this GameObject go, int layer)
		{
			go.layer = layer;
			foreach (Transform t in go.transform)
			{
				SetLayerRecursively(t.gameObject, layer);
			}
		}

		#region GetComponentInParents
		public static Component GetComponentInParents(this GameObject orig, Type componentType)
		{
			Component comp = orig.GetComponent(componentType);
			if (comp != null)
			{
				return comp;
			}
			Transform parent = orig.transform.parent;
			return parent?.GetComponentInParents(componentType);
		}

		public static T GetComponentInParents<T>(this GameObject orig) where T : Component
		{
			T comp = orig.GetComponent<T>();
			if (comp != null)
			{
				return comp;
			}
			Transform parent = orig.transform.parent;
			return parent?.GetComponentInParents<T>();
		}

		public static GameObject FindInChildren(this GameObject orig, string name)
		{
			Transform child = orig.transform.Find(name);
			if (child != null)
			{
				return child.gameObject;
			}

			return null;
		}

		public static IEnumerable<GameObject> GetImmediateChildren(this GameObject go)
		{
			foreach (Transform child in go.transform)
			{
				yield return child.gameObject;
			}
		}

		public static IEnumerable<GameObject> GetChildren(this GameObject go, Func<GameObject, bool> includeChildrenCondition)
		{
			foreach (Transform goTrans in go.transform.GetChildren(trans => trans != null && includeChildrenCondition(trans.gameObject)))
			{
				yield return goTrans.gameObject;
			}
		}

		public static IEnumerable<GameObject> GetChildren(this GameObject go)
		{
			foreach (Transform goTrans in go.transform.GetChildren())
			{
				yield return goTrans.gameObject;
			}
		}

		public static GameObject GetChildRecursive(this GameObject trans, string name)
		{
			Transform foundChild = trans.transform.GetChildRecursive(name);

			return foundChild?.gameObject;
		}

		public static T FindInChildren<T>(this GameObject go, string name) where T : Component
		{
			if (go.name == name)
			{
				return go.GetComponent<T>();
			}

			foreach (Transform t in go.transform)
			{
				return FindInChildren<T>(t.gameObject, name);
			}

			return null;
		}

		public static Component GetComponentInParents(this GameObject orig, Type componentType, System.Func<Component, bool> condition)
		{
			foreach (Component comp in orig.GetComponents(componentType))
			{
				if (condition(comp))
				{
					return comp;
				}
			}
			Transform parent = orig.transform.parent;
			return parent?.GetComponentInParents(componentType, condition);
		}

		public static T GetComponentInParents<T>(this GameObject orig, System.Func<Component, bool> condition) where T : Component
		{
			foreach (T comp in orig.GetComponents<T>())
			{
				if (condition(comp))
				{
					return comp;
				}
			}
			Transform parent = orig.transform.parent;
			return parent?.GetComponentInParents<T>(condition);
		}
		#endregion

		#region GetComponentInChildren
		public static Component GetComponentInChildren(this GameObject orig, Type componentType, bool includeInactive)
		{
			foreach (Component comp in orig.GetComponentsInChildren(componentType, includeInactive))
			{
				return comp;
			}
			return null;
		}

		public static T GetComponentInChildren<T>(this GameObject orig, bool includeInactive) where T : Component
		{
			foreach (T comp in orig.GetComponentsInChildren<T>(includeInactive))
			{
				return comp;
			}
			return null;
		}
		public static T GetComponentInChildren<T>(this GameObject orig, bool includeInactive, System.Func<T, bool> condition) where T : Component
		{
			foreach (T comp in orig.GetComponentsInChildren<T>(includeInactive))
			{
				if (condition(comp))
				{
					return comp;
				}
			}
			return null;
		}
		#endregion

		public static T RequireComponent<T>(this GameObject orig) where T : Component
		{
			T comp = orig.GetComponent<T>();
			if (comp == null)
			{
				comp = orig.AddComponent<T>();
			}
			return comp;
		}

		public static bool HasParent(this GameObject orig, Transform possibleParent)
		{
			for (Transform t = orig.transform; t != null; t = t.parent)
			{
				if (t == possibleParent)
				{
					return true;
				}
			}
			return false;
		}


		public static void ParentToRectTransform(this GameObject child, RectTransform parent)
		{
			Transform childTransform = child.transform;
			childTransform.SetParent(parent);
			childTransform.localScale = new Vector3(1f, 1f, 1f);
			childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y, 0f);

			RectTransform rt = child.GetComponentInChildren<RectTransform>();
			if (rt != null)
			{
				rt.offsetMax = Vector2.zero;
				rt.offsetMin = Vector2.zero;
			}
		}
	}
}
