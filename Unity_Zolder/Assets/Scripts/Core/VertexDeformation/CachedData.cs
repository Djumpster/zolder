// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.VertexDeformation
{
	/// <summary>
	/// Cached data is used to keep track of
	/// any changes made to the target, the target's mesh,
	/// our own mesh or blendshape weights.
	/// </summary>
	[Serializable]
	public class CachedData
	{
		[SerializeField] public SkinnedMeshRenderer Target;
		[SerializeField] public Mesh TargetMesh;
		[SerializeField] public Mesh TargetMeshBaked;
		[SerializeField] public List<Vector3> TargetMeshVertices;
		[SerializeField] public List<Vector3> TargetMeshBakedVertices;
		[SerializeField] public float TargetScale;
		[SerializeField] public float[] BlendShapeWeights;
		[SerializeField] public SkinnedMeshRenderer Self;
		[SerializeField] public Mesh SelfMesh;
		[SerializeField] public Mesh SelfMeshBaked;
		[SerializeField] public Mesh SelfOriginalMesh;
		[SerializeField] public List<Vector3> SelfMeshVertices;
	}
}
