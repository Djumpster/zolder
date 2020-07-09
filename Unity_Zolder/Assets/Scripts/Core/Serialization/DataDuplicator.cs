// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.Serialization
{
	public static class DataDuplicator
	{
		public static DataEntry Duplicate(DataEntry entry)
		{
			switch (entry.Type)
			{
				case DataEntry.DataType.Byte:
					return new DataEntry(entry.Type, new DataEntry.ByteEntry(entry.Data as DataEntry.ByteEntry), entry.Tags);
				case DataEntry.DataType.Boolean:
					return new DataEntry(entry.Type, new DataEntry.BooleanEntry(entry.Data as DataEntry.BooleanEntry), entry.Tags);
				case DataEntry.DataType.Int:
					return new DataEntry(entry.Type, new DataEntry.IntEntry(entry.Data as DataEntry.IntEntry), entry.Tags);
				case DataEntry.DataType.Float:
					return new DataEntry(entry.Type, new DataEntry.FloatEntry(entry.Data as DataEntry.FloatEntry), entry.Tags);
				case DataEntry.DataType.Vector2:
					return new DataEntry(entry.Type, new DataEntry.Vector2Entry(entry.Data as DataEntry.Vector2Entry), entry.Tags);
				case DataEntry.DataType.Vector3:
					return new DataEntry(entry.Type, new DataEntry.Vector3Entry(entry.Data as DataEntry.Vector3Entry), entry.Tags);
				case DataEntry.DataType.Vector4:
					return new DataEntry(entry.Type, new DataEntry.Vector4Entry(entry.Data as DataEntry.Vector4Entry), entry.Tags);
				case DataEntry.DataType.Quaternion:
					return new DataEntry(entry.Type, new DataEntry.QuaternionEntry(entry.Data as DataEntry.QuaternionEntry), entry.Tags);
				case DataEntry.DataType.Color:
					return new DataEntry(entry.Type, new DataEntry.ColorEntry(entry.Data as DataEntry.ColorEntry), entry.Tags);
				case DataEntry.DataType.String:
					return new DataEntry(entry.Type, new DataEntry.StringEntry(entry.Data as DataEntry.StringEntry), entry.Tags);
				case DataEntry.DataType.Enum:
					return new DataEntry(entry.Type, new DataEntry.EnumEntry(entry.Data as DataEntry.EnumEntry), entry.Tags);
				case DataEntry.DataType.Array:
				{
					DataEntry.ArrayEntry orgData = entry.Data as DataEntry.ArrayEntry;
					DataEntry[] newEntries = new DataEntry[orgData.Value.Length];
					for (int i = 0; i < newEntries.Length; i++)
					{
						newEntries[i] = Duplicate(orgData.Value[i]);
					}

					return new DataEntry(entry.Type, new DataEntry.ArrayEntry(orgData.ArrayType, newEntries), entry.Tags);
				}
				case DataEntry.DataType.Class:
				{
					DataEntry.ClassEntry orgData = entry.Data as DataEntry.ClassEntry;
					Dictionary<string, DataEntry> newEntries = new Dictionary<string, DataEntry>();
					foreach (KeyValuePair<string, DataEntry> kvp in orgData.Value)
					{
						newEntries.Add(kvp.Key, Duplicate(kvp.Value));
					}

					return new DataEntry(entry.Type, new DataEntry.ClassEntry(newEntries), entry.Tags);
				}

				default:
					throw new Exception("[DataDuplicator] Unsupported type: " + entry.Type);
			}
		}
	}
}