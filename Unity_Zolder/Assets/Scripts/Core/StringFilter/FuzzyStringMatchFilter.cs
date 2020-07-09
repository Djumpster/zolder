// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.Filter
{
	public class FuzzyStringMatchFilter : IStringFilter
	{
		private readonly FuzzyStringComparisonOptions[] options;

		public FuzzyStringMatchFilter(params FuzzyStringComparisonOptions[] options)
		{
			this.options = options;
		}

		/// <summary>
		/// Filter a collection of strings using the specified options.
		/// </summary>
		/// <param name="match">The target strings to filter.</param>
		/// <param name="input">The input string.</param>
		/// <param name="caseSensitive">Whether or not the filter should be case sensitive.</param>
		/// <returns>A collection of string matches.</returns>
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
						continue;
					}

					if (match[i].ApproximatelyEquals(input, FuzzyStringComparisonTolerance.Strong, options))
					{
						StringFilterMatch stringMatch = new StringFilterMatch(match[i]);
						int searchIndexStart = 0;

						for (int j = 0; j < input.Length; j++)
						{
							char c = input[j];
							int index = match[i].IndexOf(c, searchIndexStart);

							if (index != -1)
							{
								if (j < input.Length - 1)
								{
									char n = input[j + 1];
									int nindex = match[i].IndexOf(n, searchIndexStart);

									if (index > nindex)
									{
										continue;
									}
								}

								stringMatch.AddMatch(index, index + 1);
								searchIndexStart = index + 1;
							}
						}

						result.Add(stringMatch);
					}
				}

				return result.ToArray();
			}
		}
	}
}
