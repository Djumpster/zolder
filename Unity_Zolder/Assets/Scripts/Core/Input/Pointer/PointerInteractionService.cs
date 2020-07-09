// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// The PointerInteractionService is the main service for anything related with
	/// <see cref="IPointer"/>s.
	/// 
	/// <para>
	/// To access pointer data you should subscribe to <see cref="PointerUpdatedEvent"/>,
	/// this callback will be invoked once every frame for every active pointer source.
	/// </para>
	/// </summary>
	public class PointerInteractionService : IBootstrappable, IDisposable
	{
		public delegate void PointerUpdate(IPointer pointer, PointerHit hit);

		/// <summary>
		/// This callback will be invoked once every frame for every active pointer source.
		/// </summary>
		public event PointerUpdate PointerUpdatedEvent = delegate { };

		/// <summary>
		/// The global debug mode setting, changing this value will affect all pointers.
		/// The specific debug mode behaviour is implemented on a case-by-case basis.
		/// </summary>
		public bool IsDebugModeEnabled { set; get; }

		// Right now there shouldn't be a scenario where you would require direct
		// access to the pointers, if that changes in the future you can change this
		// property to public.
		internal IEnumerable<IPointer> Pointers => pointers;

		// Right now there shouldn't be a scenario where you would require direct
		// access to the pointers, if that changes in the future you can change this
		// property to public.
		internal int NumRegisteredPointers => pointers.Count;

		private readonly HashSet<IPointer> pointers;

		private readonly ICallbackService callbackService;

		public PointerInteractionService(ICallbackService callbackService)
		{
			pointers = new HashSet<IPointer>();

			this.callbackService = callbackService;
			this.callbackService.LateUpdateEvent += LateUpdate;
		}

		public void Dispose()
		{
			callbackService.LateUpdateEvent -= LateUpdate;
		}

		private void LateUpdate()
		{
			foreach (IPointer pointer in pointers)
			{
				PointerUpdatedEvent.Invoke(pointer, pointer.Hit);
			}
		}

		public bool RegisterPointer(IPointer pointer)
		{
			return pointers.Add(pointer);
		}

		public bool UnregisterPointer(IPointer pointer)
		{
			return pointers.Remove(pointer);
		}

		public bool IsPointerRegistered(IPointer pointer)
		{
			return pointers.Contains(pointer);
		}

		public IEnumerable<IPointer> GetPointersWithTag(string tag)
		{
			return pointers.Where(e => e.Tag == tag);
		}

		public IEnumerable<IPointer> GetAllPointersWithAnyTags(params string[] tags)
		{
			return pointers.Where(e => tags.Contains(e.Tag));
		}

		public IEnumerable<T> GetPointersWithTag<T>(string tag) where T : IPointer
		{
			return pointers.Where(e => e.Tag == tag && e is T).Cast<T>();
		}
	}
}
