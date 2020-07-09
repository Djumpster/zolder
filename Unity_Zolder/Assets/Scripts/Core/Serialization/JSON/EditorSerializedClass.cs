// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Talespin.Core.Foundation.AssetHandling;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Reflection;
using Talespin.Core.Foundation.Serialization.Drawer;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	/// <summary>
	/// <para>
	/// The base serialized class. This class stores data
	/// for a specific instance of type T, allowing instantation
	/// during runtime.
	/// </para>
	/// 
	/// <para>
	/// Additionally this editor-only variant also adds support for drawing inspectors
	/// for the data, allowing for UI based modifications.
	/// </para>
	/// </summary>
	/// <seealso cref="RuntimeSerializedClass{T}"/>
	[Serializable]
	public class EditorSerializedClass<T> : RuntimeSerializedClass<T>, IEditorSerializedClass, ISerializedRuntimeGeneratedDataHandler where T : class
	{
		private class Drawer
		{
			public Type TargetType { get; }
			public Type TargetAttributeType { get; }

			private readonly ISerializedClassDrawer drawer;

			public Drawer(Type targetType, Type targetAttributeType, ISerializedClassDrawer drawer)
			{
				TargetType = targetType;
				TargetAttributeType = targetAttributeType;

				this.drawer = drawer;
			}

			public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
			{
				return drawer.Draw(label, value, type, attribute);
			}
		}

		private static readonly Dictionary<Type, List<Drawer>> drawers;
		private static readonly Drawer fallbackDrawer;

		static EditorSerializedClass()
		{
			IEnumerable<Type> types = Reflect.AllTypesWithAttribute<SerializedClassDrawerAttribute>();
			drawers = new Dictionary<Type, List<Drawer>>();

			foreach (Type type in types)
			{
				if (!typeof(ISerializedClassDrawer).IsAssignableFrom(type))
				{
					LogUtil.Error(LogTags.SYSTEM, "EditorSerializedClass", "SerializedClassDrawerAttribte found on a type that does not implement ISerializedClassDrawer: " + type);
					continue;
				}

				SerializedClassDrawerAttribute attribute = type.GetAttribute<SerializedClassDrawerAttribute>();//

				if (attribute.TargetType == null)
				{
					LogUtil.Error(LogTags.SYSTEM, "EditorSerializedClass", "Drawer does not have a target type: " + type);
					continue;
				}

				if (!drawers.ContainsKey(attribute.TargetType))
				{
					drawers.Add(attribute.TargetType, new List<Drawer>());
				}

				var instance = Activator.CreateInstance(type) as ISerializedClassDrawer;
				drawers[attribute.TargetType].Add(new Drawer(attribute.TargetType, attribute.TargetAttributeType, instance));
			}

			fallbackDrawer = new Drawer(null, null, new FallbackDrawer());
		}

		public event Action<EditorSerializedClass<T>> OnChanged = delegate { };

		private object currentData;

		public void Reset(object o)
		{
			if (o is T)
			{
				currentData = null;
				Assign((T)o);
			}
			else
			{
				LogUtil.Error(LogTags.DATA, this, "Could not assign data: " + o);
			}
		}

		public void AssignData(object o)
		{
			if (o is T)
			{
				Assign((T)o);
			}
			else
			{
				LogUtil.Error(LogTags.DATA, this, "Could not assign data: " + o);
			}
		}

		public void Assign(T obj)
		{
			data = JsonUtility.ToJson(obj);
			GuidDatabaseManager guidDatabaseManager = GuidDatabaseManager.Instance;

			Type t = obj.GetType();
			type = guidDatabaseManager.MapTypeToGuid(t);
			name = t.FullName;
		}

		public object InstantiateObject()
		{
			if (DataType == null)
			{
				return null;
			}
			else
			{
				return (T)JsonUtility.FromJson(data, DataType);
			}
		}

		public IEditorSerializedClass Clone()
		{
			EditorSerializedClass<T> clone = (EditorSerializedClass<T>)FormatterServices.GetUninitializedObject(GetType());
			clone.foldouts = new Dictionary<object, bool>();
			clone.data = data;
			clone.type = type;
			clone.name = name;
			clone.identifier = Guid.NewGuid().ToString();
			clone.enabled = enabled;
			clone.OnChanged = delegate { };
			return clone;
		}

		public IEditorSerializedClass GetOverrideClone()
		{
			EditorSerializedClass<T> clone = (EditorSerializedClass<T>)FormatterServices.GetUninitializedObject(GetType());
			clone.foldouts = new Dictionary<object, bool>();
			clone.data = data;
			clone.type = type;
			clone.name = name;
			clone.identifier = identifier;
			clone.enabled = enabled;
			clone.OnChanged = delegate { };
			return clone;
		}

		public bool DisplayDataGUI()
		{
			if (DataType == null)
			{
				return false;
			}

			currentData = currentData ?? (JsonUtility.FromJson(data, DataType) as T);

			if (currentData == null)
			{
				Color color = GUI.contentColor;
				GUI.contentColor = Color.red;
				EditorGUILayout.LabelField(EditorGUIUtility.TrTextContent("Behaviour is invalid, it will be ignored. Consider removing it."));
				GUI.contentColor = color;

				return false;
			}

			EditorGUI.BeginChangeCheck();

			currentData = Parse(null, currentData, DataType);

			if (EditorGUI.EndChangeCheck())
			{
				data = JsonUtility.ToJson(currentData);
				OnChanged(this);
				return true;
			}

			return false;
		}

		private static Drawer FindDrawerFor(Type type, FieldInfo fieldInfo, out PropertyAttribute attribute)
		{
			Type checkType = type;

			while (checkType != null)
			{
				if (drawers.ContainsKey(checkType))
				{
					Drawer noAttributeDrawer = null;

					foreach (Drawer drawer in drawers[checkType])
					{
						if (drawer.TargetAttributeType != null)
						{
							if (fieldInfo != null && fieldInfo.IsDefined(drawer.TargetAttributeType, true))
							{
								attribute = fieldInfo.GetCustomAttribute(drawer.TargetAttributeType, true) as PropertyAttribute;
								return drawer;
							}
						}
						else
						{
							noAttributeDrawer = drawer;
						}
					}

					if (noAttributeDrawer != null)
					{
						attribute = null;
						return noAttributeDrawer;
					}
				}

				checkType = checkType.BaseType;
			}

			attribute = null;
			return null;
		}

		private object Parse(string name, object value, Type type, FieldInfo fieldInfo = null, string controlName = null, int controlIndex = 0)
		{
			GUIContent label = new GUIContent();

			if (!string.IsNullOrEmpty(name))
			{
				label.text = SerializedClassHelper.PrettifyString(name, true);
			}
			else
			{
				label.text = SerializedClassHelper.PrettifyTypeName(type, true);
			}

			if (fieldInfo != null)
			{
				HeaderAttribute header = fieldInfo.GetAttribute<HeaderAttribute>();
				if (header != null)
				{
					if (controlIndex > 0)
					{
						EditorGUILayout.Space();
					}

					EditorGUILayout.LabelField(header.header, EditorStyles.boldLabel);
				}

				TooltipAttribute tooltip = fieldInfo.GetAttribute<TooltipAttribute>();
				if (tooltip != null)
				{
					label.tooltip = tooltip.tooltip;
				}
			}

			if (!string.IsNullOrEmpty(controlName))
			{
				GUI.SetNextControlName(controlName);
			}

			if (typeof(IList).IsAssignableFrom(type))
			{
				if (value == null)
				{
					if (!type.IsArray)
					{
						value = FormatterServices.GetUninitializedObject(type);
						ConstructorInfo constructor = type.GetConstructor(new Type[0]);
						constructor.Invoke(value, new object[0]);
					}
					else
					{
						value = Array.CreateInstance(type.GetElementType(), 0);
					}
				}

				IList l = (IList)value;
				value = ParseList(label, l, type, fieldInfo);
			}
			else
			{
				PropertyAttribute attribute;
				Drawer drawer = FindDrawerFor(type, fieldInfo, out attribute);

				if (drawer != null)
				{
					EditorGUILayout.BeginHorizontal();
					value = drawer.Draw(label, value, type, attribute);
					EditorGUILayout.EndHorizontal();
				}
				else
				{
					if (type.HasAttribute<SerializableAttribute>() || type.IsAssignableFrom(DataType))
					{
						if (EditorGUI.indentLevel > 1 && value != null)
						{
							if (!foldouts.ContainsKey(value))
							{
								foldouts.Add(value, false);
							}

							foldouts[value] = EditorGUILayout.Foldout(foldouts[value], label);
							if (foldouts[value])
							{
								ParseSerializable(label, value, type, fieldInfo);
							}
						}
						else
						{
							ParseSerializable(label, value, type, fieldInfo);
						}
					}
					else
					{
						EditorGUILayout.BeginHorizontal();
						value = fallbackDrawer.Draw(label, value, type, attribute);
						EditorGUILayout.EndHorizontal();
					}
				}
			}

			return value;
		}

		#region List
		private Dictionary<object, bool> foldouts = new Dictionary<object, bool>();
		private object ParseList(GUIContent label, IList list, Type type, FieldInfo fieldInfo)
		{
			if (!foldouts.ContainsKey(list))
			{
				foldouts.Add(list, false);
			}

			foldouts[list] = EditorGUILayout.Foldout(foldouts[list], label);
			if (!foldouts[list])
			{
				return list;
			}

			Type innerType;
			EditorGUI.indentLevel++;

			innerType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];

			EditorGUILayout.BeginHorizontal();
			int arraySize = EditorGUILayout.DelayedIntField("Size", list.Count);

			if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(35)))
			{
				arraySize++;
			}

			bool wasEnabled = GUI.enabled;
			GUI.enabled &= arraySize > 0;
			if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.MaxWidth(35)))
			{
				arraySize--;
			}
			GUI.enabled = wasEnabled;

			arraySize = Mathf.Max(arraySize, 0);

			EditorGUILayout.EndHorizontal();

			if (arraySize != list.Count)
			{
				if (type.IsArray)
				{
					foldouts.Remove(list);

					Array temp = Array.CreateInstance(innerType, arraySize);
					Array.ConstrainedCopy((Array)list, 0, temp, 0, Mathf.Min(arraySize, list.Count));
					list = temp;

					if (!typeof(UnityEngine.Object).IsAssignableFrom(innerType))
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i] == null)
							{
								if (typeof(string).IsAssignableFrom(innerType))
								{
									list[i] = "";
								}
								else
								{
									list[i] = FormatterServices.GetUninitializedObject(innerType);
								}
							}
						}
					}

					foldouts.Add(list, true);
				}
				else
				{
					while (list.Count < arraySize)
					{
						if (typeof(UnityEngine.Object).IsAssignableFrom(innerType))
						{
							list.Add(null);
						}
						else
						{
							if (typeof(string).IsAssignableFrom(innerType))
							{
								list.Add("");
							}
							else
							{
								list.Add(FormatterServices.GetUninitializedObject(innerType));
							}
						}
					}

					while (list.Count > arraySize)
					{
						list.RemoveAt(list.Count - 1);
					}
				}
			}

			for (int i = 0; i < list.Count; i++)
			{
				object obj = list[i];
				string cname = (obj == null) ? "null" : (list.GetHashCode() + i).ToString();

				list[i] = Parse(i.ToString(), obj, innerType, fieldInfo, cname);

				if (Event.current.type == EventType.ContextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				{
					if (!type.IsArray)
					{
						bool dirty = ShowListItemContextMenu(list, i, obj);

						if (dirty)
						{
							break;
						}
					}
				}
			}

			for (int i = 0; i < list.Count; i++)
			{
				string cname = list[i] == null ? "null" : (list.GetHashCode() + i).ToString();

				if (GUI.GetNameOfFocusedControl() == cname)
				{
					if (GUITools.IsCommand(GUITools.Command.Delete) || GUITools.IsCommand(GUITools.Command.SoftDelete))
					{
						list.RemoveAt(i);
						GUI.changed = true;
						break;
					}

					if (GUITools.IsCommand(GUITools.Command.Duplicate))
					{
						list.Insert(i, list[i]);
						GUI.changed = true;
						break;
					}
				}
			}

			EditorGUI.indentLevel--;
			return list;
		}

		private bool ShowListItemContextMenu(IList l, int i, object obj)
		{
			bool dirty = false;
			GenericMenu menu = new GenericMenu();
			GenericMenu.MenuFunction deleteFunc = delegate ()
			{
				l.RemoveAt(i);
				dirty = true;
				GUI.changed = true;
			};
			menu.AddItem(new GUIContent("Delete"), false, deleteFunc);

			GenericMenu.MenuFunction duplicateFunc = delegate ()
			{
				// this will work if the passed objects are always value types, which for now they are.
				l.Insert(i, obj);
				dirty = true;
				GUI.changed = true;
			};
			menu.AddItem(new GUIContent("Duplicate"), false, duplicateFunc);

			GenericMenu.MenuFunction moveUpFunc = delegate ()
			{
				l[i] = l[i - 1];
				l[i - 1] = obj;
				dirty = true;
				GUI.changed = true;
			};

			if (i == 0)
			{
				menu.AddDisabledItem(new GUIContent("Move Up"));
			}
			else
			{
				menu.AddItem(new GUIContent("Move Up"), false, moveUpFunc);
			}

			GenericMenu.MenuFunction moveDownFunc = delegate ()
			{
				l[i] = l[i + 1];
				l[i + 1] = obj;

				dirty = true;
				GUI.changed = true;
			};

			if (i == l.Count - 1)
			{
				menu.AddDisabledItem(new GUIContent("Move Down"));
			}
			else
			{
				menu.AddItem(new GUIContent("Move Down"), false, moveDownFunc);
			}

			menu.ShowAsContext();
			return dirty;
		}
		#endregion

		#region Serializable
		private void ParseSerializable(GUIContent label, object value, Type type, FieldInfo fieldInfo)
		{
			if (value == null)
			{
				value = (FormatterServices.GetUninitializedObject(type));
				ConstructorInfo constructor = type.GetConstructor(new Type[0]);
				constructor.Invoke(value, new object[0]);
			}

			List<FieldInfo> fields = new List<FieldInfo>();
			List<FieldInfo> tempFields = new List<FieldInfo>();

			Type currentType = type;

			while (currentType != null)
			{
				tempFields.Clear();

				foreach (FieldInfo newField in currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					bool found = false;

					foreach (FieldInfo fieldInList in fields)
					{
						if (fieldInList.Name == newField.Name)
						{
							found = true;
						}
					}

					if (!found)
					{
						tempFields.Add(newField);
					}
				}

				fields.InsertRange(0, tempFields);
				currentType = currentType.BaseType;
			}

			EditorGUI.indentLevel++;
			int controlIndex = 0;

			for (int i = 0; i < fields.Count; i++)
			{
				FieldInfo f = fields[i];

				if (f.HasAttribute<SerializeField>() || f.IsPublic)
				{
					object inner = f.GetValue(value);
					f.SetValue(value, Parse(f.Name, inner, f.FieldType, f, null, controlIndex));
					controlIndex++;
				}
			}

			EditorGUI.indentLevel--;
		}
		#endregion
	}
}
#endif
