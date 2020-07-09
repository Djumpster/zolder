// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.RuntimePermissions
{
	/// <summary>
	/// Callback for recieving permission results
	/// </summary>
	public delegate void PermissionResultReceivedHandler(bool result);

	/// <summary>
	/// Interface for creating and disposing runtimPermissionCallbackReciever
	/// </summary>
	public interface IRequiresCallbackReceiver : IDisposable
	{
		RuntimePermissionCallbackReceiver CallbackReceiverGameObject { get; }

		void CreateCallbackReceiver();
	}
}