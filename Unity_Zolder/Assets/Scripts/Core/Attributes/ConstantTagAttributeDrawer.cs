// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Attributes;
using Talespin.Core.Foundation.Serialization.Drawer;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Attributes
{
	[SerializedClassDrawer(TargetType = typeof(string), TargetAttributeType = typeof(ConstantTagAttribute))]
	public class ConstantTagAttributeDrawer : ISerializedClassDrawer
	{
		public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
		{
			Debug.Log("Shalalali");
			List<string> availableTags = new List<string>();

			if (ConstantTagUtils.GetAvailableTags<string>((ConstantTagAttribute)attribute, availableTags).Count > 0)
			{
				string currentTag = (string)value;
				int selectedIndex = availableTags.IndexOf(currentTag);
				if (selectedIndex == -1)
				{
					selectedIndex = 0;
				}

				return availableTags[EditorGUILayout.Popup(label, selectedIndex, availableTags.ToArray())];
			}
			else
			{
				GUI.color = Color.red + Color.white * .5f;
				EditorGUILayout.LabelField(label, new GUIContent(((ConstantTagAttribute)attribute).ConstType + " has no constants!"));
				GUI.color = Color.white;
				return "";
			}
		}
	}
}
#endif
