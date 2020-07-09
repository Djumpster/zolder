// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.RuntimePermissions
{
	/// <summary>
	/// Interface for requesting and getting status on device runtime permissions
	/// </summary>
	public interface IRuntimePermissionService
	{
		bool AreLocationServicesEnabled { get; }
		PermissionResultReceivedHandler PermissionRequestAnsweredEvent { get; set; }

		PermissionStatus GetPermissionStatus(string permission);

		/// <summary>
		/// Requests a permission
		/// </summary>
		/// <param name="permission">The permission to request</param>
		void RequestPermission(string permission);

		/// <summary>
		/// Returns true if the device will not show the request prompt again despite calling <see cref="RequestPermission(DevicePermissions)"/> 
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		bool UserHasSelectedDontAskAgain(string permission);

		void ShowAppSpecificSettings();

		int GetAPILevel();
	}
}