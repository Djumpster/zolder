// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Animations.Bones;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Retargeting
{
	/// <summary>
	/// Retargets the local position, scale and rotation of the animated bones from the source rig to the target rig based on a weight
	/// </summary>	
	[DefaultExecutionOrder(-1)]
	public class BoneRetargeter : MonoBehaviour
	{
		[SerializeField] private float weight;
		[SerializeField] private Transform sourceRig;
		[SerializeField] private Transform targetRig;
		[SerializeField] private BoneTypeDescriptorListAsset retargetBones;

		private BoneTypeLookup sourceBoneTypeLookup;
		private BoneTypeLookup targetBoneTypeLookup;

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
		}

		protected void LateUpdate()
		{
			for(int i = 0; i < boneTypeDescriptors.Count; i++)
			{
				BoneTypeDescriptor boneTypeDescriptor = boneTypeDescriptors[i];

				Transform source = sourceBoneTypeLookup.GetBone(boneTypeDescriptor);
				Transform target = targetBoneTypeLookup.GetBone(boneTypeDescriptor);

				if(source == null)
				{
					Debug.LogWarning("Could not find " + boneTypeDescriptor.Name + " on " + source.name);
					continue;
				}

				if(target == null)
				{
					Debug.LogWarning("Could not find " + boneTypeDescriptor.Name + " on " + target.name);
					continue;
				}

				target.localPosition = Vector3.Lerp(target.localPosition, source.localPosition, weight);
				target.localScale = Vector3.Lerp(target.localScale, source.localScale, weight);
				target.localRotation = Quaternion.Slerp(target.localRotation, source.localRotation, weight);
			}
		}
	}
}
