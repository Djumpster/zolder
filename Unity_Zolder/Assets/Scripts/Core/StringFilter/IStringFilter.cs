// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Filter
{
	/// <summary>
	/// Represents a string filter match, can contain multiple matched sequences for a single string.
	/// </summary>
	public class StringFilterMatch
	{
		/// <summary>
		/// Represents a single string filter match sequence. This is a single continuous match.
		/// </summary>
		public struct Sequence
		{
			public string Match;

			public int MatchStart;
			public int MatchEnd;

			public Sequence(StringFilterMatch parent, int start, int end)
			{
				Match = parent.String.Substring(start, end - start);
				MatchStart = start;
				MatchEnd = end;
			}
		}

		public Sequence[] Matches
		{
			get
			{
				if (dirty)
				{
					matchesCache = matches.ToArray();
					dirty = false;
				}

				return matchesCache;
			}
		}

		public readonly string String;

		private List<Sequence> matches;
		private Sequence[] matchesCache;

		private bool dirty;

		public StringFilterMatch(string source)
		{
			String = source;
			matches = new List<Sequence>();
			dirty = true;
		}

		public void AddMatch(int start, int end)
		{
			if (start < String.Length && end <= String.Length)
			{
				Sequence sequence = new Sequence(this, start, end);
				matches.Add(sequence);
				dirty = true;
			}
		}
	}

	/// <summary>
	/// Base interface for a string filter.
	/// </summary>
	public interface IStringFilter
	{
		StringFilterMatch[] Filter(string[] match, string input, bool caseSensitive = false);
	}
}
