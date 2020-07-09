// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Talespin.Core.Foundation.Identification
{
	/// <summary>
	/// Identity provider for Windows.
	/// </summary>
	public class WindowsPlayerDeviceIdentityProvider : IDeviceIdentityProvider
	{
		/// <inheritdoc/>
		public string Type { get; }

		/// <inheritdoc/>
		public string Serial { get; }

		public WindowsPlayerDeviceIdentityProvider()
		{
			Type = "windows-pc";

			IEnumerable<string> macAddresses = NetworkInterface.GetAllNetworkInterfaces().Select(nic => nic.GetPhysicalAddress().ToString());
			Serial = macAddresses.Where(macAddress => !string.IsNullOrEmpty(macAddress)).FirstOrDefault();
		}
	}
}
