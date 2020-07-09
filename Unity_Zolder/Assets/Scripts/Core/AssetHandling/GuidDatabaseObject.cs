// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.AssetHandling
{
	public class GuidDatabaseObject : ScriptableObject
	{
		[Serializable]
		public class Asset
		{
			[SerializeField] public string GUID;
			[SerializeField] public string[] Value;

			public Asset(string guid, string[] value)
			{
				GUID = guid;
				Value = value;
			}
		}

		[SerializeField] public List<Asset> Assets;
	}
}