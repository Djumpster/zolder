// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Storage
{
	/// <summary>
	/// Replacement for PlayerPrefs using the LocalDataManager if you're too lazy to use the LocalDataManager directly.
	/// This is not the recommended way of using the LocalDataManager, but it's better than (and just as easy as)
	/// using PlayerPrefs...
	/// </summary>
	public static class LazyPlayerPrefs
	{
		private static LocalDataManager localDataManager { get { return GlobalDependencyLocator.Instance.Get<LocalDataManager>(); } }
		private static DataPacket dataPacket { get { return localDataManager.GetDataPacket("LazyPlayerPrefs", true); } }

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static string GetString(string key, string defaultValue = "")
		{
			return dataPacket.GetString(key, defaultValue);
		}

		public static void SetString(string key, string value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, string value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static bool GetBool(string key, bool defaultValue = false)
		{
			return dataPacket.GetBool(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, bool value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static double GetDouble(string key, double defaultValue = -1)
		{
			return dataPacket.GetDouble(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, double value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static int GetInt(string key, int defaultValue = -1)
		{
			return dataPacket.GetInt(key, defaultValue);
		}

		public static void SetInt(string key, int value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Sets the given value for the given key. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, int value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static float GetFloat(string key, float defaultValue = -1f)
		{
			return dataPacket.GetFloat(key, defaultValue);
		}

		public static void SetFloat(string key, float value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Sets the given value for the given key. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, float value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static DataContainer GetDataContainer(string key, DataContainer defaultValue = null)
		{
			return dataPacket.GetDataContainer(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, DataContainer value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static Color GetColor(string key, Color defaultValue = default(Color))
		{
			return dataPacket.GetColor(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, Color value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static Vector2 GetVector2(string key, Vector2 defaultValue = default(Vector2))
		{
			return dataPacket.GetVector2(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, Vector2 value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static Vector3 GetVector3(string key, Vector3 defaultValue = default(Vector3))
		{
			return dataPacket.GetVector3(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, Vector3 value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static Quaternion GetQuaternion(string key, Quaternion defaultValue = default(Quaternion))
		{
			return dataPacket.GetQuaternion(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, Quaternion value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static string[] GetStringArray(string key, string[] defaultValue = null)
		{
			return dataPacket.GetStringArray(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, string[] value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static bool[] GetBoolArray(string key, bool[] defaultValue = null)
		{
			return dataPacket.GetBoolArray(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, bool[] value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static double[] GetDoubleArray(string key, double[] defaultValue = null)
		{
			return dataPacket.GetDoubleArray(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, double[] value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists. Saved internally as double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static int[] GetIntArray(string key, int[] defaultValue = null)
		{
			return dataPacket.GetIntArray(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key. Saved internally as double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, int[] value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists. Saved internally as double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static float[] GetFloatArray(string key, float[] defaultValue = null)
		{
			return dataPacket.GetFloatArray(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key. Saved internally as double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, float[] value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public static DataContainer[] GetDataContainerArray(string key, DataContainer[] defaultValue = null)
		{
			return dataPacket.GetDataContainerArray(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public static void Set(string key, DataContainer[] value)
		{
			dataPacket.Set(key, value);
			Save();
		}

		public static bool HasKey(string key)
		{
			return ContainsKey(key);
		}

		/// <summary>
		/// Is there a stored value for the given key?
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <returns></returns>
		public static bool ContainsKey(string key)
		{
			return dataPacket.ContainsKey(key);
		}

		/// <summary>
		/// Returns all the keys of stored values in this data container.
		/// </summary>
		/// <returns></returns>
		public static string[] GetKeys()
		{
			return dataPacket.GetKeys();
		}

		public static void DeleteKey(string key)
		{
			RemoveKey(key);
		}

		/// <summary>
		/// Remove the key and value for the given key.
		/// </summary>
		/// <param name="key">The key. If it does not exist nothing will happen.</param>
		public static void RemoveKey(string key)
		{
			dataPacket.RemoveKey(key);
		}

		/// <summary>
		/// Clear all the key value pairs stored in this data container.
		/// </summary>
		public static void Clear()
		{
			dataPacket.Clear();
		}

		public static void Save()
		{
			localDataManager.Save(dataPacket);
		}

	}
}
