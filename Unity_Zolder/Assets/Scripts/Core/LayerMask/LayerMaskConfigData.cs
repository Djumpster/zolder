// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.LayerMasks
{
	/// <summary>
	/// A layer mask configuration serialized as a ScriptableObject so it can be reused.
	/// </summary>
	public class LayerMaskConfigData : ScriptableObject
	{
		public LayerMask LayerMask
		{
			get { return layerMask.LayerMask; }
#if UNITY_EDITOR
			set { layerMask.LayerMask = value; }
#endif
		}
		[SerializeField] private LayerMaskSetting layerMask;

		public static implicit operator LayerMask(LayerMaskConfigData config)
		{
			return config.LayerMask;
		}

		public static implicit operator int(LayerMaskConfigData config)
		{
			return config.LayerMask;
		}
	}
}
