// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.MemoryScramble;
using UnityEngine;

namespace Talespin.Core.Foundation.Storage
{
	/// <summary>
	/// A data container is a generic container for data that can be converted to other formats, such as JSON.
	/// It can also be serialized by Unity (for use in the editor).
	/// The data container uses string as the backing type for all supported structs.
	/// </summary>
	[Serializable]
	public class DataContainer : IDataContainer, ISerializationCallbackReceiver
	{
		#region properties
		/// <summary>
		/// Marks this data container as having changed.
		/// Can also be set to false again after it has been "handled".
		/// </summary>
		/// <value><see langword="true" /> if this instance is dirty; otherwise, <see langword="false" />.</value>
		public bool IsDirty { get; set; } = true;

		/// <summary>
		/// Is any child in this data container's hierarchy dirty?
		/// </summary>
		/// <value><see langword="true" /> if any data container in this hierarchy is dirty; otherwise, <see langword="false" />.</value>
		public bool IsHierarchyDirty
		{
			get
			{
				if (IsDirty)
				{
					return true;
				}

				foreach (object obj in keyValuePairs.Values)
				{
					if (obj is DataContainer && ((DataContainer)obj).IsHierarchyDirty)
					{
						return true;
					}
					else if (obj is DataContainer[])
					{
						foreach (DataContainer dc in (DataContainer[])obj)
						{
							if (dc != null && dc.IsHierarchyDirty)
							{
								return true;
							}
						}
					}
				}

				return false;
			}
			set
			{
				// If the whole hierarchy is marked clean then this cleans the data container itself too.
				// If the whole hierarchy is marked dirty then it doesn't imply THIS data container is dirty.
				IsDirty = value == false ? false : IsDirty;

				foreach (object obj in keyValuePairs.Values)
				{
					if (obj is DataContainer)
					{
						DataContainer nestedDataContainer = (DataContainer)obj;
						nestedDataContainer.IsHierarchyDirty = value;
					}
					else if (obj is DataContainer[])
					{
						foreach (DataContainer nestedDataContainer in (DataContainer[])obj)
						{
							if (nestedDataContainer != null)
							{
								nestedDataContainer.IsHierarchyDirty = value;
							}
						}
					}
				}
			}
		}

		public int Count { get { return keyValuePairs.Count; } }

		#endregion

		#region constructor
		public DataContainer() { }

		/// <summary>
		/// Clone constructor.
		/// </summary>
		/// <param name="dataContainer">Data container.</param>
		public DataContainer(IDataContainer dataContainer)
		{
			CopyValues(dataContainer);
		}

		#endregion

		#region private members
		/// <summary>
		/// The internal storage for the key-value pairs.
		/// </summary>
		[SerializeField] private readonly Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

		#endregion

		#region unity serialization members
		// this region is added to implement some members that will allow Unity to serialize the keyValuePairs dictionary.

