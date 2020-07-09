// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class TransformExtensions
	{
		// Look in obj and all its children for any GameObject with tag 'name'. It returns the first object found, or null 
		// if no such tag exists in the sub-tree  
		public static Transform GetChildWithTag(this Transform orig, string name)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			if (!orig)
			{
				return null;
			}
			if (orig.gameObject.tag == name)
			{
				return orig;
			}
			foreach (Transform kid in orig)
			{
				Transform res = GetChildWithTag(kid, name);
				if (res)
				{
					return res;
				}
			}
			return null;
		}

		public static Transform GetChildWithName(this Transform orig, string name)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			if (!orig)
			{
				return null;
			}
			if (orig.gameObject.name == name)
			{
				return orig;
			}
			foreach (Transform kid in orig)
			{
				Transform res = GetChildWithName(kid, name);
				if (res)
				{
					return res;
				}
			}
			return null;
		}

		// Look in obj and all its children for all GameObjects with tag 'name'. It returns a list of all object found  
		public static List<Transform> GetChildrenWithTag(this Transform orig, string name)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			List<Transform> ret = new List<Transform>();
			FindAllTagged(ret, orig, name);
			return ret;
		}

		private static void FindAllTagged(List<Transform> list, Transform t, string name)
		{
			if (!t)
			{
				return;
			}
			if (t.gameObject.tag == name)
			{
				list.Add(t);
			}
			foreach (Transform kid in t)
			{
				FindAllTagged(list, kid, name);
			}
		}

		public static IEnumerable<Transform> GetChildren(this Transform orig, Func<Transform, bool> includeChildrenCondition)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			foreach (Transform child in orig)
			{
				yield return child;
				if (includeChildrenCondition(child))
				{
					foreach (Transform grandChild in child.GetChildren(includeChildrenCondition))
					{
						yield return grandChild;
					}
				}
			}
		}

		public static IEnumerable<Transform> GetChildren(this Transform orig)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			foreach (Transform child in orig)
			{
				yield return child;
				foreach (Transform grandChild in child.GetChildren())
				{
					yield return grandChild;
				}
			}
		}


		public static IEnumerable<Transform> GetChildrenWithoutGrandChildren(this Transform orig)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			foreach (Transform child in orig)
			{
				yield return child;
			}
		}

		public static Transform GetChildRecursive(this Transform orig, string name)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			foreach (Transform child in orig)
			{
				if (child.name == name)
				{
					return child;
				}
				else
				{
					Transform grandChild = child.GetChildRecursive(name);
					if (grandChild != null)
					{
						return grandChild;
					}
				}
			}

			return null;
		}

		public static void Reset(this Transform orig, bool recursively = false)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			orig.localPosition = Vector3.zero;
			orig.localRotation = Quaternion.identity;
			orig.localScale = Vector3.one;

			if (recursively)
			{
				foreach (Transform t in orig)
				{
					t.Reset(true);
				}
			}
		}

		public static void ApplyRecursively(this Transform orig, System.Action<Transform> action)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			action(orig);
			foreach (Transform t in orig)
			{
				t.ApplyRecursively(action);
			}
		}

		public static Transform CreateChild(this Transform orig, string name)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			GameObject go = new GameObject(name);
			Transform t = go.transform;
			t.parent = orig;
			t.Reset();
			return t;
		}

		public static void DestroyChildren(this Transform orig)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			foreach (Transform t in orig)
			{
				GameObject.Destroy(t.gameObject);
			}
		}

		public static void DestroyChildrenImmediate(this Transform orig)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			// Use a temporary array because iterating over the original while modifying the collection will break things.
			Transform[] temp = new Transform[orig.childCount];
			for (int i = 0; i < orig.childCount; i++)
			{
				temp[i] = orig.GetChild(i);
			}

			foreach (Transform t in temp)
			{
				GameObject.DestroyImmediate(t.gameObject);
			}
		}

		public static string GetFullHierarchyName(this Transform orig)
		{
			if (orig == null)
			{
				throw new NullReferenceException("Extension method called on null object.");
			}

			StringBuilder sb = new StringBuilder();

			sb.Append(orig.name);
			while (orig.parent != null)
			{
				sb.Insert(0, orig.parent.name + ".");
				orig = orig.parent;
			}

			return sb.ToString();
		}

		public static int FindChildNumber(this Transform trans, int number = 0)
		{
			if (trans.parent == null)
			{
				return number;
			}

			return FindChildNumber(trans.parent, trans.GetSiblingIndex() + number);
		}
	}
}
