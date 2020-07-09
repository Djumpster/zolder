// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public static class DataDeserializer
	{
		public static DataEntry DeserializeEntry(BinaryReader reader)
		{
			//Read the type
			DataEntry.DataType type = (DataEntry.DataType)reader.ReadByte();

			//Read the tags
			int numTags = reader.ReadInt32();
			string[] tags = new string[numTags];
			for (int i = 0; i < numTags; i++)
			{
				tags[i] = reader.ReadString();
			}

			//Read the data
			switch (type)
			{
				case DataEntry.DataType.Byte:
					return new DataEntry(type, DeserializeByte(reader), tags);
				case DataEntry.DataType.Boolean:
					return new DataEntry(type, DeserializeBoolean(reader), tags);
				case DataEntry.DataType.Int:
					return new DataEntry(type, DeserializeInt(reader), tags);
				case DataEntry.DataType.Float:
					return new DataEntry(type, DeserializeFloat(reader), tags);
				case DataEntry.DataType.Vector2:
					return new DataEntry(type, DeserializeVector2(reader), tags);
				case DataEntry.DataType.Vector3:
					return new DataEntry(type, DeserializeVector3(reader), tags);
				case DataEntry.DataType.Vector4:
					return new DataEntry(type, DeserializeVector4(reader), tags);
				case DataEntry.DataType.Quaternion:
					return new DataEntry(type, DeserializeQuaternion(reader), tags);
				case DataEntry.DataType.Color:
					return new DataEntry(type, DeserializeColor(reader), tags);
				case DataEntry.DataType.String:
					return new DataEntry(type, DeserializeString(reader), tags);
				case DataEntry.DataType.Enum:
					return new DataEntry(type, DeserializeEnum(reader), tags);
				case DataEntry.DataType.Array:
					return new DataEntry(type, DeserializeArray(reader), tags);
				case DataEntry.DataType.Class:
					return new DataEntry(type, DeserializeClass(reader), tags);
				default:
					throw new Exception("[DataDeserializer] Unsupported type: " + type.ToString());
			}
		}

		private static DataEntry.ByteEntry DeserializeByte(BinaryReader reader)
		{
			byte val = reader.ReadByte();
			return new DataEntry.ByteEntry(val);
		}

		private static DataEntry.BooleanEntry DeserializeBoolean(BinaryReader reader)
		{
			bool val = reader.ReadBoolean();
			return new DataEntry.BooleanEntry(val);
		}

		private static DataEntry.IntEntry DeserializeInt(BinaryReader reader)
		{
			int val = reader.ReadInt32();
			return new DataEntry.IntEntry(val);
		}

		private static DataEntry.FloatEntry DeserializeFloat(BinaryReader reader)
		{
			float val = reader.ReadSingle();
			return new DataEntry.FloatEntry(val);
		}

		private static DataEntry.Vector2Entry DeserializeVector2(BinaryReader reader)
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			return new DataEntry.Vector2Entry(new Vector2(x, y));
		}

		private static DataEntry.Vector3Entry DeserializeVector3(BinaryReader reader)
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			float z = reader.ReadSingle();
			return new DataEntry.Vector3Entry(new Vector3(x, y, z));
		}

		private static DataEntry.Vector4Entry DeserializeVector4(BinaryReader reader)
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			float z = reader.ReadSingle();
			float w = reader.ReadSingle();
			return new DataEntry.Vector4Entry(new Vector4(x, y, z, w));
		}

		private static DataEntry.QuaternionEntry DeserializeQuaternion(BinaryReader reader)
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			float z = reader.ReadSingle();
			float w = reader.ReadSingle();
			return new DataEntry.QuaternionEntry(new Quaternion(x, y, z, w));
		}

		private static DataEntry.ColorEntry DeserializeColor(BinaryReader reader)
		{
			byte r = reader.ReadByte();
			byte g = reader.ReadByte();
			byte b = reader.ReadByte();
			byte a = reader.ReadByte();
			return new DataEntry.ColorEntry(new Color32(r, g, b, a));
		}

		private static DataEntry.StringEntry DeserializeString(BinaryReader reader)
		{
			string val = reader.ReadString();
			return new DataEntry.StringEntry(val);
		}

		private static DataEntry.EnumEntry DeserializeEnum(BinaryReader reader)
		{
			string enumType = reader.ReadString();
			string enumValue = reader.ReadString();
			return new DataEntry.EnumEntry(enumType, enumValue);
		}

		private static DataEntry.ArrayEntry DeserializeArray(BinaryReader reader)
		{
			DataEntry.DataType type = (DataEntry.DataType)reader.ReadByte();
			int numItems = reader.ReadInt32();
			DataEntry[] entries = new DataEntry[numItems];
			for (int i = 0; i < numItems; i++)
			{
				entries[i] = DeserializeEntry(reader);
			}
			return new DataEntry.ArrayEntry(type, entries);
		}

		private static DataEntry.ClassEntry DeserializeClass(BinaryReader reader)
		{
			int numItems = reader.ReadInt32();
			Dictionary<string, DataEntry> entries = new Dictionary<string, DataEntry>();
			for (int i = 0; i < numItems; i++)
			{
				string name = reader.ReadString();
				DataEntry data = DeserializeEntry(reader);
				entries.Add(name, data);
			}
			return new DataEntry.ClassEntry(entries);
		}
	}
}