// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using Talespin.Core.Foundation.Serialization.Drawer;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.AssetHandling
{
	/// <summary>
	/// Static drawer of GuidResources. Used when an editor scripts needs to manually
	/// render a resource field instead of via the property drawer.
	/// </summary>
	[SerializedClassDrawer(TargetType = typeof(string), TargetAttributeType = typeof(GuidResourceAttribute))]
	public class GuidResourceAttributeDrawer : ISerializedClassDrawer
	{
		public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
		{
			Color oldColor = GUI.contentColor;
			UnityEngine.Object current = null;

			if (!string.IsNullOrEmpty((string)value))
			{
				try
				{
					current = GuidDatabaseManager.Instance.MapGuidToObject((string)value, ((GuidResourceAttribute)attribute).BaseType);
				}
				catch { }

				if (current == null)
				{
					GUI.contentColor = Color.red;
				}
			}

			EditorGUI.BeginChangeCheck();
			UnityEngine.Object input = EditorGUILayout.ObjectField(label, current, ((GuidResourceAttribute)attribute).BaseType, false);
			GUI.contentColor = oldColor;

			bool dirty = EditorGUI.EndChangeCheck();

			string resourcePath = ResourceUtils.GetResourcePath(input);
			if (input != null)
			{
				if (string.IsNullOrEmpty(resourcePath))
				{
					EditorUtility.DisplayDialog("Weak resources", "Needs to be in a resource folder", "ok");
					return (string)value;
				}

				return GuidDatabaseManager.Instance.MapAssetToGuid(input);
			}

			if (!dirty)
			{
				return (string)value;
			}

			return string.Empty;
		}
	}
}
#endif
