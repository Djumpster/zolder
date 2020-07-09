// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Bones
{
	/// <summary>
	/// A scriptable object that holds an array of bones.
	/// Used in situations where operations on the same set of bones are required that share the same rig
	/// </summary>
	public class BoneTypeDescriptorListAsset : ScriptableObject
	{
		public BoneTypeDescriptor[] BoneTypeDescriptors => boneTypeDescriptors;
		public string[] CustomBoneNames => customBoneNames;

		[SerializeField] private BoneTypeDescriptor[] boneTypeDescriptors;
		[SerializeField] private string[] customBoneNames;
	}
}
