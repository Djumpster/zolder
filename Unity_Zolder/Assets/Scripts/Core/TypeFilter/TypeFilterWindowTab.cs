// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Talespin.Core.Foundation.Filter
{
	public abstract class TypeFilterWindowTab
	{
#if UNITY_EDITOR
		public Type Selected { protected set; get; }

		protected readonly Type[] allTypes;

		private readonly Action<Type> onSelectCallback;

		private readonly List<Type> lastDrawn;

		private readonly GUIStyle classNameStyle;
		private readonly GUIStyle namespaceStyle;

		public TypeFilterWindowTab(Type[] allTypes, Type selected, Action<Type> onSelectCallback)
		{
			this.allTypes = allTypes;
			this.onSelectCallback = onSelectCallback;

			foreach (Type type in allTypes)
			{
				if (type == selected)
				{
					Selected = selected;
					break;
				}
			}

			lastDrawn = new List<Type>();

			classNameStyle = new GUIStyle(EditorStyles.label);
			classNameStyle.richText = true;

			namespaceStyle = new GUIStyle(EditorStyles.label);
			namespaceStyle.richText = true;
		}

		public virtual void OnGUI()
		{
			Event evt = Event.current;

			if (evt.type == EventType.KeyDown && Selected != null)
			{
				int dir = evt.keyCode == KeyCode.DownArrow ? 1 : evt.keyCode == KeyCode.UpArrow ? -1 : 0;

				if (dir != 0)
				{
					evt.Use();

					int index = lastDrawn.IndexOf(Selected);
					int newIndex = index + dir;

					if (newIndex >= 0 && newIndex < lastDrawn.Count)
					{
						Selected = lastDrawn[newIndex];
					}
				}
			}

			lastDrawn.Clear();
		}

		public virtual void OnSelect(Type type)
		{
		}

		public virtual void OnSearchTermUpdated(string searchTerm)
		{
		}

		protected Rect DrawElement(Type type, string text)
		{
			lastDrawn.Add(type);

			Color originalBackgroundColor = GUI.backgroundColor;

			if (type == Selected)
			{
				GUI.backgroundColor = new Color32(62, 125, 231, 128);
			}

			Rect rect = EditorGUILayout.BeginHorizontal(GUI.skin.box);

			string namespaceText = GetNamespaceText(type);
			text += "\nin " + namespaceText;

			GUILayout.Label(text, classNameStyle);

			if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
			{
				onSelectCallback.Invoke(type);
			}

			EditorGUILayout.EndHorizontal();

			GUI.backgroundColor = originalBackgroundColor;

			return rect;
		}

		protected bool ShouldFilterItem(string fullName)
		{
			if (fullName.StartsWith("Talespin."))
			{
				return false;
			}

			if (!fullName.Contains("."))
			{
				return true;
			}

			return true;
		}

		private string GetNamespaceText(Type type)
		{
			string result = string.IsNullOrEmpty(type.Namespace) ? string.Empty : type.Namespace;

			result = result.Replace("Talespin.Core.", string.Empty);
			result = result.Replace("Talespin.", string.Empty);

			return string.IsNullOrEmpty(result) ? "Default Namespace" : result;
		}
#endif
	}
}
