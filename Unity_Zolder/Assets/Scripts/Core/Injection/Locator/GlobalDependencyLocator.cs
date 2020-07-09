// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Injection.Internal;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// Maintains globally accessible instances.
	/// Note that the dependency instances themselves are not singletons, only the <see cref="GlobalDependencyLocator"/> is.
	/// The dependencies and their factories are dynamically located when this object is instantiated.
	/// The <see cref="GlobalDependencyLocator"/> can use the factories or empty constructors to instantiate dependency instances and
	/// will cache them.
	/// </summary>
	public class GlobalDependencyLocator : BaseDependencyLocator
	{
		// Required for some unit tests that make use of services.
		// The 'Registered new GlobalDependencyLocator' message can cause
		// tests using LogAssert.NoUnexpectedReceived() to fail
		public static bool SilentMode = false;

		// Force resetting instance when switching between playing and not playing.
		private static bool wasPlayingWhenCreatedInstance = false;

		/// <summary>
		/// The current instance of the dependency injector.
		/// 
		/// <para>
		/// Will automatically be disposed when switching between play mode and edit mode.
		/// </para>
		/// </summary>
		public static IDependencyInjector Instance
		{
			get
			{
				if (instance == null || wasPlayingWhenCreatedInstance != Application.isPlaying)
				{
					if (!SilentMode)
					{
						LogUtil.Log(LogTags.SYSTEM, "GlobalDependencyLocator", "Registered new GlobalDependencyLocator.");
					}

					instance = new GlobalDependencyLocator();
					wasPlayingWhenCreatedInstance = Application.isPlaying;
				}
				return instance;
			}
		}
		private static GlobalDependencyLocator instance;

		private bool hasBeenDisposed = false;

#if UNITY_EDITOR
		private readonly List<Type> openList = new List<Type>();

		// In the editor make sure that the instance is reset
		// when switching between edit mode and play mode.
		static GlobalDependencyLocator()
		{
			UnityEditor.EditorApplication.playModeStateChanged += s =>
			{
				if (s == UnityEditor.PlayModeStateChange.ExitingEditMode || s == UnityEditor.PlayModeStateChange.ExitingPlayMode)
				{
					if (instance != null)
					{
						DependencyViewerCollector.Reset();
						instance.Dispose();
					}
				}
			};
		}
#endif

		private GlobalDependencyLocator() { }

		public override void Dispose()
		{
			if (hasBeenDisposed)
			{
				return;
			}

			hasBeenDisposed = true;

#if UNITY_EDITOR
			// Dependency Viewer Collector
			foreach (KeyValuePair<Type, object> kvp in InstantiatedDependencies)
			{
				if (kvp.Value != null)
				{
					// Make sure the dependency is removed from
					// the Dependency Viewer's database as well.
					DependencyViewerCollector.RemoveService(kvp.Value);
				}
			}
			// Dependency Viewer Collector
#endif

			base.Dispose();
			instance = null;

			if (!SilentMode)
			{
				LogUtil.Log(LogTags.SYSTEM, "GlobalDependencyLocator", "Disposed GlobalDependencyLocator.");
			}
		}

#if UNITY_EDITOR
		public override T Get<T>(string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			return (T)Get(typeof(T), identifier, includeDerivedTypes, searchAggregatedInjectors);
		}

		public override object Get(Type type, string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			// Dependency Viewer Collector
			// Try to find the "parent" of a dependency, ignoring interfaces.
			int currentIndex = openList.Count - 2;

			if (currentIndex >= 0)
			{
				while (openList[currentIndex].IsInterface)
				{
					currentIndex--;

					if (currentIndex < 0)
					{
						break;
					}
				}
			}
			// Dependency Viewer Collector

			openList.Add(type);
			object createdDependency = base.Get(type, identifier, includeDerivedTypes, searchAggregatedInjectors);

			// Dependency Viewer Collector
			// Set the breadcrumb for this dependency.
			if (createdDependency != null)
			{
				if (currentIndex >= 0)
				{
					DependencyViewerCollector.SetBreadcrumb(createdDependency.GetType(), openList[currentIndex]);
				}
				else
				{
					DependencyViewerCollector.SetInitialBreadcrumb(createdDependency.GetType());
				}
			}
			// Dependency Viewer Collector

			openList.Remove(type);

			return createdDependency;
		}

		public override bool Unbind<T>(string identifier = null)
		{
			// Dependency Viewer Collector
			// Make sure the dependency is removed from
			// the Dependency Viewer's database as well.
			InstanceProvider<T> binding = (InstanceProvider<T>)Bind<T>(identifier);

			if (binding.WasResolved && binding.Instance != null)
			{
				DependencyViewerCollector.RemoveService(binding.Instance);
			}
			// Dependency Viewer Collector

			return base.Unbind<T>(identifier);
		}

		public override bool Unbind(Type type, string identifier = null)
		{
			// Dependency Viewer Collector
			// Make sure the dependency is removed from
			// the Service Viewer's database as well.
			IInstanceResolver binding = (IInstanceResolver)base.Bind(type, identifier);
			if (binding.WasResolved && binding.Instance != null)
			{
				DependencyViewerCollector.RemoveService(binding.Instance);
			}
			// Dependency Viewer Collector

			return base.Unbind(type, identifier);
		}

		protected override object ConstructFromFactory(IDependencyLocator factory, IDependencyInjector injector)
		{
			object service = base.ConstructFromFactory(factory, injector);

			// Dependency Viewer Collector
			// Mark the dependency as "complete"
			if (service != null)
			{
				DependencyViewerCollector.AddService(factory, service);
			}
			// Dependency Viewer Collector

			return service;
		}
#endif
	}
}