// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.Filter
{
	public class StringContainsFilter : IStringFilter
	{
		public StringFilterMatch[] Filter(string[] match, string input, bool caseSensitive)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}

			if (match.Length == 0)
			{
				return new StringFilterMatch[0];
			}

			if (string.IsNullOrEmpty(input))
			{
				StringFilterMatch[] result = new StringFilterMatch[match.Length];

				for (int i = 0; i < match.Length; i++)
				{
					result[i] = new StringFilterMatch(match[i]);
				}

				return result;
			}
			else
			{
				List<StringFilterMatch> result = new List<StringFilterMatch>(match.Length);

				for (int i = 0; i < match.Length; i++)
				{
					StringComparison comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
					int startIndex = match[i].LastIndexOf(input, comparison);

					if (startIndex != -1)
					{
						StringFilterMatch stringMatch = new StringFilterMatch(match[i]);
						stringMatch.AddMatch(startIndex, startIndex + input.Length);
						result.Add(stringMatch);
					}
				}

				return result.ToArray();
			}
		}
	}
}
