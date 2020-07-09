// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public class DataEntry
	{
		public enum DataType : byte
		{
			Boolean = 0,
			Int = 10,
			Float = 20,
			Vector2 = 30,
			Vector3 = 40,
			Color = 50,
			String = 60,
			Enum = 70,
			Array = 80,
			Class = 90,
			Byte = 100,
			Quaternion = 110,
			Vector4 = 120
		}

		public readonly DataType Type;
		public readonly object Data;
		public string[] Tags { get; set; }

		public DataEntry(DataType type, object data, params string[] tags)
		{
			Type = type;
			Data = data;
			Tags = tags;
		}

		public class ByteEntry
		{
			public byte Value { get; set; }

			public ByteEntry(byte value)
			{
				Value = value;
			}

			public ByteEntry(ByteEntry entry)
			{
				Value = entry.Value;
			}
		}

		public class BooleanEntry
		{
			public bool Value { get; set; }

			public BooleanEntry(bool value)
			{
				Value = value;
			}

			public BooleanEntry(BooleanEntry entry)
			{
				Value = entry.Value;
			}
		}

		public class IntEntry
		{
			public int Value { get; set; }

			public IntEntry(int value)
			{
				Value = value;
			}

			public IntEntry(IntEntry entry)
			{
				Value = entry.Value;
			}
		}

		public class FloatEntry
		{
			public float Value { get; set; }

			public FloatEntry(float value)
			{
				Value = value;
			}

			public FloatEntry(FloatEntry entry)
			{
				Value = entry.Value;
			}
		}

		public class Vector2Entry
		{
			public Vector2 Value { get; set; }

			public Vector2Entry(Vector2 value)
			{
				Value = value;
			}

			public Vector2Entry(Vector2Entry entry)
			{
				Value = entry.Value;
			}
		}

		public class Vector3Entry
		{
			public Vector3 Value { get; set; }

			public Vector3Entry(Vector3 value)
			{
				Value = value;
			}

			public Vector3Entry(Vector3Entry entry)
			{
				Value = entry.Value;
			}
		}

		public class Vector4Entry
		{
			public Vector4 Value { get; set; }

			public Vector4Entry(Vector4 value)
			{
				Value = value;
			}

			public Vector4Entry(Vector4Entry entry)
			{
				Value = entry.Value;
			}
		}

		public class QuaternionEntry
		{
			public Quaternion Value { get; set; }

			public QuaternionEntry(Quaternion value)
			{
				Value = value;
			}

			public QuaternionEntry(QuaternionEntry entry)
			{
				Value = entry.Value;
			}
		}

		public class ColorEntry
		{
			public Color32 Value { get; set; }

			public ColorEntry(Color32 value)
			{
				Value = value;
			}

			public ColorEntry(ColorEntry entry)
			{
				Value = entry.Value;
			}
		}

		public class StringEntry
		{
			public string Value { get; set; }

			public StringEntry(string value)
			{
				Value = value;
			}

			public StringEntry(StringEntry entry)
			{
				Value = entry.Value;
			}
		}

		public class EnumEntry
		{
			public string EnumType { get; set; }
			public string Value { get; set; }

			public EnumEntry(string enumType, string value)
			{
				EnumType = enumType;
				Value = value;
			}

			public EnumEntry(EnumEntry entry)
			{
				EnumType = entry.EnumType;
				Value = entry.Value;
			}
		}

		public class ArrayEntry
		{
			public DataType ArrayType { get; set; }
			public DataEntry[] Value { get; set; }
			public readonly string GUID;

			public ArrayEntry(DataType type, DataEntry[] value)
			{
				ArrayType = type;
				Value = value;
				GUID = System.Guid.NewGuid().ToString();
			}
		}

		public class ClassEntry
		{
			public readonly Dictionary<string, DataEntry> Value;

			public ClassEntry()
			{
				Value = new Dictionary<string, DataEntry>();
			}

			public ClassEntry(Dictionary<string, DataEntry> value)
			{
				Value = value;
			}
		}
	}
}