// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Attributes;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.SceneLoading;
using Talespin.Core.Foundation.Serialization.Drawer;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Attributes
{
	[SerializedClassDrawer(TargetType = typeof(string), TargetAttributeType = typeof(SceneAttribute))]
	public class SceneAttributeDrawer : ISerializedClassDrawer
	{
		private readonly bool allowNone = false;

		public SceneAttributeDrawer(bool allowNone = false) : base()
		{
			this.allowNone = allowNone;
		}

		public SceneAttributeDrawer() : base()
		{
		}

		public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
		{
			List<string> availableScenes = SceneManagementUtils.GetAllScenesInBuild();

			if (availableScenes.Count == 0)
			{
				EditorGUILayout.LabelField("No scenes have been included in the build settings");
				return null;
			}
			if (allowNone)
			{
				availableScenes.Insert(0, "None");
			}
			string currentTag = (string)value;
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

			string result = availableScenes[EditorGUILayout.Popup(label, selectedIndex, availableScenes.ToArray())];
			GUI.color = oldColor;

			return result;
		}
	}
}
#endif
