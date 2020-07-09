// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Graphics;
using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class ColorExtensions
	{
		public static Color Invert(this Color orig)
		{
			return new Color(1f - orig.r, 1f - orig.g, 1f - orig.b, 1f - orig.a);
		}

		public static HSBAColor ToHSBA(this Color orig)
		{
			return orig;
		}

		public static Color RandomHueColor
		{
			get
			{
				return new HSBAColor(Random.value, 0.8f, 1f, 1f);
			}
		}

		public static Color ColorFromHashCode(int hashCode, float saturation = 1f, float brightness = 1f, float alpha = 1f)
		{
			System.Random random = new System.Random(hashCode);

			return new HSBAColor((float)random.NextDouble(), saturation, brightness, alpha);
		}
	}
}
