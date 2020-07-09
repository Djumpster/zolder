// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// Helper class to quickly instantiate and dependency inject prefabs.
	/// </summary>
	public static class InjectionUtils
	{
		public static GameObject InstantiateAndInject(GameObject original, IDependencyInjector injector)
		{
			return InstantiateAndInject(original, injector, (p) => UnityEngine.Object.Instantiate(p));
		}

		public static GameObject InstantiateAndInject(GameObject original, IDependencyInjector injector, Func<GameObject, GameObject> instantiateFunc)
		{
			GameObject result = null;
			bool wasActive = original.activeSelf;

			try
			{
				original.SetActive(false);

				result = instantiateFunc.Invoke(original);
				injector.Inject(result);

				result.SetActive(wasActive);
			}
			finally
			{
				original.SetActive(wasActive);
			}

			return result;
		}
	}
}
