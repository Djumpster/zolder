// Copyright 2020 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Identification
{
	/// <summary>
	/// Used to provide identification data for the current device.
	/// </summary>
	public interface IDeviceIdentityProvider
	{
		/// <summary>
		/// A custom hardware type identifier.
		/// <para>
		/// This can be customized for every platform
		/// or even for specific scripting define flags,
		/// but the convention is <c>a-hyphen-separated-string</c>
		/// such as <c>android-oculus-quest</c>.
		/// </para>
		/// </summary>
		string Type { get; }

		/// <summary>
		/// The serial number of the device.
		/// </summary>
		string Serial { get; }
	}
}
