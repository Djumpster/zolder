// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.VertexDeformation
{
	/// <summary>
	/// Data structure linking a <see cref="VertexData"/> to a <see cref="DeformationAnchor"/>.
	/// The actual link is kept track of via array indices in a row-major order array.
	/// This structure holds the offset between a vertex and an anchor point.
	/// </summary>
	[Serializable]
	public struct VertexDeformationData
	{
		[SerializeField] public Vector3 AnchorPointOffset;
	}
}
