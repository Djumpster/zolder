// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Storage
{
	/// <summary>
	/// A data container is a container for data that can be saved to local storage (hard disk etc.) or synced between
	/// a client and server.
	/// </summary>
	public interface IDataContainer
	{
		#region properties
		/// <summary>
		/// Marks this data container as having changed.
		/// Can also be set to false again after it has been "handled".
		/// </summary>
		/// <value><see langword="true" /> if this instance is dirty; otherwise, <see langword="false" />.</value>
		bool IsDirty { get; set; }

		/// <summary>
		/// Is any child in this data container's hierarchy dirty?
		/// </summary>
		/// <value><see langword="true" /> if any data container in this hierarchy is dirty; otherwise, <see langword="false" />.</value>
		bool IsHierarchyDirty { get; set; }

		/// <summary>
		/// The amount of key-value pairs in the DataContainer.
		/// </summary>
		int Count { get; }

		#endregion

		#region public methods
		#region getters & setters
		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		string GetString(string key, string defaultValue = "");

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, string value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		bool GetBool(string key, bool defaultValue = false);

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, bool value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		double GetDouble(string key, double defaultValue = -1);

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, double value);

		/// <summary>
		/// Returns the stored value if the key exists. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		int GetInt(string key, int defaultValue = -1);

		/// <summary>
		/// Sets the given value for the given key. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, int value);

		/// <summary>
		/// Returns the stored value if the key exists. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		float GetFloat(string key, float defaultValue = -1f);

		/// <summary>
		/// Sets the given value for the given key. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, float value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		DataContainer GetDataContainer(string key, DataContainer defaultValue = null, bool createNewIfNull = false);

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, DataContainer value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		Color GetColor(string key, Color defaultValue = default(Color));

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, Color value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		Vector2 GetVector2(string key, Vector2 defaultValue = default(Vector2));

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, Vector2 value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		Vector3 GetVector3(string key, Vector3 defaultValue = default(Vector3));

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, Vector3 value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		Quaternion GetQuaternion(string key, Quaternion defaultValue = default(Quaternion));

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, Quaternion value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		string[] GetStringArray(string key, string[] defaultValue = null, bool createNewIfNull = false);

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, string[] value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		bool[] GetBoolArray(string key, bool[] defaultValue = null, bool createNewIfNull = false);

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, bool[] value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		double[] GetDoubleArray(string key, double[] defaultValue = null, bool createNewIfNull = false);

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, double[] value);

		/// <summary>
		/// Returns the stored value if the key exists. Saved internally as double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		int[] GetIntArray(string key, int[] defaultValue = null, bool createNewIfNull = false);

		/// <summary>
		/// Sets the given value for the given key. Saved internally as double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, int[] value);

		/// <summary>
		/// Returns the stored value if the key exists. Saved internally as double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		float[] GetFloatArray(string key, float[] defaultValue = null, bool createNewIfNull = false);

		/// <summary>
		/// Sets the given value for the given key. Saved internally as double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, float[] value);

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <param name="createNewIfNull">Create a new instance and add it to the data container if the return 
		/// value is null? Use this with a null default parameter to only instantiate a new instance when actually
		/// necesairy.</param>
		/// <returns>Default value if null.</returns>
		DataContainer[] GetDataContainerArray(string key, DataContainer[] defaultValue = null, bool createNewIfNull = false);

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void Set(string key, DataContainer[] value);

		/// <summary>
		/// Returns the uncast object.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		object GetObject(string key, object defaultValue = null);

		/// <summary>
		/// Sets the given uncast object value for the given key.
		/// Uncast object must be one of the supported types and will be cast correctly prior to being saved.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		void SetObject(string key, object value);
		#endregion

		/// <summary>
		/// Returns the type the given key is saved as.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		DataContainerDataType GetTypeForKey(string key);

		/// <summary>
		/// Returns the internally stored data in the IDataContainer. Use with caution!
		/// </summary>
		/// <returns></returns>
		Dictionary<string, object> GetKeyValuePairs();

		/// <summary>
		/// Is there a stored value for the given key?
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <returns></returns>
		bool ContainsKey(string key);

		/// <summary>
		/// Returns all the keys of stored values in this data container.
		/// </summary>
		/// <returns></returns>
		string[] GetKeys();

		/// <summary>
		/// Remove the key and value for the given key.
		/// </summary>
		/// <param name="key">The key. If it does not exist nothing will happen.</param>
		void RemoveKey(string key);

		/// <summary>
		/// Clear all the key value pairs stored in this data container.
		/// </summary>
		void Clear();

		/// <summary>
		/// Copies the values from the source into this data container.
		/// </summary>
		/// <param name="source">Source.</param>
		void CopyValues(IDataContainer source);
		#endregion
	}
}