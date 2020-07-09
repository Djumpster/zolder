// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Maths
{
	/// <summary>
	/// An enum of all available easing types.
	/// Visit <see href="https://easings.net"/> for a visual representation
	/// of all easing functions.
	/// </summary>
	public enum EasingType
	{
		Linear,
		EaseInSine,			// https://easings.net/en#easeInSine
		EaseOutSine,		// https://easings.net/en#easeOutSine
		EaseInOutSine,		// https://easings.net/en#easeInOutSine
		EaseInCubic,		// https://easings.net/en#easeInCubic
		EaseOutCubic,		// https://easings.net/en#easeOutCubic
		EaseInOutCubic,		// https://easings.net/en#easeInOutCubic
		EaseInQuint,		// https://easings.net/en#easeInQuint
		EaseOutQuint,		// https://easings.net/en#easeOutQuint
		EaseInOutQuint,		// https://easings.net/en#easeInOutQuint
		EaseInCirc,			// https://easings.net/en#easeInCirc
		EaseOutCirc,		// https://easings.net/en#easeOutCirc
		EaseInOutCirc,		// https://easings.net/en#easeInOutCirc
		EaseInElastic,      // https://easings.net/en#easeInElastic
		EaseOutElastic,     // https://easings.net/en#easeOutElastic
		EaseInOutElastic,	// https://easings.net/en#easeInOutElastic
		EaseInQuad,         // https://easings.net/en#easeInQuad
		EaseOutQuad,        // https://easings.net/en#easeOutQuad
		EaseInOutQuad,      // https://easings.net/en#easeInOutQuad
		EaseInQuart,        // https://easings.net/en#easeInQuart
		EaseOutQuart,       // https://easings.net/en#easeOutQuart
		EaseInOutQuart,     // https://easings.net/en#easeInOutQuart
		EaseInExpo,         // https://easings.net/en#easeInExpo
		EaseOutExpo,        // https://easings.net/en#easeOutExpo
		EaseInOutExpo,      // https://easings.net/en#easeInOutExpo
		EaseInBack,         // https://easings.net/en#easeInBack
		EaseOutBack,        // https://easings.net/en#easeOutBack
		EaseInOutBack,      // https://easings.net/en#easeInOutBack
		EaseInBounce,       // https://easings.net/en#easeInBounce
		EaseOutBounce,		// https://easings.net/en#easeOutBounce
		EaseInOutBounce     // https://easings.net/en#easeInOutBounce
	}

	/// <summary>
	/// A collection of programmatic easing functions
	/// </summary>
	public static class Easing
	{
		/// <summary>
		/// A common delegate that all easing functions implement.
		/// </summary>
		/// <param name="start">The start value of the ease</param>
		/// <param name="end">The target value of the ease</param>
		/// <param name="time">The current progress of the ease, in the range [0-1]</param>
		/// <returns>The value of the ease.</returns>
		public delegate float EasingFunction(float start, float end, float time);

		private static readonly Dictionary<EasingType, EasingFunction> easingFunctionLookup;

		static Easing()
		{
			easingFunctionLookup = new Dictionary<EasingType, EasingFunction>()
			{
				{ EasingType.Linear, Linear },
				{ EasingType.EaseInSine, EaseInSine },
				{ EasingType.EaseOutSine, EaseOutSine },
				{ EasingType.EaseInOutSine, EaseInOutSine },
				{ EasingType.EaseInCubic, EaseInCubic},
				{ EasingType.EaseOutCubic, EaseOutCubic },
				{ EasingType.EaseInOutCubic, EaseInOutCubic },
				{ EasingType.EaseInQuint, EaseInQuint },
				{ EasingType.EaseOutQuint, EaseOutQuint },
				{ EasingType.EaseInOutQuint, EaseInOutQuint },
				{ EasingType.EaseInCirc, EaseInCirc },
				{ EasingType.EaseOutCirc, EaseOutCirc },
				{ EasingType.EaseInOutCirc, EaseInOutCirc },
				{ EasingType.EaseInElastic, EaseInElastic },
				{ EasingType.EaseOutElastic, EaseOutElastic },
				{ EasingType.EaseInOutElastic, EaseInOutElastic },
				{ EasingType.EaseInQuad, EaseInQuad },
				{ EasingType.EaseOutQuad, EaseInOutQuad },
				{ EasingType.EaseInOutQuad, EaseInOutQuad },
				{ EasingType.EaseInQuart, EaseInQuart },
				{ EasingType.EaseOutQuart, EaseOutQuart },
				{ EasingType.EaseInOutQuart, EaseInOutQuart },
				{ EasingType.EaseInExpo, EaseInExpo },
				{ EasingType.EaseOutExpo, EaseOutExpo },
				{ EasingType.EaseInOutExpo, EaseInOutExpo },
				{ EasingType.EaseInBack, EaseInBack },
				{ EasingType.EaseOutBack, EaseOutBack },
				{ EasingType.EaseInOutBack, EaseInOutBack },
				{ EasingType.EaseInBounce, EaseInBounce },
				{ EasingType.EaseOutBounce, EaseOutBounce },
				{ EasingType.EaseInOutBounce, EaseInOutBounce }
			};
		}

		public static float Ease(EasingType type, float start, float end, float time) => easingFunctionLookup[type].Invoke(start, end, time);

		#region Linear
		public static float Linear(float start, float end, float value) => Mathf.Lerp(start, end, value);
		#endregion

		#region Sine
		public static float EaseInSine(float start, float end, float value)
		{
			end -= start;
			return (-end * Mathf.Cos(value * (Mathf.PI * 0.5f))) + end + start;
		}

		public static float EaseOutSine(float start, float end, float value)
		{
			end -= start;
			return (end * Mathf.Sin(value * (Mathf.PI * 0.5f))) + start;
		}

		public static float EaseInOutSine(float start, float end, float value)
		{
			end -= start;
			return (-end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1)) + start;
		}
		#endregion

		#region Cubic
		public static float EaseInCubic(float start, float end, float value)
		{
			end -= start;
			return (end * value * value * value) + start;
		}

		public static float EaseOutCubic(float start, float end, float value)
		{
			value--;
			end -= start;
			return (end * ((value * value * value) + 1)) + start;
		}

		public static float EaseInOutCubic(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
			{
				return (end * 0.5f * value * value * value) + start;
			}

			value -= 2;
			return (end * 0.5f * ((value * value * value) + 2)) + start;
		}
		#endregion

		#region Quint
		public static float EaseInQuint(float start, float end, float value)
		{
			end -= start;
			return (end * value * value * value * value * value) + start;
		}

		public static float EaseOutQuint(float start, float end, float value)
		{
			value--;
			end -= start;
			return (end * ((value * value * value * value * value) + 1)) + start;
		}

		public static float EaseInOutQuint(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
			{
				return (end * 0.5f * value * value * value * value * value) + start;
			}

			value -= 2;
			return (end * 0.5f * ((value * value * value * value * value) + 2)) + start;
		}
		#endregion

		#region Circ
		public static float EaseInCirc(float start, float end, float value)
		{
			end -= start;
			return (-end * (Mathf.Sqrt(1 - (value * value)) - 1)) + start;
		}

		public static float EaseOutCirc(float start, float end, float value)
		{
			value--;
			end -= start;
			return (end * Mathf.Sqrt(1 - (value * value))) + start;
		}

		public static float EaseInOutCirc(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
			{
				return (-end * 0.5f * (Mathf.Sqrt(1 - (value * value)) - 1)) + start;
			}

			value -= 2;
			return (end * 0.5f * (Mathf.Sqrt(1 - (value * value)) + 1)) + start;
		}
		#endregion

		#region Elastic
		public static float EaseInElastic(float start, float end, float value)
		{
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float a = 0;

			if (value == 0)
			{
				return start;
			}

			if ((value /= d) == 1)
			{
				return start + end;
			}

			float s;
			if (a == 0f || a < Mathf.Abs(end))
			{
				a = end;
				s = p / 4;
			}
			else
			{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin(((value * d) - s) * (2 * Mathf.PI) / p)) + start;
		}

		public static float EaseOutElastic(float start, float end, float value)
		{
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float a = 0;

			if (value == 0)
			{
				return start;
			}

			if ((value /= d) == 1)
			{
				return start + end;
			}

			float s;
			if (a == 0f || a < Mathf.Abs(end))
			{
				a = end;
				s = p * 0.25f;
			}
			else
			{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin(((value * d) - s) * (2 * Mathf.PI) / p)) + end + start;
		}

		public static float EaseInOutElastic(float start, float end, float value)
		{
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float a = 0;

			if (value == 0)
			{
				return start;
			}

			if ((value /= d * 0.5f) == 2)
			{
				return start + end;
			}

			float s;
			if (a == 0f || a < Mathf.Abs(end))
			{
				a = end;
				s = p / 4;
			}
			else
			{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			if (value < 1)
			{
				return (-0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin(((value * d) - s) * (2 * Mathf.PI) / p))) + start;
			}

			return (a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin(((value * d) - s) * (2 * Mathf.PI) / p) * 0.5f) + end + start;
		}
		#endregion

		#region Quad
		public static float EaseInQuad(float start, float end, float value)
		{
			end -= start;
			return (end * value * value) + start;
		}

		public static float EaseOutQuad(float start, float end, float value)
		{
			end -= start;
			return (-end * value * (value - 2)) + start;
		}

		public static float EaseInOutQuad(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
			{
				return (end * 0.5f * value * value) + start;
			}

			value--;
			return (-end * 0.5f * ((value * (value - 2)) - 1)) + start;
		}
		#endregion

		#region Quart
		public static float EaseInQuart(float start, float end, float value)
		{
			end -= start;
			return (end * value * value * value * value) + start;
		}

		public static float EaseOutQuart(float start, float end, float value)
		{
			value--;
			end -= start;
			return (-end * ((value * value * value * value) - 1)) + start;
		}

		public static float EaseInOutQuart(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
			{
				return (end * 0.5f * value * value * value * value) + start;
			}

			value -= 2;
			return (-end * 0.5f * ((value * value * value * value) - 2)) + start;
		}
		#endregion

		#region Expo
		public static float EaseInExpo(float start, float end, float value)
		{
			end -= start;
			return (end * Mathf.Pow(2, 10 * (value - 1))) + start;
		}

		public static float EaseOutExpo(float start, float end, float value)
		{
			end -= start;
			return (end * (-Mathf.Pow(2, -10 * value) + 1)) + start;
		}

		public static float EaseInOutExpo(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
			{
				return (end * 0.5f * Mathf.Pow(2, 10 * (value - 1))) + start;
			}

			value--;
			return (end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2)) + start;
		}
		#endregion

		#region Back
		public static float EaseInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1;
			float s = 1.70158f;
			return (end * value * value * (((s + 1) * value) - s)) + start;
		}

		public static float EaseOutBack(float start, float end, float value)
		{
			float s = 1.70158f;
			end -= start;
			value -= 1;
			return (end * ((value * value * (((s + 1) * value) + s)) + 1)) + start;
		}

		public static float EaseInOutBack(float start, float end, float value)
		{
			float s = 1.70158f;
			end -= start;
			value /= .5f;
			if (value < 1)
			{
				s *= 1.525f;
				return (end * 0.5f * (value * value * (((s + 1) * value) - s))) + start;
			}
			value -= 2;
			s *= 1.525f;
			return (end * 0.5f * ((value * value * (((s + 1) * value) + s)) + 2)) + start;
		}
		#endregion

		#region Bounce
		public static float EaseInBounce(float start, float end, float value)
		{
			end -= start;
			float d = 1f;
			return end - EaseOutBounce(0, end, d - value) + start;
		}

		public static float EaseOutBounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < (1 / 2.75f))
			{
				return (end * (7.5625f * value * value)) + start;
			}
			else if (value < (2 / 2.75f))
			{
				value -= 1.5f / 2.75f;
				return (end * ((7.5625f * value * value) + .75f)) + start;
			}
			else if (value < (2.5 / 2.75))
			{
				value -= 2.25f / 2.75f;
				return (end * ((7.5625f * value * value) + .9375f)) + start;
			}
			else
			{
				value -= 2.625f / 2.75f;
				return (end * ((7.5625f * value * value) + .984375f)) + start;
			}
		}

		public static float EaseInOutBounce(float start, float end, float value)
		{
			end -= start;
			float d = 1f;
			if (value < d * 0.5f)
			{
				return (EaseInBounce(0, end, value * 2) * 0.5f) + start;
			}
			else
			{
				return (EaseOutBounce(0, end, (value * 2) - d) * 0.5f) + (end * 0.5f) + start;
			}
		}
		#endregion
	}
}
