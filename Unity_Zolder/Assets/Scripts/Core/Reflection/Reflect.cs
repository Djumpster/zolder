// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Talespin.Core.Foundation.Extensions;
using UnityEngine;

namespace Talespin.Core.Foundation.Reflection
{
	[Flags]
	public enum InclusionFlags
	{
		None = 0,

		/// <summary>
		/// Instantiable types, so no interfaces or abstract types
		/// </summary>
		InstantiableTypes = 1,

		/// <summary>
		/// Abstract types
		/// </summary>
		AbstractTypes = 2,

		/// <summary>
		/// Interfaces
		/// </summary>
		InterfaceTypes = 4,

		/// <summary>
		/// If specified the provided base type will be included in the results
		/// </summary>
		BaseType = 8,

		/// <summary>
		/// If specified all assemblies will be checked
		/// </summary>
		AllAssemblies = 16,

		/// <summary>
		/// Shortcut for <see cref="InstantiableTypes"/>, <see cref="AbstractTypes"/>, and <see cref="InterfaceTypes"/>
		/// </summary>
		AllTypes = InstantiableTypes | AbstractTypes | InterfaceTypes,

		/// <summary>
		/// The default is <see cref="InstantiableTypes"/> and <see cref="AllAssemblies"/>
		/// </summary>
		Default = InstantiableTypes | AllAssemblies,

		/// <summary>
		/// Every valid type
		/// </summary>
		Everything = AllTypes | AllAssemblies | BaseType
	}

	public static class Reflect
	{
		private struct CacheKey
		{
			public Assembly Assembly;
			public Type Type;
			public InclusionFlags InclusionFlags;
			public string NamespaceFilter;

			public CacheKey(Assembly assembly, Type type, string namespaceFilter, InclusionFlags inclusionFlags)
			{
				Assembly = assembly;
				Type = type;
				NamespaceFilter = namespaceFilter;
				InclusionFlags = inclusionFlags;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}

				if (ReferenceEquals(this, obj))
				{
					return true;
				}

				if (obj.GetType() != typeof(CacheKey))
				{
					return false;
				}

