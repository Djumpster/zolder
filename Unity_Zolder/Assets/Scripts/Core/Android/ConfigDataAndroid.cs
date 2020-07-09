// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.CI
{
	[Obsolete("Use command line parameters instead")]
	public class ConfigDataAndroid : ScriptableObject
	{
		/// <summary>
		/// Used for identifying with the store and such on Android.
		/// </summary>
		/// <value>The android play store public key.</value>
		public string AndroidPlayStorePublicKey { get { return androidPlayStorePublicKey; } }
		[SerializeField] private string androidPlayStorePublicKey;

		public string KeyaliasName { get { return keyaliasName; } }
		[SerializeField] private string keyaliasName;

		public string KeyaliasPass { get { return keyaliasPass; } }
		[SerializeField] private string keyaliasPass;

		public string KeystoreName { get { return keystoreName; } }
		[SerializeField] private string keystoreName;

		public string KeystorePass { get { return keystorePass; } }
		[SerializeField] private string keystorePass;
	}
}