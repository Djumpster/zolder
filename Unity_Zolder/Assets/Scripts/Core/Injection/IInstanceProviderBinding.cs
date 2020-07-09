// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// Lets you define where an instance should come from.
	/// Non-typed version for when you don't know the type at compile time.
	/// Otherwise always use IInstanceProviderBinding<T>;
	/// </summary>
	public interface IInstanceProviderBinding
	{
		Type Type { get; }

		bool BoundToInstance { get; }
		bool BoundToConstructor { get; }
		bool BoundToFunc { get; }

		string Identifier { get; }

		void ToInstance(object instance);

		void ToConstructor(Type type, int constructorIndex = 0, bool isSingleton = true);

		void ToFunc(Func<IDependencyInjector, object> factoryMethod, bool isSingleton = true);
	}

	/// <summary>
	/// Lets you define where an instance should come from.
	/// </summary>
	public interface IInstanceProviderBinding<T>
	{
		Type Type { get; }

		string Identifier { get; }

		void ToInstance(T instance);

		void ToConstructor<X>(int constructorIndex = 0, bool isSingleton = true) where X : T;

		void ToFunc(Func<IDependencyInjector, T> factoryMethod, bool isSingleton = true);
	}
}