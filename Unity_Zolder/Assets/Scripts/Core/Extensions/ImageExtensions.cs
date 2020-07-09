// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;

namespace Talespin.Core.Foundation.Extensions
{
	public static class ImageExtensions
	{
		public static Color SetAlpha(this Image image, float alpha)
		{
			Color c = image.color;
			return image.color = new Color(c.r, c.g, c.b, alpha);
		}
	}
}
