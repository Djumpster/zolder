// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Storage
{
	public class ConfigDataSaveGame : ScriptableObject
	{
		/// <summary>
		/// The hash key to use to hash the save game data when storing it on the disk.
		/// </summary>
		/// <value>The data hash key.</value>
		public string DataHashKey { get { return dataHashKey; } }
		[SerializeField] private string dataHashKey = "replace me with a proper hash";
	}
}
