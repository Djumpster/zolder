// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Text;
using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	/// <summary>
	/// This struct represents a color in HSBA space. It can be converted implicitly to and from RGBA
	/// </summary>
	public struct HSBAColor
	{
		public readonly static HSBAColor white = new HSBAColor(Color.white);
		public readonly static HSBAColor blue = new HSBAColor(Color.blue);
		public readonly static HSBAColor green = new HSBAColor(Color.green);
		public readonly static HSBAColor yellow = new HSBAColor(Color.yellow);
		public readonly static HSBAColor red = new HSBAColor(Color.red);
		public readonly static HSBAColor cyan = new HSBAColor(Color.cyan);
		public readonly static HSBAColor magenta = new HSBAColor(Color.magenta);
		public readonly static HSBAColor black = new HSBAColor(Color.black);

		private float hue;
		private float saturation;
		private float brightness;
		private float alpha;

		public float Hue
		{
			get
			{
				return hue;
			}
			set
			{
				hue = value;
				hue %= 1f;
				if (hue < 0)
				{
					hue = 1f + hue;
				}
			}
		}

		public float Saturation
		{
			get
			{
				return saturation;
			}
			set
			{
				saturation = Mathf.Clamp01(value);
			}
		}

		public float Brightness
		{
			get
			{
				return brightness;
			}
			set
			{
				brightness = Mathf.Clamp01(value);
			}
		}

		public float Alpha
		{
			get
			{
				return alpha;
			}
			set
			{
				alpha = Mathf.Clamp01(value);
			}
		}

		public HSBAColor(float hue, float saturation, float brightness, float alpha)
		{
			this.hue = hue;
			this.saturation = saturation;
			this.brightness = brightness;
			this.alpha = alpha;
		}

		/// <summary>
		/// Convert a RGBA color to HSBA.
		/// </summary>
		/// <param name="c">The RGBA color to convert.</param>
		public HSBAColor(Color c)
		{
			float maxColor = Mathf.Max(Mathf.Max(c.r, c.g), c.b);
			float minColor = Mathf.Min(Mathf.Min(c.r, c.g), c.b);
			float delta = (maxColor - minColor);

			hue = 0f;
			brightness = maxColor;
			saturation = 0f;

			if (maxColor != 0f)
			{
				saturation = delta / maxColor;
			}
			if (saturation != 0)
			{
				if (c.r == maxColor)
				{
					hue = (c.g - c.b) / delta;
				}
				else
				if (c.g == maxColor)
				{
					hue = 2f + (c.b - c.r) / delta;
				}
				else
				if (c.b == maxColor)
				{
					hue = 4f + (c.r - c.g) / delta;
				}
			}
			else
			{
				hue = 0f;
			}
			hue /= 6f;
			if (hue < 0)
			{
				hue = 1f + hue;
			}
			alpha = c.a;
		}

		/// <summary>
		/// Convert from HSBA to RGBA.
		/// </summary>
		/// <returns>A RGBA color.</returns>
		public Color ToRGB()
		{
			float chroma = saturation * brightness;
			float hueA = hue * 6f;
			float x = chroma * (1f - Mathf.Abs(hueA % 2f - 1f));
			float m = brightness - chroma;
			Color ret = Color.clear;

			if (hueA >= 0f && hueA < 1f)
			{
				ret = new Color(chroma, x, 0, alpha);
			}
			else
			if (hueA >= 1f && hueA < 2f)
			{
				ret = new Color(x, chroma, 0, alpha);
			}
			else
			if (hueA >= 2f && hueA < 3f)
			{
				ret = new Color(0, chroma, x, alpha);
			}
			else
			if (hueA >= 3f && hueA < 4f)
			{
				ret = new Color(0, x, chroma, alpha);
			}
			else
			if (hueA >= 4f && hueA < 5f)
			{
				ret = new Color(x, 0, chroma, alpha);
			}
			else
			if (hueA >= 5f && hueA <= 6f)
			{
				ret = new Color(chroma, 0, x, alpha);
			}
			ret.r += m;
			ret.g += m;
			ret.b += m;
			return ret;
		}

		public static implicit operator Color(HSBAColor col)
		{
			return col.ToRGB();
		}

		public static implicit operator HSBAColor(Color col)
		{
			return new HSBAColor(col);
		}

		public override string ToString()
		{
			var builder = new StringBuilder("(hue: ");
			builder.Append(hue);
			builder.Append(", sat: ");
			builder.Append(saturation);
			builder.Append(", brightness: ");
			builder.Append(brightness);
			builder.Append(", alpha: ");
			builder.Append(alpha);
			builder.Append(")");
			return builder.ToString();
		}
	}
}