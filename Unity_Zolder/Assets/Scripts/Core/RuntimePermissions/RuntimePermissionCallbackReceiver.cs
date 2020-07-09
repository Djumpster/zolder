// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.RuntimePermissions
{
	/// <summary>
	/// Monobehaviour for recieving native-to-managed callback unity messages for mobile devices
	/// </summary>
	public class RuntimePermissionCallbackReceiver : MonoBehaviour
	{
		public const string PERMISSION_GRANTED_MESSAGE = "true";
		public const string PERMISSION_DENIED_MESSAGE = "false";

		public event PermissionResultReceivedHandler PermissionRequestAnsweredEvent;

		public void OnPermissionRequestResult(string result)
		{
			if (result == PERMISSION_GRANTED_MESSAGE)
			{
				PermissionRequestAnsweredEvent?.Invoke(true);
			}
			else
			{
				PermissionRequestAnsweredEvent?.Invoke(false);
			}
		}
	}
}