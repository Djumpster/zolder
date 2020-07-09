// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Linq;
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// A PointerSourceInfo designed for generic gaze pointers,
	/// simply attach this script to a <see cref="PointerRay"/>.
	/// </summary>
	public class GazePointerSourceInfo : PointerSourceInfo
	{
		/// <inheritdoc/>
		public override Transform Transform => transform;
	}
}
