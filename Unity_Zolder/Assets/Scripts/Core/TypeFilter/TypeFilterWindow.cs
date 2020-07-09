// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using System;
using Talespin.Core.Foundation.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Talespin.Core.Foundation.Filter
{
#if UNITY_EDITOR
	/// <summary>
	/// An utilty window to improve type selection dropdowns.
	/// </summary>
	public class TypeFilterWindow : EditorWindow
	{
		private List<TypeFilterWindowTab> tabs;
		private Action<Type> selectionCallback;

		private TypeFilterWindowTab activeTab;
		
		private string searchTerm;

		private bool didFocusControl;

		protected void Awake()
		{
			searchTerm = string.Empty;
			didFocusControl = false;
		}

		protected void OnGUI()
		{
			if (tabs == null)
			{
				Close();
				return;
			}

			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
				{
					Event.current.Use();

					OnSelectCallback(activeTab.Selected);
					return;
				}

				if (Event.current.keyCode == KeyCode.Escape)
				{
					Event.current.Use();

					Close();
					return;
				}
			}
			else if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll")
			{
				Event.current.Use();

				TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
				textEditor.SelectAll();
			}
			else if (Event.current.type == EventType.KeyDown && GUI.GetNameOfFocusedControl() != "searchbox")
			{
				EditorGUI.FocusTextInControl("searchbox");
			}

			EditorGUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

			DrawSearchBox();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical();

			activeTab.OnGUI();

			EditorGUILayout.EndVertical();
		}

		/// <summary>
		/// Configure the window to filter by the given base type.
		/// </summary>
		/// <typeparam name="T">The target base type to filter on.</typeparam>
		/// <param name="title">The title of the window.</param>
		/// <param name="selectionCallback">Callback which is called when a selection has been made.</param>
		/// <param name="selected">The currently selected type, if any.</param>
		public void FilterBaseType<T>(string title, Action<Type> selectionCallback, Type selected = null)
		{
			FilterBaseType(title, typeof(T), selectionCallback, selected);
		}

		/// <summary>
		/// Configure the window to filter by the given base type.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="baseType">The target base type to filter on.</param>
		/// <param name="selectionCallback">Callback which is called when a selection has been made.</param>
		/// <param name="selected">The currently selected type, if any.</param>
		public void FilterBaseType(string title, Type baseType, Action<Type> selectionCallback, Type selected = null)
		{
			FilterBaseTypes(title, new Type[] { baseType }, selectionCallback, selected);
		}

		/// <summary>
		/// Configure the window to filter by the given base types.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="baseTypes">The target base types to filter on.</param>
		/// <param name="selectionCallback">Callback which is called when a selection has been made.</param>
		/// <param name="selected">The currently selected type, if any.</param>
		public void FilterBaseTypes(string title, Type[] baseTypes, Action<Type> selectionCallback, Type selected = null)
		{
			List<Type> allTypes = new List<Type>();

			for (int i = 0; i < baseTypes.Length; i++)
			{
				IEnumerable<Type> currentTypes = Reflect.AllTypesFrom(baseTypes[i]);

				foreach (Type currentType in currentTypes)
				{
					if (!allTypes.Contains(currentType))
					{
						allTypes.Add(currentType);
					}
				}
			}

			FilterSelection(title, allTypes.ToArray(), selectionCallback, selected);
		}

		public void FilterBaseTypeWithExclusions<T>(string title, Type[] exclusions, Action<Type> selectionCallback, Type selected = null)
		{
			FilterBaseTypeWithExclusions(title, typeof(T), exclusions, selectionCallback, selected);
		}

		public void FilterBaseTypeWithExclusions(string title, Type baseType, Type[] exclusions, Action<Type> selectionCallback, Type selected = null)
		{
			FilterBaseTypesWithExclusions(title, new Type[] { baseType }, exclusions, selectionCallback, selected);
		}

		public void FilterBaseTypesWithExclusions(string title, Type[] baseTypes, Type[] exclusions, Action<Type> selectionCallback, Type selected = null)
		{
			List<Type> allTypes = new List<Type>();

			for (int i = 0; i < baseTypes.Length; i++)
			{
				IEnumerable<Type> currentTypes = Reflect.AllTypesFrom(baseTypes[i]);

				foreach (Type currentType in currentTypes)
				{
					if (!allTypes.Contains(currentType))
					{
						bool exclude = false;

						foreach (Type exclusion in exclusions)
						{
							if (exclusion.IsAssignableFrom(currentType))
							{
								exclude = true;
								break;
							}
						}

						if (!exclude)
						{
							allTypes.Add(currentType);
						}
					}
				}
			}

			FilterSelection(title, allTypes.ToArray(), selectionCallback, selected);
		}

		/// <summary>
		/// Configure the window to filter by the given types.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="allTypes">The target types to filter on.</param>
		/// <param name="selectionCallback">Callback which is called when a selection has been made.</param>
		/// <param name="selected">The currently selected type, if any.</param>
		public void FilterSelection(string title, Type[] allTypes, Action<Type> selectionCallback, Type selected = null)
		{
			this.selectionCallback = selectionCallback;

			titleContent = new GUIContent(title);
			minSize = new Vector2(400, 350);
			maxSize = new Vector2(400, 500);

			tabs = new List<TypeFilterWindowTab>
			{
				new TypeFilterWindowTabSearch(allTypes, selected, OnSelectCallback),
				new TypeFilterWindowTabRecent(allTypes, selected, OnSelectCallback)
			};

			activeTab = tabs[1];
			NotifyTabsSearchTermChanged();
		}

		private void DrawSearchBox()
		{
			if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.UpArrow || Event.current.keyCode == KeyCode.DownArrow))
			{
				return;
			}

			GUI.SetNextControlName("searchbox");

			string newSearch = GUILayout.TextField(searchTerm, GUI.skin.FindStyle("ToolbarSeachTextField"));

			if (!string.IsNullOrEmpty(searchTerm) && GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
			{
				newSearch = "";
			}
			else if (string.IsNullOrEmpty(searchTerm))
			{
				GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty"));
			}

			if (newSearch != searchTerm)
			{
				searchTerm = newSearch;

				if (!string.IsNullOrEmpty(searchTerm))
				{
					activeTab = tabs[0];
				}
				else
				{
					activeTab = tabs[1];
				}

				NotifyTabsSearchTermChanged();
			}

			if (!didFocusControl)
			{
				didFocusControl = true;
				EditorGUI.FocusTextInControl("searchbox");
			}
		}

		private void NotifyTabsSearchTermChanged()
		{
			foreach (TypeFilterWindowTab tab in tabs)
			{
				tab.OnSearchTermUpdated(searchTerm);
			}
		}

		private void OnSelectCallback(Type type)
		{
			foreach (TypeFilterWindowTab tab in tabs)
			{
				tab.OnSelect(type);
			}

			selectionCallback.Invoke(type);
			Close();
		}
	}
#endif
}
