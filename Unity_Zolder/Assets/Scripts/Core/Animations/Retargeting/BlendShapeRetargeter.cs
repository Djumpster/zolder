// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Retargeting
{
	/// <summary>
	/// Retargets the defined blendshapes from a skinned mesh renderer to a target parameter in the target animator.
	/// </summary>
	public class BlendShapeRetargeter : MonoBehaviour
	{
		[Serializable]
		private struct BlendshapeAnimatorMapping
		{
			public string Blendshape;
			public string AnimatorParameter;
		}

		[SerializeField] private SkinnedMeshRenderer sourceSkinnedMeshRenderer;
		[SerializeField] private Animator targetAnimator;
		[SerializeField] private BlendshapeAnimatorMapping[] blendshapeMappings;

		private Dictionary<string, int> blendshapeIndices;

		protected void Start()
		{
			blendshapeIndices = new Dictionary<string, int>();

			for (int i = 0; i < sourceSkinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
			{
				string name = sourceSkinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
				blendshapeIndices.Add(name, i);
			}
		}

		protected void Update()
		{
			for (int i = 0; i < blendshapeMappings.Length; i++)
			{
				string blendShape = blendshapeMappings[i].Blendshape;
				string parameter = blendshapeMappings[i].AnimatorParameter;

				if (blendshapeIndices.TryGetValue(blendShape, out int blendshapeIndex))
				{
					float blendshapeValue = sourceSkinnedMeshRenderer.GetBlendShapeWeight(blendshapeIndex);
					targetAnimator.SetFloat(parameter, blendshapeValue);
				}
				else
				{
					Debug.LogWarning("Unable to find blendShape: " + blendShape);
				}
			}
		}
	}
}
