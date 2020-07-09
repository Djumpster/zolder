// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Talespin.Core.Foundation.Serialization
{
	public class NormalizedFloatDrawer : ICustomDataDrawer
	{
		public IEnumerable<string> Tags
		{
			get { yield return "normalized"; }
		}

		public bool Draw(DataEntry entry)
		{
			if (entry.Type != DataEntry.DataType.Float)
			{
				throw new Exception("[NormalizedFloatDrawer] This drawer only works on floats!");
			}
			DataEntry.FloatEntry floatEntry = entry.Data as DataEntry.FloatEntry;
			float newVal = EditorGUILayout.Slider(floatEntry.Value, 0f, 1f);

			bool dirty = false;
			if (floatEntry.Value != newVal)
			{
				floatEntry.Value = newVal;
				dirty = true;
			}
			return dirty;
		}
	}
}
#endif