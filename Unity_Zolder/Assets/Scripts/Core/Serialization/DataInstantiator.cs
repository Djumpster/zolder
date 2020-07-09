// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public static class DataInstantiator
	{
		public static T Instantiate<T>(DataEntry entry)
		{
			return (T)Instantiate(typeof(T), entry);
		}

		public static object Instantiate(Type type, DataEntry entry)
		{
			if (typeof(byte).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.ByteEntry).Value;
			}
			else
			if (typeof(bool).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.BooleanEntry).Value;
			}
			else
			if (typeof(int).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.IntEntry).Value;
			}
			else
			if (typeof(float).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.FloatEntry).Value;
			}
			else
			if (typeof(Vector2).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.Vector2Entry).Value;
			}
			else
			if (typeof(Vector3).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.Vector3Entry).Value;
			}
			else
			if (typeof(Vector4).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.Vector4Entry).Value;
			}
			else
			if (typeof(Quaternion).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.QuaternionEntry).Value;
			}
			else
			if (typeof(Color32).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.ColorEntry).Value;
			}
			else
			if (typeof(string).IsAssignableFrom(type))
			{
				return (entry.Data as DataEntry.StringEntry).Value;
			}
			else
			if (type.IsEnum)
			{
				DataEntry.EnumEntry data = entry.Data as DataEntry.EnumEntry;
				Type t = Type.GetType(data.EnumType);
				if (t == null)
				{
					LogUtil.Error(LogTags.SYSTEM, "DataInstantiator", "Type does not exist: " + data.EnumType);
					return 0;
				}
				return Enum.Parse(t, data.Value);
			}
			else
			if (type.IsArray)
			{
				DataEntry.ArrayEntry data = entry.Data as DataEntry.ArrayEntry;
				Type elementType = type.GetElementType();

				Array array = Array.CreateInstance(elementType, data.Value.Length);
				for (int i = 0; i < data.Value.Length; i++)
				{
					array.SetValue(Instantiate(elementType, data.Value[i]), i);
				}

				return array;
			}
			else
			if (typeof(IList).IsAssignableFrom(type))
			{
				DataEntry.ArrayEntry data = entry.Data as DataEntry.ArrayEntry;
				Type elementType = type.GetGenericArguments()[0];
				Type listType = typeof(List<>).MakeGenericType(new[] { elementType });
				IList list = (IList)Activator.CreateInstance(listType);
				for (int i = 0; i < data.Value.Length; i++)
				{
					list.Add(Instantiate(elementType, data.Value[i]));
				}

				return list;
			}
			else
			if (type.IsClass || type.IsValueType)
			{
				object instance = FormatterServices.GetUninitializedObject(type);
				FillClass(instance, entry.Data as DataEntry.ClassEntry);
				return instance;
			}
			else
			{
				throw new Exception("[DataInstantiator] Unsupported type: " + type.FullName);
			}
		}

		public static void FillClass(object instance, DataEntry.ClassEntry entry)
		{
			Type type = instance.GetType();
			foreach (KeyValuePair<string, DataEntry> kvp in entry.Value)
			{
				FieldInfo field = type.GetField(kvp.Key, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if (field != null)
				{
					field.SetValue(instance, Instantiate(field.FieldType, kvp.Value));
				}
			}
		}
	}
}