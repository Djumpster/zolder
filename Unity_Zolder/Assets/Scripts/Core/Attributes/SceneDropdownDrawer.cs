// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.SceneLoading;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Attributes
{
	[CustomPropertyDrawer(typeof(SceneAttribute))]
	public class SceneDropdownDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			StringOnGUI(position, property, label);
		}

		private void StringOnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			List<string> availableScenes = SceneManagementUtils.GetAllScenesInBuild();

			if (availableScenes.Count == 0)
			{
				return;
			}

			EditorGUI.BeginProperty(position, label, property);

			string currentTag = property.stringValue;
			int selectedIndex = availableScenes.IndexOf(currentTag);
			Color oldColor = GUI.color;
			if (selectedIndex == -1)
			{
				selectedIndex = 0;

				if (!string.IsNullOrEmpty(currentTag))
				{
					LogUtil.Error(LogTags.SYSTEM, this, "Scene " + currentTag + " selected but it is not in the build settings!");
					availableScenes.Insert(0, currentTag);
					GUI.color = Color.red;
				}
			}
			property.stringValue = availableScenes[EditorGUI.Popup(position, label.text, selectedIndex, availableScenes.ToArray())];
			GUI.color = oldColor;
			EditorGUI.EndProperty();
		}
	}
}
