// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Attributes
{
	[CustomPropertyDrawer(typeof(GameObjectTagAttribute))]
	public class GameObjectTagDropdownDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			if (string.IsNullOrEmpty(property.stringValue))
			{
				property.stringValue = "Untagged";
			}

			property.stringValue = EditorGUI.TagField(position, label, property.stringValue);

			EditorGUI.EndProperty();
		}
	}
}
