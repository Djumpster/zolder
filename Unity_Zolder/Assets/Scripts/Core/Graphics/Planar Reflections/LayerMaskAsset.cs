// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	public class LayerMaskAsset : ScriptableObject
	{
		public LayerMask Mask { get { return layerMask; } }

		[SerializeField] private LayerMask layerMask;

		public static implicit operator LayerMask(LayerMaskAsset layerMaskAsset)
		{
			return layerMaskAsset.Mask;
		}

		public static implicit operator int(LayerMaskAsset layerMaskAsset)
		{
			return layerMaskAsset.Mask;
		}
	}
}