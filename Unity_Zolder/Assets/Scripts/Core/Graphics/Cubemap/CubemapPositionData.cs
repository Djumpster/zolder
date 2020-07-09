// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	/// <summary>
	/// Editor representation for cubemap mapping.
	/// For the runtime representation, see <see cref="CubemapPositionService"/>.
	/// </summary>
	public class CubemapPositionData : ScriptableObject
	{
		[Serializable]
		public class CubemapEntry
		{
			[SerializeField] public UnityEngine.Object Position;
			[SerializeField] public Cubemap Cubemap;

			[SerializeField] public Vector3 CachedPosition;
			[SerializeField] public int CachedResolution;
		}

		public CubemapEntry[] Entries
		{
			get { return entries; }
		}

		public int Resolution
		{
			get { return resolution; }
		}

		[SerializeField] private CubemapEntry[] entries;

		[SerializeField] private int resolution = 1024;
	}
}
