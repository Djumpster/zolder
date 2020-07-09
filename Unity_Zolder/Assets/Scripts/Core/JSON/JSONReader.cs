// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using UnityEngine;

namespace Talespin.Core.Foundation.Parsing
{
	public class JSONReader : MonoBehaviour
	{
		public static object SearchValueForKey(object o, string key)
		{
			if (o is ArrayList)
			{
				return SearchValueForKey(o as ArrayList, key);
			}

			if (o is Hashtable)
			{
				return SearchValueForKey(o as Hashtable, key);
			}

			if (o is DictionaryEntry)
			{
				DictionaryEntry de = (DictionaryEntry)o;
				if (de.Key.ToString() == key)
				{
					return de.Value;
				}

				return
					SearchValueForKey(de.Value, key);
			}
			return null;
		}

		public static object SearchValueForKey(ArrayList al, string key)
		{
			foreach (object o in al)
			{
				object r = SearchValueForKey(o, key);
				if (r != null)
				{
					return r;
				}
			}
			return null;
		}

		public static object SearchValueForKey(Hashtable ht, string key)
		{
			foreach (DictionaryEntry o in ht)
			{
				object r = SearchValueForKey(o, key);
				if (r != null)
				{
					return r;
				}
			}
			return null;
		}
	}
}