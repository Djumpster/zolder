// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.RuntimePermissions
{
	/// <summary>
	/// Base class for runTimePermissionService, grants all permissions
	/// </summary>
	public class RuntimePermissionService : IRuntimePermissionService
	{
		/// <summary>
		/// Event that gets called when a callback on <see cref="CallbackRecieverGameObject"/> has been made
		/// </summary>
		public PermissionResultReceivedHandler PermissionRequestAnsweredEvent { get; set; }

		public virtual bool AreLocationServicesEnabled { get { return true; } }

		public RuntimePermissionService() { }

		public virtual PermissionStatus GetPermissionStatus(string permission)
		{
			return PermissionStatus.GRANTED;
		}

		/// <summary>
		/// Requests a permission
		/// <para>On PC: immediatly calls <see cref="PermissionRequestAnsweredEvent"/> with a result of true.</para>
		/// <para>On Mobile: sends a request call to native code and sends a UnityMessage back to the <see cref="RuntimePermissionCallbackReceiver"/> Monobehaviour, 
		/// this will call <see cref="PermissionRequestAnsweredEvent"/> with the correct response. </para>
		/// </summary>
		/// <param name="permission">The permission to request</param>
		public virtual void RequestPermission(string permission)
		{
			PermissionRequestAnsweredEvent?.Invoke(true);
		}

		public virtual void ShowAppSpecificSettings() { }

		public virtual int GetAPILevel()
		{
			return 0;
		}

		public virtual bool UserHasSelectedDontAskAgain(string permission)
		{
			return false;
		}

		public virtual void CreateCallbackReceiver() { }
		public virtual void Dispose() { }
	}
}
