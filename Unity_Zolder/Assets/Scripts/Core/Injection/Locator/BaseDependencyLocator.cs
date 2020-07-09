// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using Talespin.Core.Foundation.Attributes;
using Talespin.Core.Foundation.Injection.Internal;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Reflection;
using UnityEngine;

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// A dependency injector that can resolve all dependency references by default using defined
	/// <see cref="IDependencyLocator"/><T> implementations or default constructors.
	/// </summary>
	public class BaseDependencyLocator : DependencyInjector, IDependencyInjector
	{
		public List<KeyValuePair<Type, object>> InstantiatedDependencies
		{
			get
			{
				List<KeyValuePair<Type, object>> instantiatedDependencies = new List<KeyValuePair<Type, object>>();

				foreach (IInstanceResolver resolver in Resolvers)
				{
					if (resolver.WasResolved)
					{
						instantiatedDependencies.Add(new KeyValuePair<Type, object>(resolver.Type, resolver.Instance));
					}
				}

				return instantiatedDependencies;
			}
		}

		public BaseDependencyLocator()
		{
			BindFactories();
		}

		/// <inheritdoc />
		public override bool Contains<T>(string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			return true;
		}

		/// <inheritdoc />
		public override bool Contains(Type type, string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			return true;
		}

		/// <inheritdoc />
		public override T Get<T>(string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			return (T)Get(typeof(T), identifier, includeDerivedTypes, searchAggregatedInjectors);
		}

		/// <inheritdoc />
		public override object Get(Type type, string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			if (!base.Contains(type, identifier, includeDerivedTypes, searchAggregatedInjectors))
			{
				BindType(type, identifier, includeDerivedTypes, searchAggregatedInjectors);
			}

			return base.Get(type, identifier, includeDerivedTypes, searchAggregatedInjectors);
		}

		/// <summary>
		/// Find all dependency locators in the project and bind them
		/// to their appropriate types.
		/// </summary>
		protected void BindFactories()
		{
			List<Type> factoryTypes = new List<Type>(Reflect.AllTypesFrom(typeof(IDependencyLocator), null, InclusionFlags.Default | InclusionFlags.InterfaceTypes));

			for (int i = 0; i < factoryTypes.Count; i++)
			{
				Type factoryType = factoryTypes[i];
				if (factoryType.IsInterface)
				{
					continue;
				}

				Type boundDependencyType = null;
				foreach (Type @interface in factoryType.GetInterfaces())
				{
					if (@interface.IsGenericType && typeof(IDependencyLocator<>) == @interface.GetGenericTypeDefinition())
					{
						boundDependencyType = @interface.GetGenericArguments()[0];
						break;
					}
				}

				if (base.Contains(boundDependencyType, null, false))
				{
					LogUtil.Warning(LogTags.SYSTEM, this, "Found multiple factories for type " + boundDependencyType +
						", assigning the first found factory by default and skipping factory: " + factoryType.Name);

					continue;
				}

				Bind(boundDependencyType).ToFunc((IDependencyInjector injector) =>
				{
					object instance = ConstructFromFactory((IDependencyLocator)injector.Instantiate(factoryType), injector);

					if (instance == null)
					{
						throw new InvalidOperationException("Failed to construct a dependency with factory: " + factoryType.FullName);
					}

					// This is necesary so that if you instantiate a derived type in a factory for a base type
					// the DependencyLocator returns the same instance when you request the derived type directly.
					Type specificType = instance.GetType();
					if (!injector.Bind(specificType).BoundToInstance && injector.Bind(specificType).BoundToConstructor)
					{
						injector.Bind(specificType).ToInstance(instance);
					}

					return instance;
				});
			}
		}

		/// <summary>
		///  Bind a specific type to the injector.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		protected void BindType(Type type, string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			// Bind abstract / interface dependencies to their first found instantiable implementing type if no
			// specific factory was bound for that abstract / interface service.
			if (type.IsInterface || type.IsAbstract)
			{
				IEnumerable<Type> implementations = Reflect.AllTypesFrom(type);

				foreach (Type implementation in implementations)
				{
					LogUtil.Info(LogTags.SYSTEM, this, $"{type.FullName} is ambiguous, using {implementation.FullName} from now on.");

					if (typeof(UnityEngine.Object).IsAssignableFrom(implementation) &&
						!typeof(ScriptableObject).IsAssignableFrom(implementation))
					{
						continue;
					}
					
					Bind(type).ToFunc((IDependencyInjector injector) => injector.Get(implementation));

					if (implementation.IsInterface || implementation.IsAbstract)
					{
						BindType(implementation, identifier, includeDerivedTypes, searchAggregatedInjectors);
					}

					break;
				}
			}
			else if (typeof(ScriptableObject).IsAssignableFrom(type))
			{
				Bind(type).ToFunc((IDependencyInjector injector) => ConstructFromScriptableObject(injector, type));
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				// Don't allow binding UnityEngine.Object types without a factory
			}
			else
			{
				// Do not allow constructors to be called for value types
				if (type.IsValueType)
				{
					return;
				}

				// Manually filter out string and char* types, these types are not
				// caught using Type.IsValueType and will be constructed using a constructor
				// which is not allowed.
				if (type == typeof(char*) || type == typeof(string))
				{
					return;
				}

				Bind(type).ToConstructor(type);
			}
		}

		/// <summary>
		/// Construct a type using a <see cref="IDependencyLocator"/>.
		/// </summary>
		/// <param name="factory">The factory to use for construction</param>
		/// <param name="injector">A reference to the injector</param>
		/// <returns>The created instance from the factory</returns>
		protected virtual object ConstructFromFactory(IDependencyLocator factory, IDependencyInjector injector)
		{
			MethodInfo constructMethodInfo = factory.GetType().GetMethod("Construct");
			return injector.InvokeMethod(constructMethodInfo, factory);
		}

		/// <summary>
		/// Construct a "library" from a scriptable object. This will attempt to load a
		/// Scriptable Object from the path defined by <see cref="ScriptableObjectServicePathAttribute"/>, or
		/// the <c>Assets/Data/Config/Resources</c> directory.
		/// </summary>
		/// <param name="injector">A reference to the injector</param>
		/// <param name="type">The type of the scriptable object to load</param>
		/// <returns>The loaded instance of the scriptable object</returns>
		protected virtual object ConstructFromScriptableObject(IDependencyInjector injector, Type type)
		{
			ScriptableObjectServicePathAttribute pathAttribute = null;
			foreach (object attribute in type.GetCustomAttributes(false))
			{
				pathAttribute = attribute as ScriptableObjectServicePathAttribute;
				if (pathAttribute != null)
				{
					break;
				}
			}

			string path = (pathAttribute != null ? pathAttribute.SubFolderPath + "/" : "") + type.Name;
			object resource = Resources.Load(path, type);

			if (resource != null)
			{
				injector.Inject(resource);
				return resource;
			}
			else
			{
				LogUtil.Error(LogTags.SYSTEM, this, "No factory found for type: " + type.Name +
					$" at path {path} that is a ScriptableObject. Attempted to load it automatically from Resources " +
					$"but it was not found.");

				return null;
			}
		}
	}
}
