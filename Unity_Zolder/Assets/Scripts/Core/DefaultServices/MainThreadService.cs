// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Services
{
	/// <summary>
	/// Can call given methods on the main thread.
	/// </summary>
	public class MainThreadService
	{
		private Thread mainThread;
		private List<Action> queuedActions = new List<Action>();

		public MainThreadService(UnityCallbackService callbackService)
		{
			callbackService.UpdateEvent += OnUpdateEvent;
		}

		public void PerformOnMainThread(Action action)
		{
			if (mainThread == Thread.CurrentThread)
			{
				action();
				return;
			}

			queuedActions.Add(action);
		}

		private void OnUpdateEvent()
		{
			if (mainThread == null)
			{
				mainThread = Thread.CurrentThread;
			}

			while (queuedActions.Count > 0)
			{
				Action action = queuedActions[0];
				queuedActions.RemoveAt(0);
				if (action == null)
				{
					LogUtil.Warning(LogTags.SYSTEM, this, "Trying to execute method on the main thread but it was null.");
				}
				else
				{
					action.Invoke();
				}
			}
		}
	}
}
