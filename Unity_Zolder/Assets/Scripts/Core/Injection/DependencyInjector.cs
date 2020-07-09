// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// IoC container implementation.
	/// Can resolve dependencies based on bound type / identifier pair configurations.
	/// Request a type / identifier and it will return an instance that is cached or instantiated based on the
	/// bound configuration. These bound configurations will in turn also try and resolve their dependencies via the
	/// IDependencyInjector. You can also use this to inject into methods or instantiate new objects with a given
	/// constructor, resolving the parameters using the IDependencyInjector.
	/// </summary>
	public class DependencyInjector : IDependencyInjector
	{
		private static readonly Dictionary<Type, List<MethodInfo>> injectionMethodCache = new Dictionary<Type, List<MethodInfo>>();

		public IEnumerable<IInstanceResolver> Resolvers => resolvers;

		public IEnumerable<IDependencyInjector> AggregatedInjectors => aggregatedInjectors;

		private readonly List<IDependencyInjector> aggregatedInjectors = new List<IDependencyInjector>();
		private readonly List<IInstanceResolver> resolvers;
		private readonly string injectionMethodName;
		private readonly string injectionMethodNameForMonoBehaviours;

		private bool hasBeenDisposed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyInjector"/> class.
		/// </summary>
		/// <param name="injectionMethodName">The name of the Injection method called on all classes. If null,
		/// Injection will go via constructors.</param>
		/// <param name = "injectionMethodNameForMonoBehaviours">The name of the Injection method called on all
		/// classes. If not provided, Injection will go via the InjectDependencies method.</param>
		public DependencyInjector(string injectionMethodName = "InjectDependencies", string injectionMethodNameForMonoBehaviours = "InjectDependencies")
		{
			this.injectionMethodNameForMonoBehaviours = injectionMethodNameForMonoBehaviours;
			this.injectionMethodName = injectionMethodName;

			resolvers = new List<IInstanceResolver>();

			Bind<IDependencyInjector>().ToInstance(this);
			Bind<DependencyInjector>().ToInstance(this);
		}

		/// <summary>
		/// Clone constructor.
		/// </summary>
		/// <param name="resolvers"></param>
		/// <param name="injectionMethodName"></param>
		/// <param name="injectionMethodNameForMonoBehaviours"></param>
		private DependencyInjector(List<IDependencyInjector> aggregatedInjectors, List<IInstanceResolver> resolvers, string injectionMethodName, string injectionMethodNameForMonoBehaviours)
		{
			this.aggregatedInjectors = aggregatedInjectors;
			this.resolvers = CloneResolvers(resolvers);
			this.injectionMethodName = injectionMethodName;
			this.injectionMethodNameForMonoBehaviours = injectionMethodNameForMonoBehaviours;
		}

		/// <summary>
		/// Add an injector that will be checked if a dependency cannot be resolved with this injector's own bindings.
		/// </summary>
		/// <param name="injector">The injector to use in aggregate.</param>
		public virtual void Aggregate(IDependencyInjector injector)
		{
			if (!aggregatedInjectors.Contains(injector))
			{
				// Insert instead of add, because the default one is the globalLocator which can spawn everything
				// we first want to check if an aggregated non-global one already contains what we're looking for
				aggregatedInjectors.Insert(0, injector);
			}
		}

		/// <summary>
		/// Removes an injector that will be checked if a dependency cannot be resolved with this injector's own bindings,
		/// undoing an Aggregate() call.
		/// </summary>
		/// <param name="injector">The injector to no longer use in aggregate.</param>
		public virtual void Deaggregate(IDependencyInjector injector)
		{
			aggregatedInjectors.Remove(injector);
		}

		/// <summary>
		/// Create a clone of the dependency injector.
		/// </summary>
		public virtual IInjectorModule Clone()
		{
			return new DependencyInjector(aggregatedInjectors, resolvers, injectionMethodName, injectionMethodNameForMonoBehaviours);
		}

		/// <summary>
		/// Gets the binding configuration for a specific Type / identifier combination.
		/// </summary>
		/// <param name="identifier">The identifier for which this bound instance should be returned, used to
		/// differentiate between multiple instances of the same class.</param>
		/// <typeparam name="T">The type for which this bound instance should be returned.</typeparam>
		/// <returns>A configurable instance provider.</returns>
		public virtual IInstanceProviderBinding<T> Bind<T>(string identifier = null)
		{
			Type targetType = typeof(T);
			InstanceProvider<T> newResolver = new InstanceProvider<T>(identifier);

			for (int i = 0; i < resolvers.Count; i++)
			{
				if (targetType == resolvers[i].Type && resolvers[i].Identifier == identifier)
				{
					return (InstanceProvider<T>)resolvers[i];
				}
			}

			resolvers.Add(newResolver);
			return newResolver;
		}

		/// <summary>
		/// Gets the binding configuration for a specific Type / identifier combination.
		/// </summary>
		/// <param name="type">The type for which this bound instance should be returned.</param>
		/// <param name="identifier">The identifier for which this bound instance should be returned, used to
		/// differentiate between multiple instances of the same class.</param>
		/// <returns>A configurable instance provider.</returns>
		public virtual IInstanceProviderBinding Bind(Type type, string identifier = null)
		{
			for (int i = 0; i < resolvers.Count; i++)
			{
				if (type == resolvers[i].Type && resolvers[i].Identifier == identifier)
				{
					return (IInstanceProviderBinding)resolvers[i];
				}
			}

			// instantiate as InstanceProvider<> so using the Bind<T> will not cause errors when retrieving it.
			Type providerType = typeof(InstanceProvider<>);
			providerType = providerType.MakeGenericType(type);
			IInstanceProviderBinding newResolver = (IInstanceProviderBinding)Activator.CreateInstance(providerType, identifier);

			resolvers.Add((IInstanceResolver)newResolver);
			return newResolver;
		}

		/// <summary>
		/// Unbinds a specific instance to a type / id pair in the injector, if present.
		/// </summary>
		/// <param name="identifier">The identifier for which the bound instance should be returned, used to
		/// differentiate between multiple instances of the same class.</param>
		/// <typeparam name="T">The type for which the bound instance should be unbound.</typeparam>
		public virtual bool Unbind<T>(string identifier = null)
		{
			return Unbind(typeof(T), identifier);
		}

		/// <summary>
		/// Unbinds a specific instance to a type / id pair in the injector, if present.
		/// </summary>
		/// <param name="type">The type for which the bound instance should be unbound.</param>
		/// <param name="identifier">The identifier for which the bound instance should be returned, used to
		/// differentiate between multiple instances of the same class.</param>
		/// <returns>Was an entry unbound or were there none present?</returns>
		public virtual bool Unbind(Type type, string identifier = null)
		{
			for (int i = 0; i < resolvers.Count; i++)
			{
				if (type == resolvers[i].Type && resolvers[i].Identifier == identifier)
				{
					resolvers[i].Dispose();
					resolvers.RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Instantiates an object of the given type with the defined constructor while resolving parameters using
		/// the injector.
		/// </summary>
		/// <typeparam name="T">Type to instantiate.</typeparam>
		/// <param name="constructorIndex">Index of the constructor to use.</param>
		/// <returns>Instance of type T.</returns>
		public virtual T Instantiate<T>(int constructorIndex = 0)
		{
			return (T)Instantiate(typeof(T), constructorIndex);
		}

		/// <summary>
		/// Instantiates an object of the given type with the defined constructor while resolving parameters using
		/// the injector.
		/// </summary>
		/// <param name="type">Type to instantiate.</param>
		/// <param name="constructorIndex">Index of the constructor to use.</param>
		/// <returns>Instance of type T.</returns>
		public virtual object Instantiate(Type type, int constructorIndex = 0)
		{
			ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (constructors.Length < constructorIndex)
			{
				LogUtil.Error(LogTags.SYSTEM, this, "No constructor with index " + constructorIndex + " on type: " + type);
				return null;
			}

			ConstructorInfo constructor = constructors[constructorIndex];
			object[] parameters = ResolveParameters(constructor, type);

			return constructor.Invoke(parameters);
		}

		/// <summary>
		/// Gets a specific bound entry from the injector.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		/// <typeparam name="T">The type of the bound entry.</typeparam>
		public virtual T Get<T>(string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			object returnValue = Get(typeof(T), identifier, includeDerivedTypes, searchAggregatedInjectors);

			if (returnValue == null)
			{
				return default;
			}

			return (T)returnValue;
		}

		/// <summary>
		/// Gets a specific bound entry from the injector.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="type">The type of the bound entry.</typeparam>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		public virtual object Get(Type type, string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			// first look for an entry bound with the specific type
			for (int i = 0; i < resolvers.Count; i++)
			{
				if (type == resolvers[i].Type && resolvers[i].Identifier == identifier)
				{
					return resolvers[i].Resolve(this);
				}
			}

			if (includeDerivedTypes)
			{
				// then broaden the search to include derived types
				for (int i = 0; i < resolvers.Count; i++)
				{
					if (type.IsAssignableFrom(resolvers[i].Type) && resolvers[i].Identifier == identifier)
					{
						return resolvers[i].Resolve(this);
					}
				}
			}

			if (searchAggregatedInjectors)
			{
				List<IDependencyInjector> checkedInjectors = new List<IDependencyInjector> { this };
				object result = TryGetFromAggregatedInjectorsRecursively(this, ref checkedInjectors, out bool found,
					type, identifier, includeDerivedTypes);
				if (found)
				{
					return result;
				}
			}

			return null;
		}

		/// <summary>
		/// Checks if a specific type / identifier was bound.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		/// <typeparam name="T">The type of the bound entry.</typeparam>
		public virtual bool Contains<T>(string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			return Contains(typeof(T), identifier, includeDerivedTypes, searchAggregatedInjectors);
		}

		/// <summary>
		/// Checks if a specific type / identifier was bound.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="type">The type of the bound entry.</typeparam>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		public virtual bool Contains(Type type, string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true)
		{
			// first look for an entry bound with the specific type
			for (int i = 0; i < resolvers.Count; i++)
			{
				if (type == resolvers[i].Type && resolvers[i].Identifier == identifier)
				{
					return true;
				}
			}

			if (includeDerivedTypes)
			{
				// then broaden the search to include derived types
				for (int i = 0; i < resolvers.Count; i++)
				{
					if (type.IsAssignableFrom(resolvers[i].Type) && resolvers[i].Identifier == identifier)
					{
						return true;
					}
				}
			}

			if (searchAggregatedInjectors)
			{
				List<IDependencyInjector> checkedInjectors = new List<IDependencyInjector>
				{
					this
				};
				if (TryFindInAggregatedInjectorsRecursively(this, ref checkedInjectors, type, identifier, includeDerivedTypes))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns all bound entries of the given type, optionally also derived types.
		/// Note that when resolving an entry causes a new entry to be bound that the new entry does not get added as well.
		/// </summary>
		/// <returns>A list of all bound instances of the given type, optionally also derived types.</returns>
		/// <param name="includeDerivedTypes">Should all entries of a derived type also be included?</param>
		/// <typeparam name="T">The type to return.</param>
		public virtual List<T> GetAll<T>(bool includeDerivedTypes = true)
		{
			List<T> all = new List<T>();
			Type type = typeof(T);
			List<IInstanceResolver> originalResolvers = new List<IInstanceResolver>(resolvers);
			foreach (IInstanceResolver resolver in originalResolvers)
			{
				if (resolver.Type == type || (includeDerivedTypes && type.IsAssignableFrom(resolver.Type)))
				{
					all.Add((T)resolver.Resolve(this));
				}
			}

			return all;
		}

		/// <summary>
		/// Returns all bound entries of the given type, optionally also derived types.
		/// Note that when resolving an entry causes a new entry to be bound that the new entry does not get added as well.
		/// </summary>
		/// <returns>A list of all bound instances of the given type, optionally also derived types.</returns>
		/// <param name="type">The type to return.</param>
		/// <param name="includeDerivedTypes">Should all entries of a derived type also be included?</param>
		public virtual List<object> GetAll(Type type, bool includeDerivedTypes = true)
		{
			List<object> all = new List<object>();
			List<IInstanceResolver> originalResolvers = new List<IInstanceResolver>(resolvers);
			foreach (IInstanceResolver resolver in originalResolvers)
			{
				if (resolver.Type == type || (includeDerivedTypes && type.IsAssignableFrom(resolver.Type)))
				{
					all.Add(resolver.Resolve(this));
				}
			}

			return all;
		}

		/// <summary>
		/// Does a Get() on each of the bound entries to make sure they've been resolved at least once.
		/// Note that when resolving an entry causes a new entry to be bound that the new entry does not get resolved as well.
		/// </summary>
		public virtual void ResolveAll()
		{
			List<IInstanceResolver> originalResolvers = new List<IInstanceResolver>(resolvers);
			foreach (IInstanceResolver resolver in originalResolvers)
			{
				resolver.Resolve(this);
			}
		}

		/// <summary>
		/// Inject an object with any injectables it needs.
		/// </summary>
		/// <param name="objectToInject">The Object to inject on. The parameters of the injection function
		/// (either constructor or a specifically defined method) determine what injectables the object
		/// will receive.</param>
		public virtual void Inject(object objectToInject)
		{
			GameObject go = objectToInject as GameObject;
			if (go != null)
			{
				InjectGameObject(go);
			}
			else
			{
				if (!string.IsNullOrEmpty(injectionMethodName))
				{
					InjectMethod(injectionMethodName, objectToInject);
				}
			}
		}

		/// <summary>
		/// Invoke the method, resolving any parameters using the bound configuration.
		/// </summary>
		/// <param name="methodInfo">The method's method info.</param>
		/// <param name="target">The object the method should be invoked on.</param>
		/// <returns>Any return value the method might have, otherwise null.</returns>
		public virtual object InvokeMethod(MethodInfo methodInfo, object target)
		{
			return InjectMethodBase(methodInfo, target);
		}

		public virtual void Remove(object o)
		{
			// this module just passes dependencies. Since dependencies cannot be unpassed, we 
			// do not need to do anything when an object is removed.
		}

		public virtual string Log()
		{
			StringBuilder builder = new StringBuilder("Dependency Injector for: ");
			bool first = true;
			for (int i = 0; i < resolvers.Count; i++)
			{
				if (!first)
				{
					builder.Append(", ");
				}
				builder.Append(resolvers[i].Type.Name.ToString());
			}
			return ToString();
		}

		public virtual void Dispose()
		{
			if (hasBeenDisposed)
			{
				return;
			}
			hasBeenDisposed = true;

			foreach (IInstanceResolver resolver in resolvers)
			{
				resolver.Dispose();
			}
			resolvers.Clear();
			resolvers.TrimExcess();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("Dependency Injector for: ");
			foreach (IInstanceResolver inj in resolvers)
			{
				builder.Append(inj.Identifier + ": " + inj.Type + "\n");
			}
			foreach (IDependencyInjector aggregatedInjector in aggregatedInjectors)
			{
				builder.Append("Aggregated injector: " + aggregatedInjector + "\n");
			}
			builder.Append("Methods: " + injectionMethodName + ", " + injectionMethodNameForMonoBehaviours);
			return builder.ToString();
		}

		// use this method to prevent infinte loops when there is a circular aggregation.
		private object TryGetFromAggregatedInjectorsRecursively(IDependencyInjector injectorToCheck,
			ref List<IDependencyInjector> checkedInjectors, out bool found, Type type, string identifier = null,
			bool includeDerivedTypes = true)
		{
			foreach (IDependencyInjector aggregatedInjector in injectorToCheck.AggregatedInjectors)
			{
				if (checkedInjectors.Contains(aggregatedInjector))
				{
					continue;
				}
				checkedInjectors.Add(aggregatedInjector);

				if (aggregatedInjector.Contains(type, identifier, includeDerivedTypes, false))
				{
					found = true;
					return aggregatedInjector.Get(type, identifier, includeDerivedTypes, false);
				}

				object returnValue = TryGetFromAggregatedInjectorsRecursively(aggregatedInjector, ref checkedInjectors,
					out bool foundInNestedAggregatedInjector, type, identifier, includeDerivedTypes);

				if (foundInNestedAggregatedInjector)
				{
					found = true;
					return returnValue;
				}
			}

			found = false;
			return null;
		}

		// use this method to prevent infinte loops when there is a circular aggregation.
		private bool TryFindInAggregatedInjectorsRecursively(IDependencyInjector injectorToCheck,
			ref List<IDependencyInjector> checkedInjectors, Type type, string identifier = null,
			bool includeDerivedTypes = true)
		{
			foreach (IDependencyInjector aggregatedInjector in injectorToCheck.AggregatedInjectors)
			{
				if (checkedInjectors.Contains(aggregatedInjector))
				{
					continue;
				}
				checkedInjectors.Add(aggregatedInjector);

				if (aggregatedInjector.Contains(type, identifier, includeDerivedTypes, false))
				{
					return true;
				}

				if (TryFindInAggregatedInjectorsRecursively(aggregatedInjector, ref checkedInjectors, type, identifier,
					includeDerivedTypes))
				{
					return true;
				}
			}

			return false;
		}

		private List<IInstanceResolver> CloneResolvers(List<IInstanceResolver> resolvers)
		{
			List<IInstanceResolver> clonedResolvers = new List<IInstanceResolver>();
			foreach (IInstanceResolver resolver in resolvers)
			{
				clonedResolvers.Add(resolver.Clone());
			}

			return clonedResolvers;
		}

		protected virtual void InjectGameObject(GameObject go)
		{
			MonoBehaviour[] monoBehaviours = go.GetComponentsInChildren<MonoBehaviour>();
			for (int i = 0; i < monoBehaviours.Length; i++)
			{
				InjectMonobehaviour(monoBehaviours[i], go);
			}
		}

		protected void InjectMonobehaviour(MonoBehaviour monoBehaviour, GameObject rootInjectedObject)
		{			
			if (monoBehaviour == null)
			{
				LogUtil.Warning(LogTags.SYSTEM, this, $"While injecting on go '{rootInjectedObject.name}' a monoBehaviour was" +
					$" deleted during injection.");
				return;
			}
			InjectMethod(injectionMethodNameForMonoBehaviours, monoBehaviour);			
		}

		protected virtual MethodInfo[] GetInjectionMethodsOfType(Type type, string methodName, BindingFlags bindingFlags)
		{			
			return Array.FindAll(type.GetMethods(bindingFlags), info => info != null && info.Name == methodName);
		}

		private void InjectMethod(string methodName, object obj)
		{
			Type type = obj.GetType();
			if (string.IsNullOrEmpty(type.Namespace) || !type.Namespace.Contains("Talespin"))
			{
				return;
			}
			if (injectionMethodCache.ContainsKey(type))
			{
				foreach (MethodInfo injectionMethodInfo in injectionMethodCache[type])
				{
					InjectMethodBase(injectionMethodInfo, obj);
				}
				return;
			}
			List<MethodInfo> injectionMethodInfos = new List<MethodInfo>();
			injectionMethodCache.Add(type, injectionMethodInfos);

			MethodInfo[] methods = GetInjectionMethodsOfType(type, methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo == null)
				{
					LogUtil.Error(LogTags.SYSTEM, this, $"While injecting type '{type}' one of the retrieved MethodInfos '{methodName}' was null. Please check the " +
						$"implementation of {nameof(GetInjectionMethodsOfType)} in '{GetType()}' as this should never happen.");
					continue;
				}
				injectionMethodInfos.Add(methodInfo);
				InjectMethodBase(methodInfo, obj);				
			}
		}

		private string FindIdentifier(ParameterInfo info, Type objType)
		{
			object[] attr = info.GetCustomAttributes(typeof(InjectionIdentifierAttribute), true);
			if (attr.Length == 1)
			{
				return ((InjectionIdentifierAttribute)attr[0]).Identifier;
			}
			else if (attr.Length > 1)
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Multiple InjectionIdentifierAttribute on object of type: " + objType);
			}
			return null;
		}

		private object InjectMethodBase(MethodBase info, object obj)
		{
			object[] parameters = ResolveParameters(info, obj.GetType());
			return info.Invoke(obj, parameters);
		}

		private object[] ResolveParameters(MethodBase info, Type objType)
		{
			ParameterInfo[] parameters = info.GetParameters();
			object[] objects = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				Type parameterType = parameters[i].ParameterType;
				string attributeIdentifier = FindIdentifier(parameters[i], objType);

				if (Contains(parameterInfo.ParameterType, attributeIdentifier))
				{
					objects[i] = Get(parameterInfo.ParameterType, attributeIdentifier);
					continue;
				}

				if (parameters[i].HasDefaultValue)
				{
					objects[i] = parameters[i].DefaultValue;
					continue;
				}

				LogUtil.Warning(LogTags.SYSTEM, this, "Could not find bound entry for type " + parameterType.Name +
					(string.IsNullOrEmpty(attributeIdentifier) ? " with no attribute id " : " with attribute id " +
					attributeIdentifier));
			}

			return objects;
		}
	}
}
