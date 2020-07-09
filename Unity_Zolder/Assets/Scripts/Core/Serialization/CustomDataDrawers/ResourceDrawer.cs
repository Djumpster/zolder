// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.AssetHandling;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public abstract class ResourceDrawer : ICustomDataDrawer
	{
		public abstract IEnumerable<string> Tags { get; }

		protected abstract Type ResourceType { get; }

		public bool Draw(DataEntry entry)
		{
			if (entry.Type != DataEntry.DataType.String)
			{
				throw new Exception("[ResourceDrawer] This drawer only works on strings!");
			}
			DataEntry.StringEntry stringEntry = entry.Data as DataEntry.StringEntry;

			UnityEngine.Object current = string.IsNullOrEmpty(stringEntry.Value) ? null : Resources.Load(stringEntry.Value);
			UnityEngine.Object input = EditorGUILayout.ObjectField(current, ResourceType, false);
			string resourcePath = ResourceUtils.GetResourcePath(input);
			if (resourcePath == null)
			{
				resourcePath = string.Empty;
			}

			if (input != null && string.IsNullOrEmpty(resourcePath))
			{
				EditorUtility.DisplayDialog("ResourceDrawer", "Needs to be in a resource folder", "ok");
			}

			bool dirty = false;
			if (stringEntry.Value != resourcePath)
			{
				stringEntry.Value = resourcePath;
				dirty = true;
			}
			return dirty;
		}
	}
}
#endif