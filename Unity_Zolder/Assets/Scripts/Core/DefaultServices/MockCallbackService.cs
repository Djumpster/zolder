// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Services
{
	public class MockCallbackService : ICallbackService
	{
#pragma warning disable 0067
		public event Action GUIEvent;
		public event Action<bool> ApplicationFocusEvent;
		public event Action<bool> ApplicationPauseEvent;
		public event Action ApplicationQuitEvent;
		public event Action RenderObjectEvent;
		public event Action DrawGizmosEvent;
		public event Action UpdateEvent;
		public event Action FixedUpdateEvent;
		public event Action LateUpdateEvent;
#pragma warning restore 0067
	}
}