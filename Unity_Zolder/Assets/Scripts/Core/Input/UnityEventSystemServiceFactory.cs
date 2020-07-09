// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Talespin.Core.Foundation.InputHandling
{
	public class UnityEventSystemServiceFactory : IDependencyLocator<UnityEventSystemService>
	{
		public UnityEventSystemService Construct(IDependencyInjector serviceLocator)
		{
			GameObject go = new GameObject("EventSystem");
			go.AddComponent<EventSystem>();
			go.AddComponent<PointerInteractionInputModule>();
			Object.DontDestroyOnLoad(go);
			return new UnityEventSystemService(go);
		}
	}
}