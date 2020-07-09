// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using UnityEngine;
using UnityEngine.Profiling;

namespace Talespin.Core.Foundation.Services
{
	public class UnityCallbackComponent : MonoBehaviour, ICallbackService, IDisposable
	{
		// Do NOT add GameObject specific callbacks!!!!!!!!!
		// explanation below
		public event Action GUIEvent = delegate { };
		public event Action<bool> ApplicationFocusEvent = delegate { };
		public event Action<bool> ApplicationPauseEvent = delegate { };
		public event Action ApplicationQuitEvent = delegate { };
		public event Action RenderObjectEvent = delegate { };
		public event Action DrawGizmosEvent = delegate { };

		public event Action UpdateEvent
		{
			add
			{
				if (updatables.Contains(value))
				{
					LogUtil.Error(LogTags.SYSTEM, this, "Double subscribe!!!!");
				}
				else
				{
					updatables.Add(value);
				}
			}
			remove
			{
				markedUpdatables.Remove(value);
				updatables.Remove(value);
			}
		}

		public event Action FixedUpdateEvent
		{
			add
			{
				if (fixedUpdatables.Contains(value))
				{
					LogUtil.Error(LogTags.SYSTEM, this, "Double subscribe!");
				}
				else
				{
					fixedUpdatables.Add(value);
				}
			}
			remove
			{
				markedFixedUpdatables.Remove(value);
				fixedUpdatables.Remove(value);
			}
		}

		public event Action LateUpdateEvent
		{
			add
			{
				if (lateUpdatables.Contains(value))
				{
					LogUtil.Error(LogTags.SYSTEM, this, "Double subscribe!!!!");
				}
				else
				{
					lateUpdatables.Add(value);
				}
			}
			remove
			{
				markedLateUpdatables.Remove(value);
				lateUpdatables.Remove(value);
			}
		}

		private List<Action> updatables = new List<Action>();
		private List<Action> fixedUpdatables = new List<Action>();
		private List<Action> lateUpdatables = new List<Action>();

		private List<Action> markedUpdatables = new List<Action>();
		private List<Action> markedFixedUpdatables = new List<Action>();
		private List<Action> markedLateUpdatables = new List<Action>();

		// Do NOT add GameObject specific callbacks, such as:
		// 	OnEnable
		// 	OnDisable
		// 	OnDestroy
		//  OnPreRender
		//  OnPostRender
		//	OnTrigger...
		// etc.
		// Such events fire when the object of this service is created/destroyed and generally will never fire, 
		// as the subscriber won't exist when the event happens

		protected virtual void OnGUI()
		{
			GUIEvent();
		}

		protected virtual void Update()
		{
			for (int i = 0; i < updatables.Count; i++)
			{
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.BeginSample(updatables[i].Target.GetType().ToString());
#endif
				updatables[i]();
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.EndSample();
#endif
			}
			for (int i = 0; i < markedUpdatables.Count; i++)
			{
				updatables.Remove(markedUpdatables[i]);
			}
			markedUpdatables.Clear();
		}

		protected virtual void FixedUpdate()
		{
			for (int i = 0; i < fixedUpdatables.Count; i++)
			{
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.BeginSample(fixedUpdatables[i].Target.GetType().ToString());
#endif
				fixedUpdatables[i]();
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.EndSample();
#endif
			}
			for (int i = 0; i < markedFixedUpdatables.Count; i++)
			{
				fixedUpdatables.Remove(markedFixedUpdatables[i]);
			}
			markedFixedUpdatables.Clear();
		}

		protected virtual void LateUpdate()
		{
			for (int i = 0; i < lateUpdatables.Count; i++)
			{
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.BeginSample(lateUpdatables[i].Target.GetType().ToString());
#endif
				lateUpdatables[i]();
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.EndSample();
#endif
			}
			for (int i = 0; i < markedLateUpdatables.Count; i++)
			{
				lateUpdatables.Remove(markedLateUpdatables[i]);
			}
			markedLateUpdatables.Clear();
		}

		protected virtual void OnApplicationFocus(bool focusStatus)
		{
			ApplicationFocusEvent(focusStatus);
		}

		protected virtual void OnApplicationPause(bool pauseStatus)
		{
			ApplicationPauseEvent(pauseStatus);
		}

		protected virtual void OnApplicationQuit()
		{
			ApplicationQuitEvent();
		}

#if !UNITY_5_4_OR_NEWER
		private void OnLevelWasLoaded(int level)
		{
			LevelWasLoadedEvent(level);
		}
#endif

		protected virtual void OnRenderObject()
		{
			RenderObjectEvent();
		}

		protected virtual void OnDrawGizmos()
		{
			DrawGizmosEvent();
		}

		public void Dispose()
		{
			Destroy(gameObject);
		}
	}
}