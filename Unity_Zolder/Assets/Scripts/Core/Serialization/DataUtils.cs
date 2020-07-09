// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Serialization
{
	public static class DataUtils
	{
		private const System.Int32 VERSION = 1;

		#region Saving/Loading
		public static byte[] Save(this DataEntry data)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					writer.Write(VERSION);
					DataSerializer.SerializeEntry(writer, data);
				}
				return stream.ToArray();
			}
		}

		public static DataEntry Load(this byte[] data)
		{
			if (data == null)
			{
				LogUtil.Error(LogTags.DATA, "DataUtils", "No data to load!");
				return null;
			}
			using (MemoryStream stream = new MemoryStream(data))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					int version = reader.ReadInt32();
					if (version == VERSION)
					{
						return DataDeserializer.DeserializeEntry(reader);
					}
					else
					{
						LogUtil.Error(LogTags.DATA, "DataUtils", "Version: " + version + " not supported! Version is: " + VERSION);
						return null;
					}
				}
			}
		}
		#endregion

		#region ArrayUtils
		public static void AddToEnd(this DataEntry.ArrayEntry data, params string[] tags)
		{
			AddToEnd(data, DataParser.CreatePrimitiveEntry(data.ArrayType, tags));
		}

		public static void AddToEnd(this DataEntry.ArrayEntry data, DataEntry entry)
		{
			data.Value = data.Value.Concat(new[] { entry }).ToArray();
		}

		public static void InsertAt(this DataEntry.ArrayEntry data, DataEntry entry, int index)
		{
			// A bit clunky, if anyone thinks of a better way to do this please let me know :)
			List<DataEntry> list = new List<DataEntry>(data.Value);
			list.Insert(index, entry);
			data.Value = list.ToArray();
		}

		public static void RemoveAt(this DataEntry.ArrayEntry data, int index)
		{
			List<DataEntry> list = new List<DataEntry>(data.Value);
			list.RemoveAt(index);
			data.Value = list.ToArray();
		}
		#endregion

		#region ClassUtils
		public static bool HasField(this DataEntry.ClassEntry classEntry, string fieldName)
		{
			return classEntry.Value.ContainsKey(fieldName);
		}

		public static void AddField(this DataEntry.ClassEntry classEntry, string fieldName, DataEntry entry)
		{
			classEntry.Value.Add(fieldName, entry);
		}

		public static void RemoveField(this DataEntry.ClassEntry classEntry, string fieldName)
		{
			classEntry.Value.Remove(fieldName);
		}

		public static void Filter(this DataEntry.ClassEntry classEntry, System.Func<DataEntry, bool> entryFilter)
		{
			List<string> toRemove = new List<string>();
			foreach (KeyValuePair<string, DataEntry> kvp in classEntry.Value)
			{
				if (entryFilter(kvp.Value))
				{
					toRemove.Add(kvp.Key);
				}
			}
			foreach (string exposedField in toRemove)
			{
				classEntry.RemoveField(exposedField);
			}
		}
		#endregion

		public static IEnumerable<DataEntry> GetEntriesWithTag(this DataEntry entry, string tag)
		{
			if (entry == null)
			{
				LogUtil.Error(LogTags.DATA, "DataUtils", "Entry is null!");
				yield break;
			}
			if (entry.Tags == null)
			{
				LogUtil.Error(LogTags.DATA, "DataUtils", "tags is null!");
				yield break;
			}
			if (entry.Tags.Contains(tag))
			{
				yield return entry;
			}
			switch (entry.Type)
			{
				case DataEntry.DataType.Array:
				{
					DataEntry.ArrayEntry orgData = entry.Data as DataEntry.ArrayEntry;
					for (int i = 0; i < orgData.Value.Length; i++)
					{
						foreach (DataEntry de in GetEntriesWithTag(orgData.Value[i], tag))
						{
							yield return de;
						}
					}

					break;
				}
				case DataEntry.DataType.Class:
				{
					DataEntry.ClassEntry orgData = entry.Data as DataEntry.ClassEntry;
					foreach (KeyValuePair<string, DataEntry> kvp in orgData.Value)
					{
						foreach (DataEntry de in GetEntriesWithTag(kvp.Value, tag))
						{
							yield return de;
						}
					}

					break;
				}
			}
		}

		public static bool MatchEntryTemplate(this DataEntry data, DataEntry template)
		{
			if (data == null || template == null)
			{
				LogUtil.Error(LogTags.DATA, "DataUtils", "DataEntry is null!");
				return false;
			}
			data.Tags = template.Tags;
			switch (data.Type)
			{
				case DataEntry.DataType.Class:
					return MatchClassTemplate(data.Data as DataEntry.ClassEntry, template.Data as DataEntry.ClassEntry);

				case DataEntry.DataType.Enum:
					return MatchEnumTemplate(data.Data as DataEntry.EnumEntry, template.Data as DataEntry.EnumEntry);

				case DataEntry.DataType.Array:
					return MatchArrayTemplate(data.Data as DataEntry.ArrayEntry, template.Data as DataEntry.ArrayEntry);
			}
			return false;
		}

		private static bool MatchClassTemplate(DataEntry.ClassEntry classEntry, DataEntry.ClassEntry classTemplate)
		{
			List<string> removedFields = new List<string>();
			Dictionary<string, DataEntry> addFields = new Dictionary<string, DataEntry>();
			bool isDirty = false;

			// find fields to remove
			foreach (KeyValuePair<string, DataEntry> entry in classEntry.Value)
			{
				if (!classTemplate.HasField(entry.Key))
				{
					removedFields.Add(entry.Key);
					isDirty = true;
				}
			}

			// find fields that changed type
			foreach (KeyValuePair<string, DataEntry> entry in classEntry.Value)
			{
				if (classTemplate.HasField(entry.Key) && entry.Value.Type != classTemplate.Value[entry.Key].Type)
				{
					removedFields.Add(entry.Key);
					addFields.Add(entry.Key, classTemplate.Value[entry.Key]);
					isDirty = true;
				}
			}

			// find fields to add			
			foreach (KeyValuePair<string, DataEntry> entry in classTemplate.Value)
			{
				if (!classEntry.HasField(entry.Key))
				{
					addFields.Add(entry.Key, entry.Value);
					isDirty = true;
				}
			}

			// do actual removing
			foreach (string removedKey in removedFields)
			{
				classEntry.RemoveField(removedKey);
			}

			// do actual adding
			foreach (KeyValuePair<string, DataEntry> fieldToAdd in addFields)
			{
				classEntry.AddField(fieldToAdd.Key, fieldToAdd.Value);
			}

			//Recurse over all the fields
			foreach (KeyValuePair<string, DataEntry> entry in classEntry.Value)
			{
				isDirty = entry.Value.MatchEntryTemplate(classTemplate.Value[entry.Key]) || isDirty;
			}
			return isDirty;
		}

		private static bool MatchEnumTemplate(DataEntry.EnumEntry enumEntry, DataEntry.EnumEntry enumTemplate)
		{
			if (enumEntry.EnumType != enumTemplate.EnumType)
			{
				enumEntry.EnumType = enumTemplate.EnumType;
				enumEntry.Value = enumTemplate.Value;
				return true;
			}
			return false;
		}

		private static bool MatchArrayTemplate(DataEntry.ArrayEntry arrayEntry, DataEntry.ArrayEntry arrayTemplate)
		{
			if (arrayEntry.ArrayType != arrayTemplate.ArrayType)
			{
				arrayEntry.ArrayType = arrayTemplate.ArrayType;
				arrayEntry.Value = arrayTemplate.Value;
				return true;
			}
			bool isDirty = false;
			for (int i = 0; i < arrayEntry.Value.Length; i++)
			{
				if (arrayTemplate.Value.Length > 0)
				{
					isDirty = arrayEntry.Value[i].MatchEntryTemplate(arrayTemplate.Value[0]) || isDirty;
				}
			}
			return isDirty;
		}
	}
}
