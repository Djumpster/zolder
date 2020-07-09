// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.LayerMasks
{
	/// <summary>
	/// Contains a reference to a scriptable object containing a layer mask configuration.
	/// Encapsulated in order to write a custom property drawer where you can modify both the linked-in scriptable object
	/// and the layer mask configured in it directly.
	/// </summary>
	[Serializable]
	public class LayerMaskConfig
	{
		public LayerMaskConfigData LayerMaskConfigData;

		public LayerMask LayerMask { get { return LayerMaskConfigData.LayerMask; } }

		public static implicit operator LayerMask(LayerMaskConfig config)
		{
			return config.LayerMaskConfigData.LayerMask;
		}

		public static implicit operator int(LayerMaskConfig config)
		{
			return config.LayerMaskConfigData.LayerMask;
		}
	}
}
