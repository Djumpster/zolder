// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Logging;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public static class EditorDataVisualizer
	{
		public static bool DrawEntry(DataEntry entry)
		{
			if (entry == null)
			{
				return false;
			}

			CustomDataDrawerHelper drawerUtils = GlobalDependencyLocator.Instance.Get<CustomDataDrawerHelper>();

			foreach (string tag in entry.Tags)
			{
				if (drawerUtils.HasCustomDrawer(tag) && entry.Type != DataEntry.DataType.Array)
				{
					return drawerUtils.DrawWithCustomDrawer(entry, tag);
				}
			}

			bool dirty = false;
			switch (entry.Type)
			{
				case DataEntry.DataType.Byte:
					dirty = dirty || DrawByte(entry.Data as DataEntry.ByteEntry);
					break;
				case DataEntry.DataType.Boolean:
					dirty = dirty || DrawBoolean(entry.Data as DataEntry.BooleanEntry);
					break;
				case DataEntry.DataType.Int:
					dirty = dirty || DrawInt(entry.Data as DataEntry.IntEntry);
					break;
				case DataEntry.DataType.Float:
					dirty = dirty || DrawFloat(entry.Data as DataEntry.FloatEntry);
					break;
				case DataEntry.DataType.Vector2:
					dirty = dirty || DrawVector2(entry.Data as DataEntry.Vector2Entry);
					break;
				case DataEntry.DataType.Vector3:
					dirty = dirty || DrawVector3(entry.Data as DataEntry.Vector3Entry);
					break;
				case DataEntry.DataType.Vector4:
					dirty = dirty || DrawVector4(entry.Data as DataEntry.Vector4Entry);
					break;
				case DataEntry.DataType.Quaternion:
					dirty = dirty || DrawQuaternion(entry.Data as DataEntry.QuaternionEntry);
					break;
				case DataEntry.DataType.Color:
					dirty = dirty || DrawColor(entry.Data as DataEntry.ColorEntry);
					break;
				case DataEntry.DataType.String:
					dirty = dirty || DrawString(entry.Data as DataEntry.StringEntry);
					break;
				case DataEntry.DataType.Enum:
					dirty = dirty || DrawEnum(entry.Data as DataEntry.EnumEntry);
					break;
				case DataEntry.DataType.Array:
					dirty = dirty || DrawArray(entry.Data as DataEntry.ArrayEntry, entry.Tags);
					break;
				case DataEntry.DataType.Class:
					dirty = dirty || DrawClass(entry.Data as DataEntry.ClassEntry);
					break;
				default:
					GUILayout.Label("Unsupported type: " + entry.Type.ToString());
					break;
			}
			return dirty;
		}

		public static bool DrawByte(DataEntry.ByteEntry data)
		{
			byte newVal = (byte)EditorGUILayout.IntField(data.Value);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawBoolean(DataEntry.BooleanEntry data)
		{
			bool newVal = EditorGUILayout.Toggle(data.Value);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawInt(DataEntry.IntEntry data)
		{
			int newVal = EditorGUILayout.IntField(data.Value);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawFloat(DataEntry.FloatEntry data)
		{
			float newVal = EditorGUILayout.FloatField(data.Value);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawVector2(DataEntry.Vector2Entry data)
		{
			Vector2 newVal = EditorGUILayout.Vector2Field(GUIContent.none, data.Value);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawVector3(DataEntry.Vector3Entry data)
		{
			Vector3 newVal = EditorGUILayout.Vector3Field(GUIContent.none, data.Value);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawVector4(DataEntry.Vector4Entry data)
		{
			Vector4 newVal = EditorGUILayout.Vector4Field("", data.Value);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawQuaternion(DataEntry.QuaternionEntry data)
		{
			Vector4 vec4 = new Vector4(data.Value.x, data.Value.y, data.Value.z, data.Value.w);
			vec4 = EditorGUILayout.Vector4Field("", vec4);
			Quaternion newVal = new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawColor(DataEntry.ColorEntry data)
		{
			Color32 newVal = EditorGUILayout.ColorField(GUIContent.none, data.Value);
			bool dirty = newVal.r != data.Value.r ||
							newVal.g != data.Value.g ||
							newVal.b != data.Value.b ||
							newVal.a != data.Value.a;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawString(DataEntry.StringEntry data)
		{
			string newVal = EditorGUILayout.TextField(data.Value);
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawEnum(DataEntry.EnumEntry data)
		{
			Type type = Type.GetType(data.EnumType);

			string[] options = Enum.GetNames(type);
			string current = data.Value;
			if (string.IsNullOrEmpty(current) || !options.Contains(current))
			{
				current = options.First();
			}

			int currentIndex = Array.FindIndex(options, o => o == current);
			string newVal = options[EditorGUILayout.Popup(currentIndex, options)];
			bool dirty = newVal != data.Value;
			data.Value = newVal;
			return dirty;
		}

		public static bool DrawArray(DataEntry.ArrayEntry data, params string[] tags)
		{
			bool dirty = false;

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("+", GUILayout.Width(22f)))
			{
				try
				{
					data.AddToEnd(tags);
					dirty = true;
				}
				catch (Exception e) { LogUtil.Error(LogTags.DATA, "EditorDataVisualizer", "Unable to create new entry: " + e.Message); }
			}
			GUI.backgroundColor = Color.white;

			GUILayout.BeginVertical();
			int count = 0;
			foreach (DataEntry entry in data.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label(count.ToString() + ":");

				GUI.SetNextControlName(data.GUID);
				dirty = dirty || DrawEntry(entry);

				if (GUI.GetNameOfFocusedControl() == data.GUID)
				{
					if (GUILayout.Button("d", GUILayout.Width(20f))) //Duplicates a entry and inserts it after this one.
					{
						data.InsertAt(DataDuplicator.Duplicate(entry), count);
						dirty = true;
					}
					if (GUILayout.Button("x", GUILayout.Width(20f)))
					{
						data.RemoveAt(count);
						dirty = true;
					}
				}
				GUILayout.EndHorizontal();
				count++;
			}

			GUILayout.EndVertical();
			return dirty;
		}

		public static bool DrawClass(DataEntry.ClassEntry entry)
		{
			bool dirty = false;
			GUILayout.BeginVertical();
			foreach (KeyValuePair<string, DataEntry> kvp in entry.Value)
			{
				if (kvp.Value.Tags.Contains("hidden"))
				{
					continue;
				}

				GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label(kvp.Key + ":", GUILayout.Width(60f));
				dirty = dirty || DrawEntry(kvp.Value);
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			return dirty;
		}
	}
}
#endif