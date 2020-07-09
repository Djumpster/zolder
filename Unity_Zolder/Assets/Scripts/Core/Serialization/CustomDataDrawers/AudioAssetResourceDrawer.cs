// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Audio;

namespace Talespin.Core.Foundation.Serialization
{
	public class AudioAssetResourceDrawer : ResourceDrawer
	{
		public const string TAG = "AudioAsset";

		public override IEnumerable<string> Tags
		{
			get { yield return TAG; }
		}

		protected override Type ResourceType
		{
			get { return typeof(AudioAsset); }
		}
	}
}
#endif
