// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Talespin.Core.Foundation.Parsing;
using UnityEngine;

namespace Talespin.Core.Foundation.Storage
{
	/// <summary>
	/// An identifiable packet of data with the same functionality (roughly) as JSON and the PlayerPrefs.
	/// </summary>
	[Serializable]
	public class DataPacket : IDataContainer
	{
		#region properties
		public string Identifier
		{
			get
			{
				return identifier;
			}
#if UNITY_EDITOR
			set
			{
				if (identifier != value)
				{
					identifier = value;
					IsDirty = true;
				}
			}
#endif
		}
		[JSON.JsonSerializeField] private string identifier;

		[JSON.JsonSerializeField] private DataContainer data;

		/// <summary>
		/// Marks this data container as having changed.
		/// Can also be set to false again after it has been "handled".
		/// </summary>
		/// <value><see langword="true" /> if this instance is dirty; otherwise, <see langword="false" />.</value>
		public bool IsDirty { get { return data.IsDirty; } set { data.IsDirty = value; } }

		/// <summary>
		/// Is any child in this data container's hierarchy dirty?
		/// </summary>
		/// <value><see langword="true" /> if any data container in this hierarchy is dirty; otherwise, <see langword="false" />.</value>
		public bool IsHierarchyDirty { get { return data.IsHierarchyDirty; } set { data.IsHierarchyDirty = value; } }

		/// <summary>
		/// The amount of key-value pairs in the DataContainer.
		/// </summary>
		public int Count { get { return data.Count; } }

		#endregion

		#region constructor
		public DataPacket(string identifier, DataContainer data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("[Data] data cannot be null.");
			}
			if (string.IsNullOrEmpty(identifier))
			{
				throw new ArgumentNullException("[Data] identifier cannot be null or empty.");
			}

			this.identifier = identifier;
			this.data = data;
		}

		public DataPacket(string identifier) : this(identifier, new DataContainer()) { }

		#endregion

		#region public methods
		#region getters & setters
		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public string GetString(string key, string defaultValue = "")
		{
			return data.GetString(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, string value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public bool GetBool(string key, bool defaultValue = false)
		{
			return data.GetBool(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, bool value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public double GetDouble(string key, double defaultValue = -1)
		{
			return data.GetDouble(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, double value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public int GetInt(string key, int defaultValue = -1)
		{
			return (int)GetDouble(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, int value)
		{
			Set(key, (double)value);
		}

		/// <summary>
		/// Returns the stored value if the key exists. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public float GetFloat(string key, float defaultValue = -1f)
		{
			return (float)GetDouble(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, float value)
		{
			Set(key, (double)value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		public DataContainer GetDataContainer(string key, DataContainer defaultValue = null, bool createNewIfNull = false)
		{
			return data.GetDataContainer(key, defaultValue, createNewIfNull);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, DataContainer value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public Color GetColor(string key, Color defaultValue = default(Color))
		{
			return data.GetColor(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, Color value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public Vector2 GetVector2(string key, Vector2 defaultValue = default(Vector2))
		{
			return data.GetVector2(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, Vector2 value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public Vector3 GetVector3(string key, Vector3 defaultValue = default(Vector3))
		{
			return data.GetVector3(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, Vector3 value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public Quaternion GetQuaternion(string key, Quaternion defaultValue = default(Quaternion))
		{
			return data.GetQuaternion(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, Quaternion value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		public string[] GetStringArray(string key, string[] defaultValue = null, bool createNewIfNull = false)
		{
			return data.GetStringArray(key, defaultValue, createNewIfNull);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, string[] value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		public bool[] GetBoolArray(string key, bool[] defaultValue = null, bool createNewIfNull = false)
		{
			return data.GetBoolArray(key, defaultValue, createNewIfNull);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, bool[] value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		public double[] GetDoubleArray(string key, double[] defaultValue = null, bool createNewIfNull = false)
		{
			return data.GetDoubleArray(key, defaultValue, createNewIfNull);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, double[] value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists. Saved internally as double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		public int[] GetIntArray(string key, int[] defaultValue = null, bool createNewIfNull = false)
		{
			return data.GetIntArray(key, defaultValue, createNewIfNull);
		}

		/// <summary>
		/// Sets the given value for the given key. Saved internally as double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, int[] value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists. Saved internally as double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		public float[] GetFloatArray(string key, float[] defaultValue = null, bool createNewIfNull = false)
		{
			return data.GetFloatArray(key, defaultValue, createNewIfNull);
		}

		/// <summary>
		/// Sets the given value for the given key. Saved internally as double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, float[] value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		public DataContainer[] GetDataContainerArray(string key, DataContainer[] defaultValue = null, bool createNewIfNull = false)
		{
			return data.GetDataContainerArray(key, defaultValue, createNewIfNull);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, DataContainer[] value)
		{
			data.Set(key, value);
		}

		/// <summary>
		/// Returns the uncast object.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public object GetObject(string key, object defaultValue = null)
		{
			return data.GetObject(key, defaultValue);
		}

		/// <summary>
		/// Sets the given uncast object value for the given key.
		/// Uncast object must be one of the supported types and will be cast correctly prior to being saved.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void SetObject(string key, object value)
		{
			data.SetObject(key, value);
		}
		#endregion

		/// <summary>
		/// Returns the type the given key is saved as.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public DataContainerDataType GetTypeForKey(string key)
		{
			return data.GetTypeForKey(key);
		}

		/// <summary>
		/// Is there a stored value for the given key?
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <returns></returns>
		public bool ContainsKey(string key)
		{
			return data.ContainsKey(key);
		}

		/// <summary>
		/// Returns all the keys of stored values in this data packet.
		/// </summary>
		/// <returns></returns>
		public string[] GetKeys()
		{
			return data.GetKeys();
		}

		/// <summary>
		/// Returns the internally stored data in the IDataContainer. Use with caution!
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> GetKeyValuePairs()
		{
			return data.GetKeyValuePairs();
		}

		/// <summary>
		/// Remove the key and value for the given key.
		/// </summary>
		/// <param name="key">The key. If it does not exist nothing will happen.</param>
		public void RemoveKey(string key)
		{
			data.RemoveKey(key);
		}

		/// <summary>
		/// Clear all the key value pairs stored in this data packet.
		/// </summary>
		public void Clear()
		{
			data.Clear();
		}

		/// <summary>
		/// Copies the values from the source into this data container.
		/// </summary>
		/// <param name="source">Source.</param>
		public void CopyValues(IDataContainer source)
		{
			data.CopyValues(source);
		}

		/// <summary>
		/// Override the ToString() method to easily debug the data packets.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(identifier);
			stringBuilder.Append(", containing key-value pairs:");
			stringBuilder.Append(data.ToString());

			return stringBuilder.ToString();
		}

		#endregion
	}
}
