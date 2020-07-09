// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Talespin.Core.Foundation.Filter
{
	public class TypeFilterWindowTabRecent : TypeFilterWindowTab
	{
#if UNITY_EDITOR
		[Serializable]
		private class ArrayWrapper
		{
			[SerializeField] public string[] v;

			public ArrayWrapper(string[] v)
			{
				this.v = v;
			}
		}

		private List<string> allRecents;
		private List<Type> recents;

		private Vector2 scrollPosition;

		public TypeFilterWindowTabRecent(Type[] allTypes, Type selected, Action<Type> onSelectCallback) : base(allTypes, selected, onSelectCallback)
		{
			string recentsString = EditorPrefs.GetString("TSW_Recents", "");

			ArrayWrapper wrapper = JsonUtility.FromJson<ArrayWrapper>(recentsString);
			allRecents = wrapper == null || wrapper.v == null ? new List<string>() : new List<string>(wrapper.v);

			recents = new List<Type>();
		}

		public override void OnSelect(Type type)
		{
			base.OnSelect(type);

			while (allRecents.Contains(type.FullName))
			{
				allRecents.Remove(type.FullName);
			}

			allRecents.Insert(0, type.FullName);

			string recentsString = JsonUtility.ToJson(new ArrayWrapper(allRecents.ToArray()));
			EditorPrefs.SetString("TSW_Recents", recentsString);
		}

		public override void OnGUI()
		{
			base.OnGUI();

			EditorGUILayout.BeginVertical();
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			foreach (Type type in recents)
			{
				DrawElement(type, type.Name);
			}

			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		public override void OnSearchTermUpdated(string searchTerm)
		{
			recents.Clear();

			Dictionary<string, Type> available = new Dictionary<string, Type>();
			foreach (Type type in allTypes)
			{
				available.Add(type.FullName, type);
			}

			foreach (string recent in allRecents)
			{
				if (ShouldFilterItem(recent))
				{
					continue;
				}

				if (available.ContainsKey(recent))
				{
					recents.Add(available[recent]);
				}
			}

			if (recents.Count > 0)
			{
				Selected = recents[0];
			}
		}
#endif
	}
}