				CacheKey other = (CacheKey)obj;
				return Assembly == other.Assembly && Type == other.Type && NamespaceFilter == other.NamespaceFilter && InclusionFlags == other.InclusionFlags;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (Assembly != null ? Assembly.GetHashCode() : 0) ^ (Type != null ? Type.GetHashCode() : 0) ^ (NamespaceFilter != null ? NamespaceFilter.GetHashCode() : 0) ^ InclusionFlags.GetHashCode();
				}
			}

		}

		private static readonly Dictionary<Type, FieldInfo[]> cachedFieldInfo;
		private static readonly Dictionary<CacheKey, Type[]> cachedTypes;

		private static readonly string[] assemblyBlackList;

		static Reflect()
		{
			cachedFieldInfo = new Dictionary<Type, FieldInfo[]>();
			cachedTypes = new Dictionary<CacheKey, Type[]>();

			assemblyBlackList = new string[]
			{
				"Accessibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
			};
		}

		public static object Instantiate(string classType, params object[] parameters)
		{
			Type type = GetType(classType, InclusionFlags.AllAssemblies);

			if (type == null)
			{
				throw new ArgumentException(classType + " is not a valid class type.", nameof(classType));
			}

			return Instantiate(type, parameters);
		}

		public static object Instantiate(Type classType, params object[] parameters)
		{
			return Activator.CreateInstance(classType, parameters);
		}

		public static bool TypeExists(string classType)
		{
			return Type.GetType(classType) != null;
		}

		public static IEnumerable<Type> AllTypesWithAttribute<T>()
		{
			return AllTypesWithAttribute(typeof(T));
		}

		public static IEnumerable<Type> AllTypesWithAttribute(Type type)
		{
			return AllTypesWithAttribute(type, null);
		}

		public static IEnumerable<Type> AllTypesWithAttribute<T>(string namespaceFilter)
		{
			return AllTypesWithAttribute(typeof(T), namespaceFilter);
		}

		public static IEnumerable<Type> AllTypesWithAttribute(Type type, string namespaceFilter)
		{
			List<Type> types = new List<Type>();
			foreach (Assembly asm in GetAllAssemblies())
			{
				try
				{
					IEnumerable<Type> currentTypes = asm.GetTypes().Where(t =>
					{
						if (namespaceFilter != null && t.Namespace != namespaceFilter)
						{
							return false;
						}

						return t.IsDefined(type);
					});

					types.AddRange(currentTypes);
				}
				catch (ReflectionTypeLoadException e)
				{
					Debug.LogWarning("[Reflect] Failed to load assembly " + asm.FullName + ", exception: " + e);
				}
			}

			return types;
		}

		public static IEnumerable<Type> AllTypes()
		{
			return AllTypes(null);
		}

		public static IEnumerable<Type> AllTypes(string namespaceFilter)
		{
			List<Type> types = new List<Type>();

			foreach (Assembly asm in GetAllAssemblies())
			{
				try
				{
					IEnumerable<Type> currentTypes = asm.GetTypes().Where(t =>
					{
						if (namespaceFilter != null && t.Namespace != namespaceFilter)
						{
							return false;
						}

						return true;
					});

					types.AddRange(currentTypes);
				}
				catch (ReflectionTypeLoadException e)
				{
					Debug.LogWarning("[Reflect] Failed to load assembly " + asm.FullName + ", exception: " + e);
				}
			}

			return types;
		}

		public static IEnumerable<Type> AllTypesFrom<T>()
		{
			return AllTypesFrom(typeof(T));
		}

		public static IEnumerable<Type> AllTypesFrom<T>(string namespaceFilter, InclusionFlags inclusionFlags = InclusionFlags.Default)
		{
			return AllTypesFrom(typeof(T), namespaceFilter, inclusionFlags);
		}

		public static IEnumerable<Type> AllTypesFrom(Type type, string namespaceFilter = null, InclusionFlags inclusionFlags = InclusionFlags.Default)
		{
			if (!inclusionFlags.HasFlag(InclusionFlags.AllAssemblies))
			{
				return (AllTypesFrom(typeof(Reflect).Assembly/*type.GetAssembly()*/, type, namespaceFilter, inclusionFlags));
			}

			List<Type> types = new List<Type>();
			foreach (Assembly asm in GetAllAssemblies())
			{
				types.AddRange(AllTypesFrom(asm, type, namespaceFilter, inclusionFlags));
			}

			return types;
		}

		public static IEnumerable<Type> AllTypesFrom(Assembly targetAssembly, Type type, string namespaceFilter = null, InclusionFlags inclusionFlags = InclusionFlags.Default)
		{
			CacheKey key = new CacheKey(targetAssembly, type, namespaceFilter, inclusionFlags);

			if (!cachedTypes.ContainsKey(key))
			{
				try
				{
					Type[] found = targetAssembly.GetTypes();
					List<Type> result = new List<Type>(found.Length);

					foreach (Type t in found)
					{
						if (TypeFilterFunc(t, type, namespaceFilter, inclusionFlags))
						{
							result.Add(t);
						}
					}

					cachedTypes.Add(key, result.ToArray());
				}
				catch (ReflectionTypeLoadException exception)
				{
					Debug.LogWarning("Failed to load assembly " + targetAssembly.FullName + ", exception: " + exception);
					return Array.Empty<Type>();
				}
			}

			return cachedTypes[key];
		}

		public static IEnumerable<string> AllTypeStringsFrom<T>(string namespaceFilter = null, Func<Type, bool> filterFunc = null, InclusionFlags inclusionFlags = InclusionFlags.Default)
		{
			return AllTypeStringsFrom(typeof(T), namespaceFilter, filterFunc, inclusionFlags);
		}

		public static IEnumerable<string> AllTypeStringsFrom(Type t, string namespaceFilter = null, Func<Type, bool> filterFunc = null, InclusionFlags inclusionFlags = InclusionFlags.Default)
		{
			IEnumerable<Type> types = AllTypesFrom(t, namespaceFilter, inclusionFlags);
			List<string> ret = new List<string>(types.Count());

			foreach (Type type in types)
			{
				if (filterFunc == null || filterFunc(type))
				{
					ret.Add(type.FullName);
				}
			}

			return ret;
		}

		public static FieldInfo[] GetPublicFields(Type t)
		{
			if (cachedFieldInfo.ContainsKey(t))
			{
				return cachedFieldInfo[t];
			}

			return cachedFieldInfo[t] = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
		}

		public static Type GetType(string typeName, InclusionFlags inclusionFlags = InclusionFlags.AllAssemblies)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				return null;
			}

			Type type = Type.GetType(typeName);

			if (type == null && inclusionFlags.HasFlag(InclusionFlags.AllAssemblies))
			{
				foreach (Assembly a in GetAllAssemblies())
				{
					type = a.GetType(typeName);

					if (type != null)
					{
						return type;
					}
				}
			}

			return type;
		}

		public static IEnumerable<FieldInfo> GetFieldInfos(Type type)
		{
			List<FieldInfo> fieldInfo = new List<FieldInfo>(type.GetFields());

			if (type.BaseType != null)
			{
				fieldInfo.AddRange(GetFieldInfos(type.BaseType));
			}

			return fieldInfo;
		}

		private static bool TypeFilterFunc(Type type, Type baseType, string namespaceFilter, InclusionFlags inclusionFlags)
		{
			// Filter out the base type
			if (!inclusionFlags.HasFlag(InclusionFlags.BaseType) && type == baseType)
			{
				return false;
			}

			// Filter out abstract types
			if (!inclusionFlags.HasFlag(InclusionFlags.AbstractTypes) && type.IsAbstract)
			{
				return false;
			}

			// Filter out interface types
			if (!inclusionFlags.HasFlag(InclusionFlags.InterfaceTypes) && type.IsInterface)
			{
				return false;
			}

			// Filter out instantiable types
			if (!inclusionFlags.HasFlag(InclusionFlags.InstantiableTypes) && !type.IsAbstract && !type.IsInterface)
			{
				return false;
			}

			// If we're filtering on namespaces and the namespace doesn't match then discard this entry.
			if (namespaceFilter != null && type.Namespace != namespaceFilter)
			{
				return false;
			}

			foreach (Type interfaceType in type.GetInterfaces())
			{
				if (baseType.IsAssignableFrom(interfaceType))
				{
					return true;
				}
			}

			return baseType.IsAssignableFrom(type);
		}

		private static IEnumerable<Assembly> GetAllAssemblies()
		{
			List<Assembly> assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());

			for (int i = assemblies.Count - 1; i >= 0; i--)
			{
				if (assemblyBlackList.IndexOf(assemblies[i].FullName) != -1)
				{
					assemblies.RemoveAt(i);
				}
			}

			return assemblies;
		}
	}
}
