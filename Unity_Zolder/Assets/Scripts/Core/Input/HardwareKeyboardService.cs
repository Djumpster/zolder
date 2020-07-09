// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using Talespin.Core.Foundation.Events;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Services;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// Bootstrappable service to handle sending of keyboard inputs to global events.
	/// Do note that this is a temp solution and should be changed to the new input system.
	/// </summary>
	public class HardwareKeyboardService : IBootstrappable, System.IDisposable
	{
		private GlobalEvents globalEvents;
		private UnityCallbackService unityCallbackService;

		private float timeBetweenHold = 0.065f;
		private float backspaceDelay = 0.5f;
		private float timeOfBackspaceHold;
		private float timeOfNextDeletion;
		private bool isHoldingBackspace;
		private bool backspaceDelayPassed;

		public HardwareKeyboardService(GlobalEvents globalEvents, UnityCallbackService unityCallbackService)
		{
			this.globalEvents = globalEvents;
			this.unityCallbackService = unityCallbackService;
			this.unityCallbackService.UpdateEvent += OnUpdateEvent;
		}

		public void Dispose()
		{
			unityCallbackService.UpdateEvent -= OnUpdateEvent;
		}

		private void OnUpdateEvent()
		{
			if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				globalEvents.Invoke(new KeyboardInputEvent(this, "\n"));
			}
			
			if(Input.GetKeyDown(KeyCode.Tab))
			{
				globalEvents.Invoke(new KeyboardInputEvent(this, KeyCode.Tab.ToString()));
			}

			if (Input.GetKey(KeyCode.Backspace))
			{
				HandleBackspaceLogic();
			}
			else
			{
				isHoldingBackspace = false;
			}

			if (Input.anyKeyDown)
			{
				foreach (char c in Input.inputString)
				{
					//Handle backspace and return by there own response.
					if (c == '\b' || c == '\n')
					{
						continue;
					}
					
					globalEvents.Invoke(new KeyboardInputEvent(this, c.ToString()));
				}
			}
		}

		private void HandleBackspaceLogic()
		{
			if (!isHoldingBackspace)
			{
				timeOfBackspaceHold = Time.time;
				backspaceDelayPassed = false;
				timeOfNextDeletion = Mathf.Infinity;
				globalEvents.Invoke(new KeyboardInputEvent(this, KeyCode.Backspace.ToString()));
			}
			else if (!backspaceDelayPassed && Time.time > timeOfBackspaceHold + backspaceDelay)
			{
				backspaceDelayPassed = true;
				timeOfNextDeletion = Time.time + timeBetweenHold;
				globalEvents.Invoke(new KeyboardInputEvent(this, KeyCode.Backspace.ToString()));
			}
			else if (Time.time > timeOfNextDeletion)
			{
				timeOfNextDeletion = Time.time + timeBetweenHold;
				globalEvents.Invoke(new KeyboardInputEvent(this, KeyCode.Backspace.ToString()));
			}

			isHoldingBackspace = true;
		}
	}
}
