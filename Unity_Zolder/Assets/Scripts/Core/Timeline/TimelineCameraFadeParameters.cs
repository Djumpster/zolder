// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// simple data container for Camera fade parameters as used in <see cref="TimelineCameraFadeAsset"/>
	/// </summary>
	[Serializable]
	public class TimelineCameraFadeParameters
	{
		public Color FromColor => fromColor;
		public Color ToColor => toColor;
		public bool AlsoFadeUI => alsoFadeUI;

		[SerializeField] private Color fromColor;
		[SerializeField] private Color toColor;
		[SerializeField] private bool alsoFadeUI;

		public TimelineCameraFadeParameters(Color fromColor, Color toColor, bool alsoFadeUI)
		{
			this.fromColor = fromColor;
			this.toColor = toColor;
			this.alsoFadeUI = alsoFadeUI;
		}
	}
}
