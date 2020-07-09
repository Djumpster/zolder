// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Services
{
	public interface ICallbackService
	{
		event Action GUIEvent;
		event Action<bool> ApplicationFocusEvent;
		event Action<bool> ApplicationPauseEvent;
		event Action ApplicationQuitEvent;
		event Action RenderObjectEvent;
		event Action DrawGizmosEvent;
		event Action UpdateEvent;
		event Action FixedUpdateEvent;
		event Action LateUpdateEvent;
	}
}