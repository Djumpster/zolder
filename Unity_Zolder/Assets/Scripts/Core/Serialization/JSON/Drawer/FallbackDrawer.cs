// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization.Drawer
{
	public class FallbackDrawer : ISerializedClassDrawer
	{
		public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
		{
			if (typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				EditorGUILayout.LabelField(label, new GUIContent("Unity types are not supported instead use GuidResourceAttribute."));
			}
			else
			{
				EditorGUILayout.LabelField(label, new GUIContent("No drawer available for this type."));
			}

			return value;
		}
	}
}
#endif
