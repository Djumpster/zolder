// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Animations.Bones;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Retargeting
{
	/// <summary>
	/// Retargets the local rotation value of a bone to the animator.
	/// </summary>
	public class LocalRotationRetargeter : MonoBehaviour
	{
		[Serializable]
		private struct WeightedBoneAnimatorParameter
		{
			public BoneAnimatorParameter Parameter;
			public float Weight;

			public WeightedBoneAnimatorParameter(BoneAnimatorParameter parameter, float weight)
			{
				Parameter = parameter;
				Weight = weight;
			}
		}

		[SerializeField] private Animator targetAnimator;
		[SerializeField] private Transform sourceRig;
		[SerializeField] private WeightedBoneAnimatorParameter[] retargetBones;

		private BoneTypeLookup boneTypeLookup;
		private Vector3 animatorValues;

		private List<WeightedBoneAnimatorParameter> allBoneAnimatorParameters;

		protected void Start()
		{
			allBoneAnimatorParameters = new List<WeightedBoneAnimatorParameter>();

			for(int i = 0; i < retargetBones.Length; i++)
			{
				if(retargetBones[i].Parameter.CustomBone != null)
				{
					WeightedBoneAnimatorParameter newParameter = new WeightedBoneAnimatorParameter(retargetBones[i].Parameter, retargetBones[i].Weight);
					newParameter.Parameter.ProcessCustomBone();

					allBoneAnimatorParameters.Add(newParameter);

					CustomBoneTypeDiscriptorProcessor.Process(newParameter.Parameter.CustomBone);
				}
				else
				{
					allBoneAnimatorParameters.Add(retargetBones[i]);
				}
			}

			boneTypeLookup = new BoneTypeLookup(sourceRig);

			animatorValues = new Vector3();
		}

		protected void Update()
		{
			for(int i = 0; i < allBoneAnimatorParameters.Count; i++)
			{
				BoneAnimatorParameter parameter = allBoneAnimatorParameters[i].Parameter;
				float weight = allBoneAnimatorParameters[i].Weight;

				Transform transform = boneTypeLookup.GetBone(parameter.Bone);
				Vector3 localRotation = transform.localEulerAngles;

				animatorValues.x = targetAnimator.GetFloat(parameter.Rotation.X);
				animatorValues.y = targetAnimator.GetFloat(parameter.Rotation.Y);
				animatorValues.z = targetAnimator.GetFloat(parameter.Rotation.Z);

				Retarget(parameter.Rotation.X, animatorValues.x, localRotation.x, weight);
				Retarget(parameter.Rotation.Y, animatorValues.y, localRotation.y, weight);
				Retarget(parameter.Rotation.Z, animatorValues.z, localRotation.z, weight);
			}
		}

		private void Retarget(string parameter, float currentAnimatorValue, float transformRotationValue, float weight)
		{
			float result = Mathf.Lerp(currentAnimatorValue, Wrap(transformRotationValue), weight);
			targetAnimator.SetFloat(parameter, result);
		}

		/// <summary>
		/// Wraps the passed rotation value (0 to 360 default) to -180 to 180
		/// </summary>
		/// <param name="value"></param>
		private float Wrap(float value)
		{
			return Mathf.Repeat(value + 180, 360) - 180;
		}
	}
}
