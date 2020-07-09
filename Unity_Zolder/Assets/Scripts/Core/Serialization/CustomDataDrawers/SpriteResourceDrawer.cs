// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public class SpriteResourceDrawer : ResourceDrawer
	{
		public override IEnumerable<string> Tags
		{
			get { yield return "spriteResource"; }
		}

		protected override Type ResourceType
		{
			get { return typeof(Sprite); }
		}
	}
}
#endif