// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Filter
{
	public enum FuzzyStringComparisonOptions
	{
		UseHammingDistance,

		UseJaccardDistance,

		UseJaroDistance,

		UseJaroWinklerDistance,

		UseLevenshteinDistance,

		UseLongestCommonSubsequence,

		UseLongestCommonSubstring,

		UseNormalizedLevenshteinDistance,

		UseOverlapCoefficient,

		UseRatcliffObershelpSimilarity,

		UseSorensenDiceDistance,

		UseTanimotoCoefficient,

		CaseSensitive
	}
}
