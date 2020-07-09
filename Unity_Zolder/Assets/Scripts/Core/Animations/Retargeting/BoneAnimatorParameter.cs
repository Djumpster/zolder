// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Animations.Bones;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Retargeting
{
	/// <summary>
	/// A bone with its associated animator parameter targets
	/// </summary>
	[Serializable]
	public class BoneAnimatorParameter
	{
		[Serializable]
		public struct ParameterKey
		{
			public string X;
			public string Y;
			public string Z;
		}

		public BoneTypeDescriptor Bone => bone;
		public Transform CustomBone => customBone;

		public ParameterKey Position => positionParameters;
		public ParameterKey Rotation => rotationParameters;
		public ParameterKey Scale => scaleParameters;

		[SerializeField] private BoneTypeDescriptor bone;
		[SerializeField] private Transform customBone;

		[SerializeField] private ParameterKey positionParameters;
		[SerializeField] private ParameterKey rotationParameters;
		[SerializeField] private ParameterKey scaleParameters;

		public void ProcessCustomBone()
		{
			bone = new BoneTypeDescriptor(customBone.name, customBone.name);
		}
	}
}
