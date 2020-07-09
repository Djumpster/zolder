// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// A Inversion of Control container implementation.
	/// 
	/// <para>
	/// Can resolve dependencies based on bound type / identifier pair configurations. Each type / identifier pair is unique, so:
	/// <c>Bind{SomeClass}()</c> and <c>Bind{SomeClass}("someIdentifier")</c> will be two separate instances.</para>
	/// 
	/// <para>
	/// Request a type / identifier and it will return an instance that is cached or instantiated based on the bound configuration.
	/// These bound configurations will in turn also try and resolve their dependencies via the IDependencyInjector.
	/// You can also use this to inject into methods or instantiate new objects with a given constructor, resolving the parameters using the IDependencyInjector.
	/// </para>
	/// </summary>
	public interface IDependencyInjector : IInjectorModule
	{
		/// <summary>
		/// A collection of aggregated injectors, when a binding is not found locally within
		/// this injector, it will optionally look for bindings in the aggregated injectors.
		/// </summary>
		IEnumerable<IDependencyInjector> AggregatedInjectors { get; }

		/// <summary>
		/// Add an injector that will be checked if a dependency cannot be resolved with this injector's own bindings.
		/// </summary>
		/// <param name="injector">The injector to use in aggregate.</param>
		void Aggregate(IDependencyInjector injector);

		/// <summary>
		/// Removes an injector that will be checked if a dependency cannot be resolved with this injector's own bindings,
		/// undoing an Aggregate() call.
		/// </summary>
		/// <param name="injector">The injector to no longer use in aggregate.</param>
		void Deaggregate(IDependencyInjector injector);

		/// <summary>
		/// Gets the binding configuration for a specific Type / identifier combination.
		/// </summary>
		/// <param name="identifier">The identifier for which this bound instance should be returned, used to differentiate between multiple instances of the same class.</param>
		/// <typeparam name="T">The type for which this bound instance should be returned.</typeparam>
		/// <returns>A configurable instance provider.</returns>
		IInstanceProviderBinding<T> Bind<T>(string identifier = null);

		/// <summary>
		/// Gets the binding configuration for a specific Type / identifier combination.
		/// </summary>
		/// <param name="type">The type for which this bound instance should be returned.</param>
		/// <param name="identifier">The identifier for which this bound instance should be returned, used to differentiate between multiple instances of the same class.</param>
		/// <returns>A configurable instance provider.</returns>
		IInstanceProviderBinding Bind(Type type, string identifier = null);

		/// <summary>
		/// Unbinds a specific instance to a type / id pair in the injector, if present.
		/// </summary>
		/// <param name="identifier">The identifier for which the bound instance should be returned, used to differentiate between multiple instances of the same class.</param>
		/// <typeparam name="T">The type for which the bound instance should be unbound.</typeparam>
		/// <returns>Was an entry unbound or were there none present?</returns>
		bool Unbind<T>(string identifier = null);

		/// <summary>
		/// Unbinds a specific instance to a type / id pair in the injector, if present.
		/// </summary>
		/// <param name="type">The type for which the bound instance should be unbound.</param>
		/// <param name="identifier">The identifier for which the bound instance should be returned, used to differentiate between multiple instances of the same class.</param>
		/// <returns>Was an entry unbound or were there none present?</returns>
		bool Unbind(Type type, string identifier = null);

		/// <summary>
		/// Instantiates an object of the given type with the defined constructor while resolving parameters using the injector.
		/// </summary>
		/// <typeparam name="T">Type to instantiate.</typeparam>
		/// <param name="constructorIndex">Index of the constructor to use.</param>
		/// <returns>Instance of type T.</returns>
		T Instantiate<T>(int constructorIndex = 0);

		/// <summary>
		/// Instantiates an object of the given type with the defined constructor while resolving parameters using the injector.
		/// </summary>
		/// <param name="type">Type to instantiate.</param>
		/// <param name="constructorIndex">Index of the constructor to use.</param>
		/// <returns>Instance of type T.</returns>
		object Instantiate(Type type, int constructorIndex = 0);

		/// <summary>
		/// Invoke the method, resolving any parameters using the bound configuration.
		/// </summary>
		/// <param name="methodInfo">The method's method info.</param>
		/// <param name="target">The object the method should be invoked on.</param>
		/// <returns>Any return value the method might have, otherwise null.</returns>
		object InvokeMethod(MethodInfo methodInfo, object target);

		/// <summary>
		/// Gets a specific bound entry from the injector.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		/// <typeparam name="T">The type of the bound entry.</typeparam>
		T Get<T>(string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true);

		/// <summary>
		/// Gets a specific bound entry from the injector.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="type">The type of the bound entry.</typeparam>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		object Get(Type type, string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true);

		/// <summary>
		/// Checks if a specific type / identifier was bound.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		/// <typeparam name="T">The type of the bound entry.</typeparam>
		bool Contains<T>(string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true);

		/// <summary>
		/// Checks if a specific type / identifier was bound.
		/// </summary>
		/// <returns>The bound entry.</returns>
		/// <param name="type">The type of the bound entry.</typeparam>
		/// <param name="identifier">The Identifier used to distinguish between multiple occurences of the same class.</param>
		/// <param name="includeDerivedTypes">If an entry could not be found for the exact type, should derived types be checked?</param>
		/// <param name="searchAggregatedInjectors">If an entry could not be found should aggregated injectors be checked?</param>
		bool Contains(Type type, string identifier = null, bool includeDerivedTypes = true, bool searchAggregatedInjectors = true);

		/// <summary>
		/// Returns all bound entries of the given type, optionally also derived types.
		/// Note that when resolving an entry causes a new entry to be bound that the new entry does not get added as well.
		/// </summary>
		/// <returns>A list of all bound instances of the given type, optionally also derived types.</returns>
		/// <param name="includeDerivedTypes">Should all entries of a derived type also be included?</param>
		/// <typeparam name="T">The type to return.</param>
		List<T> GetAll<T>(bool includeDerivedTypes = true);

		/// <summary>
		/// Returns all bound entries of the given type, optionally also derived types.
		/// Note that when resolving an entry causes a new entry to be bound that the new entry does not get added as well.
		/// </summary>
		/// <returns>A list of all bound instances of the given type, optionally also derived types.</returns>
		/// <param name="type">The type to return.</param>
		/// <param name="includeDerivedTypes">Should all entries of a derived type also be included?</param>
		List<object> GetAll(Type type, bool includeDerivedTypes = true);

		/// <summary>
		/// Does a Get() on each of the bound entries to make sure they resolve once.
		/// Note that when resolving an entry causes a new entry to be bound that the new entry does not get resolved as well.
		/// </summary>
		void ResolveAll();
	}
}
