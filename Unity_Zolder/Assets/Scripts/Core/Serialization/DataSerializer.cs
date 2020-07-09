// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Serialization
{
	public static class DataSerializer
	{
		public static void SerializeEntry(BinaryWriter writer, DataEntry entry)
		{
			if (entry == null)
			{
				LogUtil.Error(LogTags.DATA, "DataSerializer", "entry is null!!");
				return;
			}
			writer.Write((byte)entry.Type); //Write datatype.

			//Write tags
			writer.Write(entry.Tags.Length);
			foreach (string tag in entry.Tags)
			{
				writer.Write(tag);
			}

			switch (entry.Type)
			{
				case DataEntry.DataType.Byte:
					SerializeByte(writer, entry.Data as DataEntry.ByteEntry);
					break;
				case DataEntry.DataType.Boolean:
					SerializeBoolean(writer, entry.Data as DataEntry.BooleanEntry);
					break;
				case DataEntry.DataType.Int:
					SerializeInt(writer, entry.Data as DataEntry.IntEntry);
					break;
				case DataEntry.DataType.Float:
					SerializeFloat(writer, entry.Data as DataEntry.FloatEntry);
					break;
				case DataEntry.DataType.Vector2:
					SerializeVector2(writer, entry.Data as DataEntry.Vector2Entry);
					break;
				case DataEntry.DataType.Vector3:
					SerializeVector3(writer, entry.Data as DataEntry.Vector3Entry);
					break;
				case DataEntry.DataType.Vector4:
					SerializeVector4(writer, entry.Data as DataEntry.Vector4Entry);
					break;
				case DataEntry.DataType.Quaternion:
					SerializeQuaternion(writer, entry.Data as DataEntry.QuaternionEntry);
					break;
				case DataEntry.DataType.Color:
					SerializeColor(writer, entry.Data as DataEntry.ColorEntry);
					break;
				case DataEntry.DataType.String:
					SerializeString(writer, entry.Data as DataEntry.StringEntry);
					break;
				case DataEntry.DataType.Enum:
					SerializeEnum(writer, entry.Data as DataEntry.EnumEntry);
					break;
				case DataEntry.DataType.Array:
					SerializeArray(writer, entry.Data as DataEntry.ArrayEntry);
					break;
				case DataEntry.DataType.Class:
					SerializeClass(writer, entry.Data as DataEntry.ClassEntry);
					break;
				default:
					throw new Exception("[DataSerializer] Unsupported type: " + entry.Type.ToString());
			}
		}

		private static void SerializeByte(BinaryWriter writer, DataEntry.ByteEntry data)
		{
			writer.Write(data.Value);
		}

		private static void SerializeBoolean(BinaryWriter writer, DataEntry.BooleanEntry data)
		{
			writer.Write(data.Value);
		}

		private static void SerializeInt(BinaryWriter writer, DataEntry.IntEntry data)
		{
			writer.Write(data.Value);
		}

		private static void SerializeFloat(BinaryWriter writer, DataEntry.FloatEntry data)
		{
			writer.Write(data.Value);
		}

		private static void SerializeVector2(BinaryWriter writer, DataEntry.Vector2Entry data)
		{
			writer.Write(data.Value.x);
			writer.Write(data.Value.y);
		}

		private static void SerializeVector3(BinaryWriter writer, DataEntry.Vector3Entry data)
		{
			writer.Write(data.Value.x);
			writer.Write(data.Value.y);
			writer.Write(data.Value.z);
		}

		private static void SerializeVector4(BinaryWriter writer, DataEntry.Vector4Entry data)
		{
			writer.Write(data.Value.x);
			writer.Write(data.Value.y);
			writer.Write(data.Value.z);
			writer.Write(data.Value.w);
		}

		private static void SerializeQuaternion(BinaryWriter writer, DataEntry.QuaternionEntry data)
		{
			writer.Write(data.Value.x);
			writer.Write(data.Value.y);
			writer.Write(data.Value.z);
			writer.Write(data.Value.w);
		}

		private static void SerializeColor(BinaryWriter writer, DataEntry.ColorEntry data)
		{
			writer.Write(data.Value.r);
			writer.Write(data.Value.g);
			writer.Write(data.Value.b);
			writer.Write(data.Value.a);
		}

		private static void SerializeString(BinaryWriter writer, DataEntry.StringEntry data)
		{
			writer.Write(data.Value);
		}

		private static void SerializeEnum(BinaryWriter writer, DataEntry.EnumEntry data)
		{
			writer.Write(data.EnumType);
			writer.Write(data.Value);
		}

		private static void SerializeArray(BinaryWriter writer, DataEntry.ArrayEntry data)
		{
			writer.Write((byte)data.ArrayType); //Write the array type.
			writer.Write(data.Value.Length); //Write number of items
			foreach (DataEntry entry in data.Value)
			{
				SerializeEntry(writer, entry); //Write the data of the entry
			}
		}

		private static void SerializeClass(BinaryWriter writer, DataEntry.ClassEntry data)
		{
			writer.Write(data.Value.Count); //Write number of items
			foreach (KeyValuePair<string, DataEntry> kvp in data.Value)
			{
				writer.Write(kvp.Key); //Write the name of the field
				SerializeEntry(writer, kvp.Value); //Write the data of the field
			}
		}
	}
}