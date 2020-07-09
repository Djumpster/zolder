// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Bones
{
	/// <summary>
	/// Behaviour which will set the current components transform to that of the selected bone
	/// </summary>
	public class FollowBoneType : MonoBehaviour
	{
		public BoneTypeDescriptor BoneDescriptor { get; set; }
		public Transform Bone
		{
			get
			{
				if (boneLookup == null)
				{
					return null;
				}
				return boneLookup.GetBone(BoneDescriptor);
			}
		}

		[SerializeField] private Transform rootOfAnimatedGraphic;
		private Transform cachedTransform;
		private BoneTypeLookup boneLookup;

		public void Initialize(BoneTypeLookup boneLookup)
		{
			this.boneLookup = boneLookup;
		}

		public void Initialize(BoneTypeLookup boneLookup, BoneTypeDescriptor descriptor)
		{
			this.boneLookup = boneLookup;
			BoneDescriptor = descriptor;
		}

		protected void Start()
		{
			cachedTransform = transform;
			if (boneLookup == null && rootOfAnimatedGraphic != null)
			{
				boneLookup = new BoneTypeLookup(rootOfAnimatedGraphic);
			}
		}

		protected void LateUpdate()
		{
			if (Bone == null)
			{
				return;
			}
			cachedTransform.position = Bone.position;
			cachedTransform.rotation = Bone.rotation;
		}
	}
}