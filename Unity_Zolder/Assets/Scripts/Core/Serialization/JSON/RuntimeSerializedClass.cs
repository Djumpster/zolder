// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.AssetHandling;
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	/// <summary>
	/// The base serialized class. This class stores data
	/// for a specific instance of type T, allowing instantation
	/// during runtime.
	/// </summary>
	/// <seealso cref="EditorSerializedClass{T}"/>
	[Serializable]
	public class RuntimeSerializedClass<T> : IRuntimeSerializedClass, ISerializedRuntimeGeneratedDataHandler where T : class
	{
		public Type DataType
		{
			get
			{
				GuidDatabaseManager guidDatabaseManager = GuidDatabaseManager.Instance;

				Type result = guidDatabaseManager.MapGuidToType(type);
				name = result != null ? result.FullName : name;

				return result;
			}
		}

		public string Identifier => identifier;
		public string FullTypeString => type;

		/// <inheritdoc/>
		public string Name => name;

		public bool Enabled
		{
			set => enabled = value;
			get => enabled;
		}

		[SerializeField] protected string data;
		[SerializeField] protected string type;
		[SerializeField] protected string identifier;
		[SerializeField] protected string name;
		[SerializeField] protected bool enabled;

		private IRuntimeGeneratedData runtimeGeneratedData; //Always stored at runtime, doesn't need the SerializeField attribute

		public RuntimeSerializedClass(string type, string data, string name)
		{
			this.data = data;
			this.type = type;
			this.name = name;

			identifier = Guid.NewGuid().ToString();
			enabled = true;
		}

		public RuntimeSerializedClass() : this(string.Empty, "{}", string.Empty)
		{
		}

		public RuntimeSerializedClass(RuntimeSerializedClass<T> other) : this(other.data, other.type, other.name)
		{
		}

		public void StoreRuntimeSerializedData(string type, string identifier, bool enabled, IRuntimeGeneratedData runtimeGeneratedData)
		{
			this.type = type;
			this.identifier = identifier;
			this.enabled = enabled;
			this.runtimeGeneratedData = runtimeGeneratedData;			
		}

		public T Instantiate()
		{
			if (DataType == null || DataType.IsAbstract)
			{
				return default;
			}
			else
			{
				T instance = JsonUtility.FromJson(data, DataType) as T;

				if (instance != null && runtimeGeneratedData != null)
				{
					IInjectRuntimeGeneratedData injectable  = instance as IInjectRuntimeGeneratedData;
					if (injectable != null)
					{
						injectable.InjectRuntimeGeneratedData(runtimeGeneratedData, GlobalDependencyLocator.Instance);
					}
				}
				return instance;
			}
		}

		public bool HasData() => !string.IsNullOrEmpty(type);
	}
}
