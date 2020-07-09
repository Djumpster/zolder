// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Talespin.Core.Foundation.Filter
{
	public class TypeFilterWindowTabSearch : TypeFilterWindowTab
	{
#if UNITY_EDITOR
		private readonly TypeFilter typeFilter;

		private TypeFilter.Match[] typeFilterCache;
		private string[] formattedTypeNameCache;

		private Vector2 scrollPosition;

		private string searchTerm;

		public TypeFilterWindowTabSearch(Type[] allTypes, Type selected, Action<Type> onSelectCallback) : base(allTypes, selected, onSelectCallback)
		{
			typeFilter = new TypeFilter(new FuzzyStringMatchFilter(FuzzyStringComparisonOptions.UseLongestCommonSubsequence, FuzzyStringComparisonOptions.CaseSensitive));
		}

		public override void OnGUI()
		{
			base.OnGUI();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				EditorGUILayout.HelpBox("Don't see the type you're looking for? Ensure that it resides in a Talespin namespace.", MessageType.Info);
			}

			DrawScrollView();
		}

		public override void OnSearchTermUpdated(string searchTerm)
		{
			TypeFilter.Match[] filteredTypes = typeFilter.Filter(allTypes, searchTerm, false);

			List<TypeFilter.Match> newCache = new List<TypeFilter.Match>(filteredTypes.Length);

			bool containsSelected = false;

			for (int i = 0; i < filteredTypes.Length; i++)
			{
				string fullName = filteredTypes[i].Type.FullName;

				if (ShouldFilterItem(fullName))
				{
					continue;
				}

				if (!containsSelected && filteredTypes[i].Type == Selected)
				{
					containsSelected = true;
				}

				newCache.Add(filteredTypes[i]);
			}

			newCache.Sort((e1, e2) => e1.Type.FullName.CompareTo(e2.Type.FullName));

			if (!containsSelected)
			{
				Selected = null;

				if (newCache.Count > 0)
				{
					Selected = newCache[0].Type;
				}
			}

			this.searchTerm = searchTerm;
			this.typeFilterCache = newCache.ToArray();

			UpdateFormattingCache();
		}

		private void DrawScrollView()
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			for (int i = 0; i < typeFilterCache.Length; i++)
			{
				DrawElement(typeFilterCache[i].Type, formattedTypeNameCache[i]);
			}

			EditorGUILayout.EndScrollView();
		}
		
		private void UpdateFormattingCache()
		{
			formattedTypeNameCache = new string[typeFilterCache.Length];

			for (int i = 0; i < typeFilterCache.Length; i++)
			{
				formattedTypeNameCache[i] = ApplyFormatting(typeFilterCache[i]);
			}
		}

		private string ApplyFormatting(TypeFilter.Match match)
		{
			const string HIGHLIGHT_START = "<b>";
			const string HIGHLIGHT_END = "</b>";

			string result = match.StringMatch.String;
			int offset = 0;

			StringFilterMatch.Sequence[] sequences = match.StringMatch.Matches;

			for (int i = 0; i < sequences.Length; i++)
			{
				int start = sequences[i].MatchStart + offset;
				offset += HIGHLIGHT_START.Length;

				int end = sequences[i].MatchEnd + offset;
				offset += HIGHLIGHT_END.Length;

				result = result.Insert(start, HIGHLIGHT_START);
				result = result.Insert(end, HIGHLIGHT_END);
			}

			return result;
		}
#endif
	}
}
