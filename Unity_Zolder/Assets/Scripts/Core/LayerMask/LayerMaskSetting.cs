// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.LayerMasks
{
	/// <summary>
	/// A layermask that can be configured. Encapsulated in order to write a custom property drawer.
	/// This is necesary because the default behaviour for LayerMasks is to set all layers to true
	/// when selecting "everything", even undefined labels. When new layers become defined you then
	/// have to go through all objects with configured LayerMasks to disable those new layers.
	/// Worse yet, everything is enabled by default when creating a new LayerMask -
	/// ISerializationCallbackReceiver was implemented to override this default behaviour.
	/// The custom property drawer adds the option "Everything (excluding undefined layers)".
	/// The downside is that selecting "everything" will still make the popup show the "mixed..." label
	/// since the extra options are always false.
	/// </summary>
	[Serializable]
	public class LayerMaskSetting : ISerializationCallbackReceiver
	{
		public LayerMask LayerMask
		{
			get { return layerMask; }
#if UNITY_EDITOR
			set { layerMask = value; }
#endif
		}
		[SerializeField] private LayerMask layerMask;

		[SerializeField, HideInInspector] private bool isFirstInitialization = true;

		public static implicit operator LayerMask(LayerMaskSetting setting)
		{
			return setting.LayerMask;
		}

		public static implicit operator int(LayerMaskSetting setting)
		{
			return setting.LayerMask;
		}

		public void OnBeforeSerialize()
		{
			// have to do this here instead of in constructor otherwise Unity starts complaining about
			// calling LayerMask.LayerToName(i) when you're not supposed to.
			if (isFirstInitialization)
			{
				isFirstInitialization = false;

				for (int i = 0; i < 32; i++)
				{
					string layerName = LayerMask.LayerToName(i);
					bool isDefined = !string.IsNullOrEmpty(layerName);
					if (isDefined)
					{
						layerMask = layerMask | (1 << i);
					}
					else
					{
						layerMask = layerMask & ~(1 << i);
					}
				}
			}
		}

		public void OnAfterDeserialize()
		{

		}
	}
}
