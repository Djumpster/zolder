// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Storage;
using UnityEngine;

namespace Talespin.Core.Foundation.Settings
{
	public class NativeResolutionServiceFactory : IDependencyLocator<NativeResolutionService>
	{
		public NativeResolutionService Construct(IDependencyInjector serviceLocator)
		{
			return new NativeResolutionService();
		}
	}

	public class NativeResolutionService
	{
		private const string WIDTH_KEY = "nativeScreenWidth";
		private const string HEIGHT_KEY = "nativeScreenHeight";

		public float Width { get; private set; }
		public float Height { get; private set; }

		public NativeResolutionService()
		{
			if (!LazyPlayerPrefs.HasKey(WIDTH_KEY))
			{
				LazyPlayerPrefs.SetFloat(WIDTH_KEY, Screen.width);
			}
			if (!LazyPlayerPrefs.HasKey(HEIGHT_KEY))
			{
				LazyPlayerPrefs.SetFloat(HEIGHT_KEY, Screen.height);
			}

			Width = LazyPlayerPrefs.GetFloat(WIDTH_KEY);
			Height = LazyPlayerPrefs.GetFloat(HEIGHT_KEY);
		}
	}
}