		/// <summary>
		/// The data contained in this data container converted to JSON so it can be serialized and deserialized.
		/// </summary>
		[SerializeField] private string jsonData;

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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			return GetObject<string>(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, string value)
		{
			if (value == null)
			{
				value = "";
			}

			Set(key, (object)value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public bool GetBool(string key, bool defaultValue = false)
		{
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			return (bool)GetStruct<SecureBool>(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, bool value)
		{
			Set(key, (SecureBool)value as object);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public double GetDouble(string key, double defaultValue = -1)
		{
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			if (keyValuePairs.ContainsKey(key))
			{
				double.TryParse(keyValuePairs[key].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double outDouble);
				keyValuePairs[key] = new SecureDouble(outDouble);
			}
			return (double)GetStruct<SecureDouble>(key, defaultValue);
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, double value)
		{
			Set(key, ((SecureDouble)value).ToString() as object);
		}

		/// <summary>
		/// Returns the stored value if the key exists. Stored internally as a double.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public int GetInt(string key, int defaultValue = -1)
		{
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			DataContainer dc = GetObject<DataContainer>(key, defaultValue);
			if (dc == null && createNewIfNull)
			{
				dc = new DataContainer();
				Set(key, dc);
			}
			return dc;
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, DataContainer value)
		{
			Set(key, (object)value);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public Color GetColor(string key, Color defaultValue = default(Color))
		{
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			IDataContainer dc = GetDataContainer(key);
			if (dc == null || !dc.ContainsKey("r") || !dc.ContainsKey("g") || !dc.ContainsKey("b") || !dc.ContainsKey("a"))
			{
				return defaultValue;
			}

			return new Color(dc.GetFloat("r"), dc.GetFloat("g"), dc.GetFloat("b"), dc.GetFloat("a"));
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, Color value)
		{
			IDataContainer dc = GetDataContainer(key, new DataContainer());
			dc.Set("r", value.r);
			dc.Set("g", value.g);
			dc.Set("b", value.b);
			dc.Set("a", value.a);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public Vector2 GetVector2(string key, Vector2 defaultValue = default(Vector2))
		{
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			IDataContainer dc = GetDataContainer(key);
			if (dc == null || !dc.ContainsKey("x") || !dc.ContainsKey("y"))
			{
				return defaultValue;
			}

			return new Vector2(dc.GetFloat("x"), dc.GetFloat("y"));
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, Vector2 value)
		{
			IDataContainer dc = GetDataContainer(key, new DataContainer());
			dc.Set("x", value.x);
			dc.Set("y", value.y);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public Vector3 GetVector3(string key, Vector3 defaultValue = default(Vector3))
		{
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			IDataContainer dc = GetDataContainer(key);
			if (dc == null || !dc.ContainsKey("x") || !dc.ContainsKey("y") || !dc.ContainsKey("z"))
			{
				return defaultValue;
			}

			return new Vector3(dc.GetFloat("x"), dc.GetFloat("y"), dc.GetFloat("z"));
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, Vector3 value)
		{
			IDataContainer dc = GetDataContainer(key, new DataContainer());
			dc.Set("x", value.x);
			dc.Set("y", value.y);
			dc.Set("z", value.z);
		}

		/// <summary>
		/// Returns the stored value if the key exists.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public Quaternion GetQuaternion(string key, Quaternion defaultValue = default(Quaternion))
		{
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			IDataContainer dc = GetDataContainer(key);
			if (dc == null || !dc.ContainsKey("x") || !dc.ContainsKey("y") || !dc.ContainsKey("z") || !dc.ContainsKey("w"))
			{
				return defaultValue;
			}

			return new Quaternion(dc.GetFloat("x"), dc.GetFloat("y"), dc.GetFloat("z"), dc.GetFloat("w"));
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, Quaternion value)
		{
			IDataContainer dc = GetDataContainer(key, new DataContainer());
			dc.Set("x", value.x);
			dc.Set("y", value.y);
			dc.Set("z", value.z);
			dc.Set("w", value.z);
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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			string[] array = GetObject<string[]>(key, defaultValue);
			if (array == null && createNewIfNull)
			{
				array = new string[0];
				Set(key, array);
			}
			return array;
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, string[] value)
		{
			Set(key, (object)value);
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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			SecureBool[] secureArray = GetObject<SecureBool[]>(key, null);
			if (secureArray == null)
			{
				if (defaultValue != null)
				{
					return defaultValue;
				}
				else if (createNewIfNull)
				{
					bool[] newArray = new bool[0];
					Set(key, newArray);
					return newArray;
				}
				return null;
			}

			bool[] array = new bool[secureArray.Length];
			for (int i = 0; i < secureArray.Length; i++)
			{
				array[i] = secureArray[i];
			}

			return array;
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, bool[] value)
		{
			SecureBool[] secureArray = new SecureBool[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				secureArray[i] = value[i];
			}

			Set(key, secureArray as object);
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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			SecureDouble[] secureArray = GetObject<SecureDouble[]>(key, null);
			if (secureArray == null)
			{
				if (defaultValue != null)
				{
					return defaultValue;
				}
				else if (createNewIfNull)
				{
					double[] newArray = new double[0];
					Set(key, newArray);
					return newArray;
				}
				return null;
			}

			double[] array = new double[secureArray.Length];
			for (int i = 0; i < secureArray.Length; i++)
			{
				array[i] = (double)secureArray[i];
			}

			return array;
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, double[] value)
		{
			SecureDouble[] secureArray = new SecureDouble[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				secureArray[i] = value[i];
			}

			Set(key, secureArray as object);
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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			double[] secureArray = GetDoubleArray(key);
			if (secureArray == null)
			{
				if (defaultValue != null)
				{
					return defaultValue;
				}
				else if (createNewIfNull)
				{
					int[] newArray = new int[0];
					Set(key, newArray);
					return newArray;
				}
				return null;
			}

			int[] intArray = new int[secureArray.Length];
			for (int i = 0; i < secureArray.Length; i++)
			{
				intArray[i] = (int)secureArray[i];
			}
			return intArray;
		}

		/// <summary>
		/// Sets the given value for the given key. Saved internally as double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, int[] value)
		{
			if (value == null)
			{
				Set(key, (double[])null);
				return;
			}

			double[] doubleArray = new double[value.Length];
			for (int i = 0; i < doubleArray.Length; i++)
			{
				doubleArray[i] = value[i];
			}
			Set(key, doubleArray);
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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			double[] secureArray = GetDoubleArray(key);
			if (secureArray == null)
			{
				if (defaultValue != null)
				{
					return defaultValue;
				}
				else if (createNewIfNull)
				{
					float[] newArray = new float[0];
					Set(key, newArray);
					return newArray;
				}
				return null;
			}

			float[] floatArray = new float[secureArray.Length];
			for (int i = 0; i < secureArray.Length; i++)
			{
				floatArray[i] = (float)secureArray[i];
			}
			return floatArray;
		}

		/// <summary>
		/// Sets the given value for the given key. Saved internally as double.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, float[] value)
		{
			if (value == null)
			{
				Set(key, (double[])null);
				return;
			}

			double[] doubleArray = new double[value.Length];
			for (int i = 0; i < doubleArray.Length; i++)
			{
				doubleArray[i] = value[i];
			}
			Set(key, doubleArray);
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
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			DataContainer[] secureArray = GetObject<DataContainer[]>(key, defaultValue);
			if (secureArray == null && createNewIfNull)
			{
				DataContainer[] newArray = new DataContainer[0];
				Set(key, newArray);
				return newArray;
			}

			return secureArray;
		}

		/// <summary>
		/// Sets the given value for the given key.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void Set(string key, DataContainer[] value)
		{
			Set(key, (object)value);
		}

		/// <summary>
		/// Returns the uncast object.
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <param name="defaultValue">The default value to return, if none is found.</param>
		/// <returns>Default value if null.</returns>
		public object GetObject(string key, object defaultValue = null)
		{
			if (key == null)
			{
				LogUtil.Error(LogTags.DATA, this, "Requested data with null key.");
				return defaultValue;
			}

			return GetObject<object>(key, defaultValue);
		}

		/// <summary>
		/// Sets the given uncast object value for the given key.
		/// Uncast object must be one of the supported types and will be cast correctly prior to being saved.
		/// </summary>
		/// <param name="key">The key to store the value with.</param>
		/// <param name="value">The value to store with the key.</param>
		public void SetObject(string key, object value)
		{
			if (value is bool)
			{
				Set(key, (bool)value);
			}
			else if (value is double)
			{
				Set(key, (double)value);
			}
			else if (value is float)
			{
				Set(key, Convert.ToDouble((float)value));
			}
			else if (value is int)
			{
				Set(key, Convert.ToDouble((int)value));
			}
			else if (value is bool[])
			{
				Set(key, value as bool[]);
			}
			else if (value is double[])
			{
				Set(key, value as double[]);
			}
			else if (value is float[])
			{
				float[] val = value as float[];
				double[] ar = new double[val.Length];
				for (int i = 0; i < val.Length; i++)
				{
					ar[i] = Convert.ToDouble((double)val[i]);
				}
			}
			else if (value is int[])
			{
				int[] val = value as int[];
				double[] ar = new double[val.Length];
				for (int i = 0; i < val.Length; i++)
				{
					ar[i] = Convert.ToDouble((double)val[i]);
				}
			}
			else if (value is string ||
				value is DataContainer ||
				value is string[] ||
				value is DataContainer[])
			{
				Set(key, value);
			}
			else
			{
				throw new InvalidOperationException("Attempted to set unsupported object of type " + value.GetType().Name);
			}
		}

		#endregion

		/// <summary>
		/// Returns the type the given key is saved as.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public DataContainerDataType GetTypeForKey(string key)
		{
			if (ContainsKey(key))
			{
				object value = keyValuePairs[key];

				if (value is DataContainer[])
				{
					return DataContainerDataType.DataContainer_Array;
				}
				else if (value is SecureBool[])
				{
					return DataContainerDataType.Bool_Array;
				}
				else if (value is SecureDouble[])
				{
					return DataContainerDataType.Double_Array;
				}
				else if (value is string[])
				{
					return DataContainerDataType.String_Array;
				}
				else if (value is DataContainer)
				{
					return DataContainerDataType.DataContainer;
				}
				else if (value is SecureBool)
				{
					return DataContainerDataType.Bool;
				}
				else if (value is SecureDouble)
				{
					return DataContainerDataType.Double;
				}
				else if (value is string)
				{
					return DataContainerDataType.String;
				}
			}

			return DataContainerDataType.None;
		}

		/// <summary>
		/// Returns the internally stored data in the IDataContainer. Use with caution!
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> GetKeyValuePairs()
		{
			return keyValuePairs;
		}

		/// <summary>
		/// Is there a stored value for the given key?
		/// </summary>
		/// <param name="key">The key for the stored value.</param>
		/// <returns></returns>
		public bool ContainsKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			return keyValuePairs.ContainsKey(key);
		}

		/// <summary>
		/// Returns all the keys of stored values in this data packet.
		/// </summary>
		/// <returns></returns>
		public string[] GetKeys()
		{
			List<string> keys = new List<string>();
			foreach (string key in keyValuePairs.Keys)
			{
				keys.Add(key);
			}

			return keys.ToArray();
		}

		/// <summary>
		/// Remove the key and value for the given key.
		/// </summary>
		/// <param name="key">The key. If it does not exist nothing will happen.</param>
		public void RemoveKey(string key)
		{
			if (keyValuePairs.ContainsKey(key))
			{
				keyValuePairs.Remove(key);
				IsDirty = true;
			}
		}

		/// <summary>
		/// Clear all the key value pairs stored in this data packet.
		/// </summary>
		public void Clear()
		{
			if (keyValuePairs.Count > 0)
			{
				keyValuePairs.Clear();
				IsDirty = true;
			}
		}

		/// <summary>
		/// Copies the values from the source into this data container.
		/// </summary>
		/// <param name="source">Source.</param>
		public void CopyValues(IDataContainer source)
		{
			if (source == null)
			{
				return;
			}

			Dictionary<string, object> sourceKVP = source.GetKeyValuePairs();
			foreach (KeyValuePair<string, object> kvp in sourceKVP)
			{
				if (kvp.Value is DataContainer[])
				{
					DataContainer[] array = kvp.Value as DataContainer[];
					DataContainer[] cloneArray = new DataContainer[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null)
						{
							cloneArray[i] = new DataContainer(array[i]);
						}
					}
					Set(kvp.Key, cloneArray);
				}
				else if (kvp.Value is SecureBool[])
				{
					Set(kvp.Key, ((kvp.Value as SecureBool[]).Clone()) as SecureBool[]);
				}
				else if (kvp.Value is SecureDouble[])
				{
					Set(kvp.Key, ((kvp.Value as SecureDouble[]).Clone()) as SecureDouble[]);
				}
				else if (kvp.Value is string[])
				{
					Set(kvp.Key, ((kvp.Value as string[]).Clone()) as string[]);
				}
				else if (kvp.Value is DataContainer)
				{
					Set(kvp.Key, new DataContainer(kvp.Value as DataContainer));
				}
				else if (kvp.Value is string ||
					kvp.Value is SecureDouble ||
					kvp.Value is SecureBool)
				{
					Set(kvp.Key, kvp.Value);
				}
			}
		}

		/// <summary>
		/// Override the ToString() method to easily debug the data packets.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetType().Name + " contains key-value pairs:");
			foreach (string key in keyValuePairs.Keys)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(key);
				stringBuilder.Append(": ");
				stringBuilder.Append(keyValuePairs[key]);
			}

			return stringBuilder.ToString();
		}

		#region Unity serialization callbacks
		// this region is added to implement some members that will allow Unity to serialize the keyValuePairs dictionary.

		/// <summary>
		/// Unity callback used to serialize the non-serializable keyValuePairs into 2 seperate, serializable lists.
		/// </summary>
		public void OnBeforeSerialize()
		{
			jsonData = DataContainerJSONSerializer.Serialize(this);
		}

		/// <summary>
		/// Unity callback used to deserialize the 2 seperate, serializable lists back into the non-serializable keyValuePairs 
		/// Dictionary<string, string>.
		/// </summary>
		public void OnAfterDeserialize()
		{
			IDataContainer dataContainer = DataContainerJSONSerializer.Deserialize(jsonData);
			foreach (string key in dataContainer.GetKeys())
			{
				Set(key, dataContainer.GetObject(key));
			}
		}

		#endregion
		#endregion

		#region private methods
		private void Set(string key, object value)
		{
			if (string.IsNullOrEmpty(key))
			{
				LogUtil.Error(LogTags.DATA, this, "Cannot add value with an empty key!");
				return;
			}

			if (keyValuePairs.ContainsKey(key))
			{
				if (keyValuePairs[key] != value)
				{
					keyValuePairs[key] = value;
					IsDirty = true;
				}
			}
			else
			{
				keyValuePairs.Add(key, value);
				IsDirty = true;
			}
		}

		private T GetObject<T>(string key, T defaultValue = null) where T : class
		{
			if (string.IsNullOrEmpty(key))
			{
				key = "";
				LogUtil.Warning(LogTags.DATA, this, "Requesting value with empty key, is this intended?");
			}

			object val = null;
			bool exists = keyValuePairs.TryGetValue(key, out val);
			if (val is T)
			{
				return (T)val;
			}
			if (defaultValue != null)
			{
				if (exists)
				{
					keyValuePairs[key] = defaultValue;
				}
				else
				{
					keyValuePairs.Add(key, defaultValue);
				}
			}

			return defaultValue;
		}

		private T GetStruct<T>(string key, T defaultValue) where T : struct
		{
			if (string.IsNullOrEmpty(key))
			{
				key = "";
				LogUtil.Warning(LogTags.DATA, this, "Requesting value with empty key, is this intended?");
			}

			if (keyValuePairs.ContainsKey(key) && keyValuePairs[key] is T)
			{
				return (T)keyValuePairs[key];
			}

			keyValuePairs.Remove(key);
			keyValuePairs.Add(key, defaultValue);

			return defaultValue;
		}

		#endregion
	}
}
