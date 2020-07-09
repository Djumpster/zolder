// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Identification
{
	/// <summary>
	/// An empty device identification provider, used as a fallback
	/// to allow the application to work on non-configured platforms.
	/// </summary>
	public class EmptyDeviceIdentityProvider : IDeviceIdentityProvider
	{
		/// <inheritdoc/>
		public string Type { get; }

		/// <inheritdoc/>
		public string Serial { get; }

		public EmptyDeviceIdentityProvider()
		{
			Type = string.Empty;
			Serial = string.Empty;
		}
	}
}
