// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.AssetHandling;
using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	public class CubemapTextureData : ScriptableObject
	{
		public string Front => front;
		public string Back => back;
		public string Left => left;
		public string Right => right;
		public string Top => top;
		public string Bottom => bottom;

		[SerializeField, GuidResource(typeof(Texture))] private string front;
		[SerializeField, GuidResource(typeof(Texture))] private string back;
		[SerializeField, GuidResource(typeof(Texture))] private string left;
		[SerializeField, GuidResource(typeof(Texture))] private string right;
		[SerializeField, GuidResource(typeof(Texture))] private string top;
		[SerializeField, GuidResource(typeof(Texture))] private string bottom;
	}
}
