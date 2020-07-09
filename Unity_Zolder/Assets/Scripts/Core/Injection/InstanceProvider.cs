// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Reflection;
using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// Can provide an instance of type T.
	/// Needs to be configured to know where the instance should come from.
	/// Options include using a passed instance, constructing a new instance or using a factory method.
	/// </summary>
	/// <typeparam name="T">The type the constructed instance should be or derive from.</typeparam>
	public class InstanceProvider<T> : IInstanceProviderBinding, IInstanceProviderBinding<T>, IInstanceResolver
	{
		public Type Type { get { return typeof(T); } }

		public bool BoundToInstance => Instance != null;
		public bool BoundToConstructor => constructorIndex != -1;
		public bool BoundToFunc => factoryMethod != null;

		public string Identifier { get; private set; }

		public bool WasResolved { get; private set; } = false;

		public object Instance { get; private set; }

		private Type constructedType;
		private int constructorIndex = -1;

		private Func<IDependencyInjector, object> factoryMethod;
		private Func<IDependencyInjector, T> typedFactoryMethod;

		private bool isSingleton = true;
		private bool isBeingResolved = false;

		public InstanceProvider(string identifier)
		{
			Identifier = identifier;
			Instance = default(T);
		}

		public void Dispose()
		{
			if (Instance is IDisposable disposable)
			{
				disposable.Dispose();
			}
			Instance = null;
		}

		public void ToInstance(T instance)
		{
			ToInstance((object)instance);
		}

		public void ToInstance(object instance)
		{
			Reset();

			Instance = instance;
			isSingleton = true;
			WasResolved = true;
		}

		public void ToConstructor<X>(int constructorIndex = 0, bool isSingleton = true) where X : T
		{
			ToConstructor(typeof(X), constructorIndex, isSingleton);
		}

		public void ToConstructor(Type type, int constructorIndex = 0, bool isSingleton = true)
		{
			Reset();

			constructedType = type;
			this.constructorIndex = constructorIndex;
			this.isSingleton = isSingleton;
		}

		public void ToFunc(Func<IDependencyInjector, T> factoryMethod, bool isSingleton = true)
		{
			Reset();

			typedFactoryMethod = factoryMethod;
			this.isSingleton = isSingleton;
		}

		public void ToFunc(Func<IDependencyInjector, object> factoryMethod, bool isSingleton = true)
		{
			Reset();

			this.factoryMethod = factoryMethod;
			this.isSingleton = isSingleton;
		}

		public object Resolve(IDependencyInjector injector)
		{
			if (WasResolved)
			{
				return Instance;
			}

			if (isBeingResolved)
			{
				throw new OverflowException("Circular reference detected, asked to resolve again while already" +
					" resolving reference " + Type.Name + " " + Identifier);
			}
			isBeingResolved = true;

			if (constructorIndex != -1)
			{
				object resolvedInstance = ResolveConstructor(injector, constructorIndex);
				if (WasResolved)
				{
					// in case a new instance was bound during the constructor then ignore the result of the factory method.
					return Instance;
				}
				if (isSingleton)
				{
					Instance = resolvedInstance;
					WasResolved = true;
				}
				isBeingResolved = false;
				return Instance;
			}

			if (typedFactoryMethod != null)
			{
				T resolvedInstance = typedFactoryMethod(injector);
				if (WasResolved)
				{
					// in case a new instance was bound during the factory method then ignore the result of the factory method.
					return Instance;
				}
				if (isSingleton)
				{
					Instance = resolvedInstance;
					WasResolved = true;
				}
				isBeingResolved = false;
				return resolvedInstance;
			}

			if (factoryMethod != null)
			{
				object resolvedInstance = factoryMethod(injector);
				if (WasResolved)
				{
					// in case a new instance was bound during the factory method then ignore the result of the factory method.
					return Instance;
				}
				if (isSingleton)
				{
					Instance = resolvedInstance;
					WasResolved = true;
				}
				isBeingResolved = false;
				return Instance;
			}

			isBeingResolved = false;
			return Instance;
		}

		public IInstanceResolver Clone()
		{
			InstanceProvider<T> clone = new InstanceProvider<T>(Identifier)
			{
				Identifier = Identifier,
				constructedType = constructedType,
				constructorIndex = constructorIndex,
				factoryMethod = factoryMethod,
				typedFactoryMethod = typedFactoryMethod,
				isSingleton = isSingleton,
				WasResolved = WasResolved,
				Instance = Instance
			};

			return clone;
		}

		private void Reset()
		{
			isBeingResolved = false;
			WasResolved = false;
			Instance = default(T);
			isSingleton = true;
			factoryMethod = null;
			typedFactoryMethod = null;
			constructedType = null;
			constructorIndex = -1;
		}

		private object ResolveConstructor(IDependencyInjector injector, int constructorIndex)
		{
			ConstructorInfo[] constructors = constructedType.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

			if (constructorIndex >= constructors.Length)
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Constructor index " + constructorIndex + " does not exist on type: " + constructedType);
				return null;
			}

			ConstructorInfo constructorInfo = constructors[constructorIndex];

			object[] parameters = ResolveParameters(injector, constructorInfo, constructedType);
			return constructorInfo.Invoke(parameters);
		}

		private object[] ResolveParameters(IDependencyInjector injector, MethodBase info, Type objType)
		{
			ParameterInfo[] parameters = info.GetParameters();
			object[] objects = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				InjectionIdentifierAttribute attribute = parameterInfo.GetCustomAttribute<InjectionIdentifierAttribute>();
				string identifier = attribute?.Identifier;
				objects[i] = injector.Get(parameters[i].ParameterType, identifier);
			}

			return objects;
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
	}
}