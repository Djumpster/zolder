// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public abstract class StringPopupDrawer : ICustomDataDrawer
	{
		public abstract IEnumerable<string> Tags { get; }

		public bool Draw(DataEntry entry)
		{
			if (entry.Type != DataEntry.DataType.String)
			{
				throw new Exception("[StringPopupDrawer] This drawer only works on strings!");
			}
			DataEntry.StringEntry stringEntry = entry.Data as DataEntry.StringEntry;

			string[] allOptions = GetOptions(entry).ToArray();
			string currentVal = stringEntry.Value;
			string newVal = string.Empty;

			if (allOptions.Length > 0)
			{
				int selected = string.IsNullOrEmpty(currentVal) ? 0 : Array.IndexOf(allOptions, currentVal);
				int newSelection = EditorGUILayout.Popup(selected, allOptions);
				newVal = allOptions[Mathf.Max(0, newSelection)];
			}

			bool dirty = false;
			if (currentVal != newVal)
			{
				stringEntry.Value = newVal;
				dirty = true;
			}
			return dirty;
		}

		protected abstract IEnumerable<string> GetOptions(DataEntry entry);
	}
}
#endif