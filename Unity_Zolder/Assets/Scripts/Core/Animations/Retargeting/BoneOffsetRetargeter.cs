// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Animations.Bones;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Retargeting
{
	/// <summary>
	/// Retargets the local position, scale and rotation of the animated bones from the source rig to the target rig with an offset from the initial source values.
	/// </summary>
	[DefaultExecutionOrder(-1)]
	public class BoneOffsetRetargeter : MonoBehaviour
	{
		private struct TransformValues
		{
			public Vector3 Position;
			public Vector3 EulerAngles;
			public Vector3 Scale;
		}

		[SerializeField] private Transform sourceRig;
		[SerializeField] private Transform targetRig;
		[SerializeField] private BoneTypeDescriptorListAsset retargetBones;

		private BoneTypeLookup sourceBoneTypeLookup;
		private BoneTypeLookup targetBoneTypeLookup;

		private List<TransformValues> originalTransformValues;

		private List<BoneTypeDescriptor> boneTypeDescriptors;

		protected void Start()
		{
			boneTypeDescriptors = new List<BoneTypeDescriptor>(retargetBones.BoneTypeDescriptors);

			for (int i = 0; i < retargetBones.CustomBoneNames.Length; i++)
			{
				boneTypeDescriptors.Add(new BoneTypeDescriptor(retargetBones.CustomBoneNames[i], retargetBones.CustomBoneNames[i]));
			}

			CustomBoneTypeDiscriptorProcessor.Process(retargetBones.CustomBoneNames);

			sourceBoneTypeLookup = new BoneTypeLookup(sourceRig);
			targetBoneTypeLookup = new BoneTypeLookup(targetRig);

			originalTransformValues = new List<TransformValues>();

			for (int i = 0; i < boneTypeDescriptors.Count; i++)
			{
				Transform sourceBone = sourceBoneTypeLookup.GetBone(boneTypeDescriptors[i]);
				
				TransformValues transformValues = new TransformValues()
				{
					Position = sourceBone.localPosition,
					EulerAngles = sourceBone.localEulerAngles,
					Scale = sourceBone.localScale
				};

				originalTransformValues.Add(transformValues);
			}
		}

		protected void LateUpdate()
		{
			for (int i = 0; i < boneTypeDescriptors.Count; i++)
			{
				Transform sourceBone = sourceBoneTypeLookup.GetBone(boneTypeDescriptors[i]);
				Transform targetBone = targetBoneTypeLookup.GetBone(boneTypeDescriptors[i]);

				TransformValues offset = GetOffset(sourceBone, originalTransformValues[i]);

				targetBone.localPosition += offset.Position;
				targetBone.localEulerAngles += offset.EulerAngles;
				targetBone.localScale += offset.Scale;
			}
		}

		private TransformValues GetOffset(Transform source, TransformValues originalValues)
		{
			return new TransformValues()
			{
				Position = source.localPosition - originalValues.Position,
				EulerAngles = source.localEulerAngles - originalValues.EulerAngles,
				Scale = source.localScale - originalValues.Scale
			};
		}
	}
}
