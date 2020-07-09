// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	public class AlwaysIncludedShadersLibrary : ScriptableObject
	{
		public Shader[] Shaders => shaders;
		[SerializeField] private Shader[] shaders;
	}
}