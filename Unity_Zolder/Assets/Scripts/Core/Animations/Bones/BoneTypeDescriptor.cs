// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Animations.Bones
{
	/// <summary>
	/// Descriptor for describing different bonetypes where key will contain the fieldnames of the static members
	/// to maintain typesafety and the name will contain the bonename to lookup in the hierarchy
	/// </summary>
	[Serializable]
	public class BoneTypeDescriptor
	{
		/// <summary>
		/// The name of the static field which defines the bonetype
		/// </summary>
		public string Key;

		/// <summary>
		/// The name of the bone in the hierarchy to search for
		/// </summary>
		public string Name;

		public BoneTypeDescriptor(string name = "", string key = "")
		{
			Name = name;
			Key = key;
		}
	}
}
