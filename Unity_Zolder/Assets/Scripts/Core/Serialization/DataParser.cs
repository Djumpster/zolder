// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Talespin.Core.Foundation.Parsing;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public static class DataParser
	{
		public static DataEntry.DataType GetDatatype(Type type)
		{
			if (typeof(byte).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Byte;
			}
			else if (typeof(bool).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Boolean;
			}
			else if (typeof(int).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Int;
			}
			else if (typeof(float).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Float;
			}
			else if (typeof(Vector2).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Vector2;
			}
			else if (typeof(Vector3).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Vector3;
			}
			else if (typeof(Vector4).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Vector4;
			}
			else if (typeof(Quaternion).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Quaternion;
			}
			else if (typeof(Color32).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Color;
			}
			else if (typeof(string).IsAssignableFrom(type))
			{
				return DataEntry.DataType.String;
			}
			else if (type.IsEnum)
			{
				return DataEntry.DataType.Enum;
			}
			else if (type.IsArray || typeof(ICollection).IsAssignableFrom(type))
			{
				return DataEntry.DataType.Array;
			}
			else if (type.IsClass || type.IsValueType)
			{
				return DataEntry.DataType.Class;
			}

			throw new ArgumentException("[DataParser] Unsupported type: " + type.FullName);
		}

		public static DataEntry Parse(Type type)
		{
			//We need to create a instance from the type to be able to read the data. 
			//First we try to create is using a empty constructor, so we get proper default values.
			//If that fails we create it without a constructor. (all fields will be initialised to their type default).
			object instance = null;
			try { instance = Activator.CreateInstance(type); }
			catch { }
			if (instance == null)
			{
				instance = FormatterServices.GetUninitializedObject(type);
			}

			return Parse(instance);
		}

		public static DataEntry Parse(object obj, params string[] tags)
		{
			if (obj == null)
			{
				throw new ArgumentException("[DataParser] Input cannot be null");
			}

			Type type = obj.GetType();
			IEnumerable<string> typeTags = type.GetCustomAttributes(typeof(DataTagAttribute), false).Select(o => (o as DataTagAttribute).Tag);
			tags = tags.Concat(typeTags).ToArray();

			DataEntry.DataType dataType = GetDatatype(type);
			switch (dataType)
			{
				case DataEntry.DataType.Byte:
					return new DataEntry(dataType, new DataEntry.ByteEntry((byte)obj), tags);
				case DataEntry.DataType.Boolean:
					return new DataEntry(dataType, new DataEntry.BooleanEntry((bool)obj), tags);
				case DataEntry.DataType.Int:
					return new DataEntry(dataType, new DataEntry.IntEntry((int)obj), tags);
				case DataEntry.DataType.Float:
					return new DataEntry(dataType, new DataEntry.FloatEntry((float)obj), tags);
				case DataEntry.DataType.Vector2:
					return new DataEntry(dataType, new DataEntry.Vector2Entry((Vector2)obj), tags);
				case DataEntry.DataType.Vector3:
					return new DataEntry(dataType, new DataEntry.Vector3Entry((Vector3)obj), tags);
				case DataEntry.DataType.Vector4:
					return new DataEntry(dataType, new DataEntry.Vector4Entry((Vector4)obj), tags);
				case DataEntry.DataType.Quaternion:
					return new DataEntry(dataType, new DataEntry.QuaternionEntry((Quaternion)obj), tags);
				case DataEntry.DataType.Color:
					return new DataEntry(dataType, new DataEntry.ColorEntry((Color32)obj), tags);
				case DataEntry.DataType.String:
					return new DataEntry(dataType, new DataEntry.StringEntry((string)obj), tags);
				case DataEntry.DataType.Enum:
				{
					string enumType = type.AssemblyQualifiedName;
					string enumValue = Enum.GetName(type, obj);
					return new DataEntry(dataType, new DataEntry.EnumEntry(enumType, enumValue), tags);
				}
				case DataEntry.DataType.Array:
				{
					ICollection collection = (ICollection)obj;
					DataEntry[] entries = new DataEntry[collection.Count];
					int i = 0;
					foreach (object entry in collection)
					{
						entries[i] = Parse(entry, tags);
						i++;
					}
					Type elemType = type.IsGenericType ? type.GetGenericArguments()[0] : type.GetElementType();
					DataEntry.DataType arrayType = GetDatatype(elemType);
					return new DataEntry(dataType, new DataEntry.ArrayEntry(arrayType, entries), tags);
				}
				case DataEntry.DataType.Class:
				{
					Dictionary<string, DataEntry> entries = new Dictionary<string, DataEntry>();
					foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					{
						if (field.GetCustomAttributes(typeof(SerializeField), false).Any() ||
							   field.GetCustomAttributes(typeof(JSON.JsonSerializeField), false).Any())
						{
							object val = field.GetValue(obj);
							if (val == null)
							{
								//If the field has no instance(can happen to reference types) create an instance.
								if (field.FieldType.IsArray)
								{
									val = Array.CreateInstance(field.FieldType.GetElementType(), 0);
								}
								else
								if (typeof(string).IsAssignableFrom(field.FieldType))
								{
									val = string.Empty;
								}
								else
								{
									val = FormatterServices.GetUninitializedObject(field.FieldType);
								}
							}
							if (val != null)
							{
								string[] fieldTags = field.GetCustomAttributes(typeof(DataTagAttribute), false).Select(o => (o as DataTagAttribute).Tag).ToArray();
								entries.Add(field.Name, Parse(val, fieldTags));
							}
						}
					}
					return new DataEntry(DataEntry.DataType.Class, new DataEntry.ClassEntry(entries), tags);
				}
				default:
					throw new Exception("[DataDeserializer] Unsupported type: " + dataType.ToString());
			}
		}

		public static DataEntry CreatePrimitiveEntry(DataEntry.DataType dataType, params string[] tags)
		{
			switch (dataType)
			{
				case DataEntry.DataType.Byte:
					return new DataEntry(dataType, new DataEntry.ByteEntry(0), tags);
				case DataEntry.DataType.Boolean:
					return new DataEntry(dataType, new DataEntry.BooleanEntry(false), tags);
				case DataEntry.DataType.Int:
					return new DataEntry(dataType, new DataEntry.IntEntry(0), tags);
				case DataEntry.DataType.Float:
					return new DataEntry(dataType, new DataEntry.FloatEntry(0f), tags);
				case DataEntry.DataType.Vector2:
					return new DataEntry(dataType, new DataEntry.Vector2Entry(new Vector2(0f, 0f)), tags);
				case DataEntry.DataType.Vector3:
					return new DataEntry(dataType, new DataEntry.Vector3Entry(new Vector3(0f, 0f, 0f)), tags);
				case DataEntry.DataType.Vector4:
					return new DataEntry(dataType, new DataEntry.Vector4Entry(new Vector4(0f, 0f, 0f, 0f)), tags);
				case DataEntry.DataType.Quaternion:
					return new DataEntry(dataType, new DataEntry.QuaternionEntry(Quaternion.identity), tags);
				case DataEntry.DataType.Color:
					return new DataEntry(dataType, new DataEntry.ColorEntry(new Color32(1, 1, 1, 1)), tags);
				case DataEntry.DataType.String:
					return new DataEntry(dataType, new DataEntry.StringEntry(string.Empty), tags);
				case DataEntry.DataType.Class:
					for (int i = 0; i < tags.Length; i++)
					{
						if (tags[i].StartsWith("create_"))
						{
							string typeName = tags[i].Split('_')[1];
							DataEntry d = Parse(System.Type.GetType(typeName));
							return d;
						}
					}
					throw new Exception("[DataDeserializer] Unable to create entry for this class. Try adding a [CreateTag(typeof(YourClass))] to fix this.");
				default:
					throw new Exception("[DataDeserializer] Unable to create entry for non-primitive type: " + dataType.ToString());
			}
		}
	}
}