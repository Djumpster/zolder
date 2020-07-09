// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// PointerSourceInfos are used by the default pointer implementations
	/// so they can be made as generic as possible.
	/// 
	/// For custom pointer implementations you may not need this.
	/// </summary>
	[DisallowMultipleComponent]
	public abstract class PointerSourceInfo : MonoBehaviour
	{
		/// <summary>
		/// The transform that is being used for the position and direction of the pointer.
		/// </summary>
		public abstract Transform Transform { get; }
	}
}
