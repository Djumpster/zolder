// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System;
using System.Text;
using Talespin.Core.Foundation.Services;

#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Talespin.Core.Foundation.Injection
{
	public class CallbackModule : IInjectorModule
	{
		private List<Action> updatable = new List<Action>();
		private List<Action> lateUpdatable = new List<Action>();
		private List<Action> fixedUpdatable = new List<Action>();
		private List<Action> gizmos = new List<Action>();
		private List<Action> guis = new List<Action>();
		private ICallbackService unityCallbackService;
		private bool disposed = false;
		private bool hasUpdate;
		private bool hasLateUpdate;
		private bool hasFixedUpdate;
		private bool hasGizmos;
		private bool hasGuis;

		public CallbackModule(ICallbackService callback)
		{
			unityCallbackService = callback;
		}

		public IInjectorModule Clone()
		{
			return new CallbackModule(unityCallbackService);
		}

		public void Dispose()
		{
			disposed = true;
			if (hasUpdate)
			{
				unityCallbackService.UpdateEvent -= Update;
			}
			if (hasLateUpdate)
			{
				unityCallbackService.LateUpdateEvent -= LateUpdate;
			}
			if (hasFixedUpdate)
			{
				unityCallbackService.FixedUpdateEvent -= FixedUpdate;
			}
			if (hasGizmos)
			{
				unityCallbackService.DrawGizmosEvent -= HandleGizmosEvent;
			}
			if (hasGuis)
			{
				unityCallbackService.GUIEvent -= HandleGUIEvent;
			}

			updatable.Clear();
			lateUpdatable.Clear();
			fixedUpdatable.Clear();
			gizmos.Clear();
			guis.Clear();

			updatable = null;
			lateUpdatable = null;
			gizmos = null;
			fixedUpdatable = null;
			guis = null;
		}

		public string Log()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("Updatable:");
			for (int i = 0; i < updatable.Count; i++)
			{
				builder.Append(updatable[i].GetType().ToString());
				builder.Append(" ");
			}
			builder.Append("\n");

			builder.AppendLine("LateUpdatable:");
			for (int i = 0; i < lateUpdatable.Count; i++)
			{
				builder.Append(lateUpdatable[i].GetType().ToString());
				builder.Append(" ");
			}
			builder.Append("\n");

			builder.AppendLine("FixedUpdatable:");
			for (int i = 0; i < fixedUpdatable.Count; i++)
			{
				builder.Append(fixedUpdatable[i].GetType().ToString());
				builder.Append(" ");
			}
			builder.Append("\n");

			builder.AppendLine("Gizmos:");
			for (int i = 0; i < gizmos.Count; i++)
			{
				builder.Append(gizmos[i].GetType().ToString());
				builder.Append(" ");
			}
			builder.Append("\n");

			builder.AppendLine("GUI:");
			for (int i = 0; i < guis.Count; i++)
			{
				builder.Append(guis[i].GetType().ToString());
				builder.Append(" ");
			}
			builder.Append("\n");

			return builder.ToString();
		}

		public void Inject(object o)
		{
			IUpdatable upd = o as IUpdatable;
			if (upd != null && !updatable.Contains(upd.Update))
			{
				if (!hasUpdate)
				{
					hasUpdate = true;
					unityCallbackService.UpdateEvent += Update;
				}
				updatable.Add(upd.Update);
			}
			ILateUpdatable late = o as ILateUpdatable;
			if (late != null && !lateUpdatable.Contains(late.LateUpdate))
			{
				if (!hasLateUpdate)
				{
					hasLateUpdate = true;
					unityCallbackService.LateUpdateEvent += LateUpdate;
				}
				lateUpdatable.Add(late.LateUpdate);
			}
			IFixedUpdatable fix = o as IFixedUpdatable;
			if (fix != null && !fixedUpdatable.Contains(fix.FixedUpdate))
			{
				if (!hasFixedUpdate)
				{
					hasFixedUpdate = true;
					unityCallbackService.FixedUpdateEvent += FixedUpdate;
				}
				fixedUpdatable.Add(fix.FixedUpdate);
			}
			IGizmo giz = o as IGizmo;
			if (giz != null && !gizmos.Contains(giz.OnDrawGizmo))
			{
				if (!hasGizmos)
				{
					hasGizmos = true;
					unityCallbackService.DrawGizmosEvent += HandleGizmosEvent;
				}
				gizmos.Add(giz.OnDrawGizmo);
			}
			IOnGUI gui = o as IOnGUI;
			if (gui != null && !guis.Contains(gui.OnGUI))
			{
				if (!hasGuis)
				{
					hasGuis = true;
					unityCallbackService.GUIEvent += HandleGUIEvent;
				}
				guis.Add(gui.OnGUI);
			}
		}

		public void Remove(object o)
		{
			IUpdatable upd = o as IUpdatable;
			if (upd != null)
			{
				updatable.Remove(upd.Update);
			}
			ILateUpdatable late = o as ILateUpdatable;
			if (late != null)
			{
				lateUpdatable.Remove(late.LateUpdate);
			}
			IFixedUpdatable fix = o as IFixedUpdatable;
			if (fix != null)
			{
				fixedUpdatable.Remove(fix.FixedUpdate);
			}
			IGizmo giz = o as IGizmo;
			if (giz != null)
			{
				gizmos.Remove(giz.OnDrawGizmo);
			}
			IOnGUI gui = o as IOnGUI;
			if (gui != null)
			{
				guis.Remove(gui.OnGUI);
			}
		}

		private void Clean(List<Action> list)
		{
			if (list == null || disposed)
			{
				return;
			}

			for (int i = list.Count - 1; i >= 0; i--)
			{
				Action obj = list[i];
				if (obj == null || (obj != null && obj.Target == null))
				{
					list.RemoveAt(i);
				}
			}
		}

		private void FixedUpdate()
		{
			if (fixedUpdatable == null || disposed)
			{
				return;
			}

			Clean(fixedUpdatable);

			// this class can be disposed while executing this for loop!
			for (int i = 0; !disposed && i < fixedUpdatable.Count; i++)
			{
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.BeginSample(fixedUpdatable[i].Target.GetType().ToString());
#endif
				fixedUpdatable[i]();
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.EndSample();
#endif
			}
		}

		private void LateUpdate()
		{
			if (lateUpdatable == null || disposed)
			{
				return;
			}

			Clean(lateUpdatable);
			// this class can be disposed while executing this for loop!
			for (int i = 0; !disposed && i < lateUpdatable.Count; i++)
			{
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.BeginSample(lateUpdatable[i].Target.GetType().ToString());
#endif
				lateUpdatable[i]();
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.EndSample();
#endif
			}
		}

		private void Update()
		{
			if (updatable == null || disposed)
			{
				return;
			}

			Clean(updatable);

			// this class can be disposed while executing this for loop!
			for (int i = 0; !disposed && i < updatable.Count; i++)
			{
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.BeginSample(updatable[i].Target.GetType().ToString());
#endif
				updatable[i]();
#if UNITY_EDITOR || DEEP_PROFILE_PLAYER
				Profiler.EndSample();
#endif
			}
		}

		private void HandleGizmosEvent()
		{
			if (gizmos == null || disposed)
			{
				return;
			}

			Clean(gizmos);
			for (int i = 0; i < gizmos.Count; i++)
			{
				gizmos[i]();
			}
		}

		private void HandleGUIEvent()
		{
			if (guis == null || disposed)
			{
				return;
			}

			Clean(guis);
			for (int i = 0; i < guis.Count; i++)
			{
				guis[i]();
			}
		}
	}
}