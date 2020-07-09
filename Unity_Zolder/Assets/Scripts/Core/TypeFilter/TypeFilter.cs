// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

/// <summary>
/// Utility class to filter a collection of types.
/// </summary>
namespace Talespin.Core.Foundation.Filter
{
	public class TypeFilter
	{
		/// <summary>
		/// Represents a single filter match. This struct is a wrapper around <see cref="StringFilterMatch"/> with support for Types.
		/// </summary>
		public struct Match
		{
			public Type Type;
			public StringFilterMatch StringMatch;

			public Match(Type type, StringFilterMatch stringMatch)
			{
				Type = type;
				StringMatch = stringMatch;
			}
		}

		private readonly IStringFilter stringFilter;

		public TypeFilter(IStringFilter stringFilter)
		{
			this.stringFilter = stringFilter;
		}

		/// <summary>
		/// Filter the specified type collection by the match parameter.
		/// </summary>
		/// <param name="source">A collection of types.</param>
		/// <param name="match">The string to match with.</param>
		/// <param name="includeNamespaces">Whether or not to include namespaces in the match.</param>
		/// <returns>A collection of matches.</returns>
		/// <seealso cref="Match"/>
		public Match[] Filter(Type[] source, string match, bool includeNamespaces = true)
		{
			string[] filterSource = new string[source.Length];

			for (int i = 0; i < source.Length; i++)
			{
				Type type = source[i];
				string name = includeNamespaces ? type.FullName : type.Name;

				filterSource[i] = name + "\\" + i;
			}

			StringFilterMatch[] filterResult = stringFilter.Filter(filterSource, match, false);
			Match[] result = new Match[filterResult.Length];

			for (int i = 0; i < filterResult.Length; i++)
			{
				string[] split = filterResult[i].String.Split('\\');
				int index = int.Parse(split[1]);

				StringFilterMatch newMatch = new StringFilterMatch(filterResult[i].String.Substring(0, filterResult[i].String.IndexOf("\\")));

				for (int j = 0; j < filterResult[i].Matches.Length; j++)
				{
					newMatch.AddMatch(filterResult[i].Matches[j].MatchStart, filterResult[i].Matches[j].MatchEnd);
				}

				result[i] = new Match(source[index], newMatch);
			}

			return result;
		}
	}
}
