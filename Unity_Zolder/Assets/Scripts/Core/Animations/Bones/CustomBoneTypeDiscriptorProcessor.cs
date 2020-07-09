// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Bones
{
	/// <summary>
	/// A helper class that adds any non-predefined <see cref="BoneTypeDescriptor"/> to the list of available bones in a <see cref="BoneTypeLookup"/>
	/// IMPORTANT: Custom bones should be added before constructing a <see cref="BoneTypeLookup"/>.
	/// </summary>
	public static class CustomBoneTypeDiscriptorProcessor
	{
		/// <summary>
		/// Processes the passed bones to add them to the available bones to look up in the <see cref="BoneTypeLookup"/>
		/// Should be done before constructing <see cref="BoneTypeLookup"/>.
		/// </summary>
		/// <param name="bones">The <see cref="Transform"/> that needs to be added</param>
		/// <returns>Returns a list of <see cref="BoneTypeDescriptor"/> that have been processed</returns>
		public static List<BoneTypeDescriptor> Process(params Transform[] bones)
		{
			return Process(bones.Select(b => b.name).ToArray());
		}

		/// <summary>
		/// Similar to <see cref="Process(Transform[])"/>, but takes a string instead if linking in a <see cref="Transform"/> is not possible
		/// Should be done before constructing <see cref="BoneTypeLookup"/>.
		/// </summary>
		/// <param name="bones">The name of the bone that needs to be added</param>
		/// <returns>Returns a list of <see cref="BoneTypeDescriptor"/> that have been processed</returns>
		public static List<BoneTypeDescriptor> Process(params string[] bones)
		{
			List<BoneTypeDescriptor> customBoneTypes = new List<BoneTypeDescriptor>();

			List<BoneTypeDescriptor> availableBones = BoneTypeLookup.GetAvailableBoneTypes();

			for (int i = 0; i < bones.Length; i++)
			{
				BoneTypeDescriptor boneTypeDescriptor = new BoneTypeDescriptor(bones[i], bones[i]);

				if(availableBones.Contains(boneTypeDescriptor))
				{
					continue;
				}

				BoneTypeLookup.AddBones(boneTypeDescriptor);

				customBoneTypes.Add(boneTypeDescriptor);
			}

			return customBoneTypes;
		}
	}
}
