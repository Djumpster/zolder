// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Services
{
	public class UnityCallbackServiceFactory : IDependencyLocator<UnityCallbackService>
	{
		public UnityCallbackService Construct(IDependencyInjector serviceLocator)
		{
			if (Application.isPlaying)
			{
				GameObject go = new GameObject("UnityCallbackService");
				Object.DontDestroyOnLoad(go);
				return go.AddComponent<UnityCallbackService>();
			}
			else
			{
				return null;
			}
		}
	}
}