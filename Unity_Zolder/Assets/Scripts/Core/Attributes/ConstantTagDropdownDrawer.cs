// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Attributes
{
	[CustomPropertyDrawer(typeof(ConstantTagAttribute))]
	public class ConstantTagDropdownDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ConstantTagAttribute tagAttribute = attribute as ConstantTagAttribute;

			if (tagAttribute.ConstType == null || tagAttribute.Types == null || tagAttribute.Types.Length == 0)
			{
				LogUtil.Warning(LogTags.DATA, this, "No filter was defined for the EntityTagAttribute! Please add a type with Tags defined as string constants.");
				return;
			}

			if (tagAttribute.ConstType == typeof(string))
			{
				StringOnGUI(position, property, label);
			}
			else if (tagAttribute.ConstType == typeof(int))
			{
				IntOnGUI(position, property, label);
			}
			else
			{
				LogUtil.Error(LogTags.DATA, this, "Type " + tagAttribute.ConstType.Name + " not supported, feel free to add it ;)");
			}
		}

		private void StringOnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			List<string> availableTags = new List<string>();
			ConstantTagAttribute tagAttribute = attribute as ConstantTagAttribute;
			ConstantTagUtils.GetAvailableTags<string>(tagAttribute, availableTags);

			if (availableTags.Count == 0)
			{
				LogUtil.Error(LogTags.DATA, this, "No valid values were available!");
				return;
			}

			string currentTag = property.stringValue;
			int selectedIndex = availableTags.IndexOf(currentTag);
			if (selectedIndex == -1)
			{
				if (!string.IsNullOrEmpty(currentTag))
				{
					LogUtil.Error(LogTags.DATA, this, "Invalid value '" + currentTag + "' selected in dropdown!" +
						" Converted to " + availableTags[0]);
				}

				selectedIndex = 0;
			}

			EditorGUI.BeginProperty(position, label, property);
			property.stringValue = availableTags[EditorGUI.Popup(position, label.text, selectedIndex, availableTags.ToArray())];
			EditorGUI.EndProperty();
		}

		private void IntOnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			List<string> availableTagStrings = new List<string>();
			ConstantTagAttribute tagAttribute = attribute as ConstantTagAttribute;
			List<int> availableTags = ConstantTagUtils.GetAvailableTags<int>(tagAttribute, availableTagStrings);

			int currentTag = property.intValue;
			int selectedIndex = availableTags.IndexOf(currentTag);
			if (selectedIndex == -1)
			{
				LogUtil.Error(LogTags.DATA, this, "Invalid value '" + currentTag + "' selected in dropdown!" +
					" Converted to " + availableTagStrings[0]);
				selectedIndex = 0;
			}

			EditorGUI.BeginProperty(position, label, property);
			property.intValue = availableTags[EditorGUI.Popup(position, label.text, selectedIndex, availableTagStrings.ToArray())];
			EditorGUI.EndProperty();
		}
	}
}