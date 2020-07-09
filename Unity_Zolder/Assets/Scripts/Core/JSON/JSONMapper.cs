// Copyright 2018 Talespin, LLC. All Rights Reserved.

//When STRICT_CASTING is defined we get some more error reporting when trying to parse a JSON.
//Specifically it complains when a field in the target C# type exists, but cannot be found in the JSON. 
//#define STRICT_CASTING

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Talespin.Core.Foundation.Extensions;
/*
* This maps your JSON string to an object, and vice versa. You should not know anything about JSON, and not have to do anything special.
* Everything 'just works'. "ToJson" converts the object to a string, "ToObject<T>" converts your string to an object. 
* 
* Please stop using Hashtables and Arraylist, those are lame and nobody knows whats in them and how to use them. 
*
* To serialize any object to a string:
* JSON.ToJson(anObject);
* 
* To deserialize a JSON string to an object:
* JSON.ToObject<T>(T anObject);
*
* Fields are serialized when:
* 	- They are public OR
*  - They have [SerializeField] OR
*  - They have [JsonSerializeField]
* 
* The name used when exporting or importing either the name of the field, or the name specified by the custon JsonName attribute. This 
* Exporting fails epicly when you have a circular reference, 
* 
* You can have callbacks for custom importers and exporters. So whenever your type is being serialized the function is called, and whatever you return is used.
* The JsonIgnoreCustomImporter/JsonIgnoreCustomExporter can be applied to fields or classes(!). If it is supplied the custom function will not be called. 
* 
* Importing means from the JSON string to Object. 
* This function accepts the value from json and converts it to a Person.
* 
* JSONMapper.RegisterImporter( (string jsonValue, Person targetValue) => {
* 	return Person.GetById(jsonValue);
* });
* 
* Exporting means from the Object to JSON string. The result of an exporter is decoded as well, so dont register importers for base types.
* This function converts a Person to a string by exposing its ID. 
* 
* JSONMapper.RegisterExporter( (Person sourceValue, string targetValue) => {
*  return sourceValue.Id;
* });
* 
*/

//TODO: Cache reflection
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.MemoryScramble;
using Talespin.Core.Foundation.Misc;
using UnityEngine;

namespace Talespin.Core.Foundation.Parsing
{
	public partial class JSON
	{
		// This key is used for printing an object's actual type in the JSON.
		// This is necessary for the JSONMapper to deserialize to a derived class (otherwise it has no way of knowing the actual type). 
		private const string TYPE_KEY = "[actualtype]";

		//Converts anything to a json string
		public static string ToJson(object obj, bool format = false)
		{
			StringBuilder sb = new StringBuilder();
			ToJson(obj.GetType(), obj, sb);
			string json = sb.ToString();
			return format ? FormatJson(json) : json;
		}

		//Converts the raw json to whatever your object is
		public static T ToObject<T>(string json)
		{
			var jsonObject = JsonDecode(json);
			if (jsonObject is T)
			{
				return (T)jsonObject;
			}
			return (T)JsonDecode(jsonObject, typeof(T));
		}

		public static object ToObject(string json, Type objectType)
		{
			return JsonDecode(json, objectType);
		}

		//Converts a Hashtable to whatever your object is
		public static T ToObject<T>(Hashtable jsonObject)
		{
			return (T)JsonDecode(jsonObject, typeof(T));
		}

		public static object ToObject(Hashtable jsonObject, Type objectType)
		{
			return JsonDecode(jsonObject, objectType);
		}

		//Converts an ArrayList to whatever your object is
		public static T ToObject<T>(ArrayList jsonObject)
		{
			return (T)JsonDecode(jsonObject, typeof(T));
		}

		public static object ToObject(ArrayList jsonObject, Type objectType)
		{
			return JsonDecode(jsonObject, objectType);
		}

		#region Attributes
		//Defines the name of a field to use when serialize/deserializing the field
		public class JsonName : Attribute
		{
			public string Name;
			public JsonName(string Name)
			{
				this.Name = Name;
			}
		}
		public class JsonSerializeField : Attribute { }         //Include this field, even if its private or doesnt have SerializeField
		public class JsonIgnore : Attribute { }                 //Ignore this field for life.
		public class JsonIgnoreCustomExporter : Attribute { }   //Dont use the custom exporter for this class or field
		public class JsonIgnoreCustomImporter : Attribute { }   //Dont use the custom importer for this class or field

		public class FieldSerializer : Attribute
		{
			public Type serializerType;
			public FieldSerializer(Type serializerType)
			{
				this.serializerType = serializerType;
			}
		}
		#endregion

		#region Export, from Object to String		
		private static void ToJson(Type serializationType, object value, StringBuilder builder, bool customExporterField = false)
		{
			if (value != null &&
				exporters.ContainsKey(value.GetType()) &&
				(value.GetType().GetCustomAttributes(typeof(JsonIgnoreCustomExporter), true).Length == 0 || customExporterField))
			{
				//It's difficult to check the target type, just try and continue otherwise
				try
				{
					object val = exporters[value.GetType()](value);
					ToJson(val.GetType(), val, builder);
					return;
				}
				catch (InvalidCastException)
				{
				}
			}

			//Try the default stuff, which doesnt support classes, arrays or enums
			if (!instance.SerializeValue(value, builder))
			{
				Type actualObjectType = value.GetType();
				if (typeof(ICollection).IsAssignableFrom(actualObjectType)) //Is it Collection-ish?
				{
					SerializeArray(value, actualObjectType, builder);
				}
				else if (actualObjectType.IsClass || (actualObjectType.IsValueType && !actualObjectType.IsPrimitive && !actualObjectType.IsEnum))
				{
					SerializeClass(serializationType, value, actualObjectType, builder);
				}
				else if (actualObjectType.IsEnum)
				{
					SeralizeEnum(value, actualObjectType, builder);
				}
				else
				{
					throw new Exception(string.Format("Cannot convert! Value: {0} type: {1}, build so far: {2}",
													  value,
													  actualObjectType,
													  builder.ToString()));
				}
			}
		}

		//Serialises Arrays and stuff that looks like an array
		private static void SerializeArray(object value, Type targetType, StringBuilder builder)
		{
			Type[] genArgs = GetGenericArguments(targetType);       // targetType.GetGenericArguments();

			if (targetType.IsArray)                         //Is it an actual array, type[]
			{
				Array arr = value as Array;
				builder.Append('[');
				for (int i = 0; i < arr.Length; i++)
				{
					ToJson(targetType.GetElementType(), arr.GetValue(i), builder);

					if (i < arr.Length - 1)
					{
						builder.Append(",");
					}
				}
				builder.Append(']');
			}
			else if (genArgs.Length == 1) //One generic argument, just a List<T> or something. Convert to [T,T,T]
			{
#if UNITY_IPHONE
				bool first = true;
				builder.Append('[');
				AOTSafeForEach(value, (object item) =>
				{
					if(!first)
					{
						builder.Append(",");
					}

					ToJson (genArgs[0], item, builder);
					first = false;
				});
				builder.Append(']');
#else

				var list = value as IEnumerable;
				bool first = true;
				builder.Append('[');
				foreach (var item in list)
				{
					if (!first)
					{
						builder.Append(",");
					}
					ToJson(genArgs[0], item, builder);

					first = false;
				}
				builder.Append(']');
#endif
			}
			else if (genArgs.Length == 2)           //Two generic args, something like Dictionary<T,I>. Convert to { "T" : I, "T" : I }
			{
				var dic = value as IDictionary;
				bool first = true;
				builder.Append('{');
				foreach (DictionaryEntry item in dic)
				{
					if (!first)
					{
						builder.Append(", ");
					}
					ToJson(genArgs[0], item.Key, builder);
					builder.Append(" : ");
					ToJson(genArgs[1], item.Value, builder);

					first = false;
				}
				builder.Append('}');
			}
			else
			{
				throw new Exception(string.Format("Cannot convert! Value: {0} type: {1}, build so far:", value, targetType, builder.ToString()));
			}
		}

		private static void SerializeClass(Type serializationType, object value, Type targetType, StringBuilder builder)
		{
			FieldInfo[] fields = GetFieldsToSerialize(targetType);
			builder.Append('{');
			Type realObjectType = value.GetType();
			if (realObjectType != serializationType)
			{
				builder.AppendFormat("\"{0}\" : \"{1}\"", TYPE_KEY, realObjectType.FullName);

				if (fields.Length > 0)
				{
					builder.Append(",");
				}
			}

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo field = fields[i];
				builder.AppendFormat("\"{0}\" : ", GetFieldJsonName(field));

				object[] attributes = field.GetCustomAttributes(typeof(FieldSerializer), true);
				if (attributes.Length > 0)
				{
					FieldSerializer attr = (FieldSerializer)attributes[0];
					IJsonSerializer serializer = GetSerializer(attr.serializerType);
					string json = serializer.Serialize(field.GetValue(value));
					builder.Append(json);
				}
				else
				{
					object fieldValue = field.GetValue(value);
					ToJson(fieldValue.GetType(), fieldValue, builder, !field.HasAttribute<JsonIgnoreCustomExporter>());
				}

				if (i < fields.Length - 1)
				{
					builder.Append(",");
				}
			}
			builder.Append('}');
		}

		private static void SeralizeEnum(object value, Type targetType, StringBuilder builder)
		{
			builder.Append("\"").Append(value.ToString()).Append("\"");
		}

		//Aot sucks balls! On iOS foreaching over an object, which is just casted to an Enumerable can go wrong. 
		//Read more: http://forum.unity3d.com/threads/168019-quot-System-String-doesn-t-implement-interface-System-Collections-IEnumerator-quot-crash 
		private static void AOTSafeForEach<T>(object enumerable, Action<T> action)
		{
			Type listType = null;
			Type[] interfaceTypes = enumerable.GetType().GetInterfaces();
			for (int i = 0; i < interfaceTypes.Length; i++)
			{
				if (!interfaceTypes[i].IsGenericType && interfaceTypes[i] == typeof(IEnumerable))
				{
					listType = interfaceTypes[i];
					break;
				}
			}

			if (listType == null)
			{
				throw new ArgumentException("Object does not implement IEnumerable interface", "enumerable");
			}

			System.Reflection.MethodInfo method = listType.GetMethod("GetEnumerator");
			if (method == null)
			{
				throw new InvalidOperationException("Failed to get 'GetEnumberator()' method info from IEnumerable type");
			}

			IEnumerator enumerator = null;
			try
			{
				enumerator = (IEnumerator)method.Invoke(enumerable, null);
				if (enumerator is IEnumerator)
				{
					while (enumerator.MoveNext())
					{
						action((T)enumerator.Current);
					}
				}
				else
				{
					LogUtil.Log(LogTags.SYSTEM, "JSONMapper", string.Format("{0}.GetEnumerator() returned '{1}' instead of IEnumerator.",
														enumerable.ToString(),
														enumerator.GetType().Name));
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		#endregion

		#region Importing, from String to Object
		//Converts a ArrayList, a Hashtable, a double, a string, null, true, or false to a sensible type.
		//shouldntCheckCustomImporterAttr is true when the field we came from doesnt have this attr. 
		private static object JsonDecode(object jsonObject, Type targetType, bool customImporterField = false)
		{
			//Is there an importer, and shouldnt we ignore this class?
			if (importers.ContainsKey(targetType) && (targetType.GetCustomAttributes(typeof(JsonIgnoreCustomImporter), true).Length <= 0 || customImporterField))
			{
				try //It's difficult to actually check the type of the importer, so just try and continue if it fails
				{
					return importers[targetType](jsonObject);
				}
				catch (InvalidCastException) { }
			}

			if (jsonObject == null)
			{
				return null;
			}
			else if (jsonObject is Hashtable) //Object, convert it to a class!
			{
				return DeserializeClass(jsonObject, targetType);
			}
			else if (jsonObject is ArrayList) //ArrayList, lets make a cool array!
			{
				return DeserializeArray(jsonObject, targetType);
			}
			else if (targetType.IsEnum)
			{
				return Enum.Parse(targetType, jsonObject.ToString());
			}
			else if (targetType == typeof(Hashtable))
			{
				return JsonDecode(jsonObject.ToString());
			}
			else if (targetType == typeof(SecureInt))
			{
				int jsonInt = (int)Convert.ChangeType(jsonObject, typeof(int));
				return (SecureInt)jsonInt;
			}
			else if (targetType == typeof(SecureFloat))
			{
				float jsonFloat = (float)Convert.ChangeType(jsonObject, typeof(float));
				return (SecureFloat)jsonFloat;
			}
			else if (targetType == typeof(DateTime))
			{
				return DateTime.Parse(jsonObject.ToString());
			}
			else if (targetType == typeof(UDateTime))
			{
				return (UDateTime)DateTime.Parse(jsonObject.ToString());
			}
			else //Anything else, attempt to convert it. 
			{
				try
				{
					return Convert.ChangeType(jsonObject, targetType);
				}
				catch (InvalidCastException)
				{
					LogUtil.Error(LogTags.DATA, "JSONMapper", string.Format("Couldn't convert {0} of type {1} to type {2}", jsonObject, jsonObject.GetType(), targetType));
					return null;
				}
			}
		}

		private static object DeserializeClass(object jsonObject, Type targetType)
		{
			Hashtable jsonHash = jsonObject as Hashtable;
			Type[] genericArgs = GetGenericArguments(targetType);
			//We also convert to a Dictionary<T,U> because they are semantically the same in the JSON. Lets check if we are just that. 

			if (jsonHash.ContainsKey(TYPE_KEY))
			{
				targetType = Type.GetType(jsonHash[TYPE_KEY].ToString());
			}

			if (typeof(IEnumerable).IsAssignableFrom(targetType) && genericArgs.Length == 2)
			{
				IDictionary dic = Activator.CreateInstance(targetType) as IDictionary;
				//TODO: This seems to reverse the order of the elements, we might want to correct this?
				foreach (DictionaryEntry obj in jsonHash)
				{
					object key = JsonDecode(obj.Key, genericArgs[0]);
					object value = JsonDecode(obj.Value, genericArgs[1]);
					dic.Add(key, value);
				}
				return dic;
			}
			else
			{
				var resultObject = Activator.CreateInstance(targetType);
				FieldInfo[] fields = GetFieldsToSerialize(targetType);
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo field = fields[i];
					string fieldName = GetFieldJsonName(field);
					if (jsonHash.ContainsKey(fieldName))
					{
						object value = jsonHash[fieldName];

						// test for custom field deserialization.
						object[] attributes = field.GetCustomAttributes(typeof(FieldSerializer), true);
						if (attributes.Length > 0)
						{
							FieldSerializer attr = (FieldSerializer)attributes[0];
							IJsonSerializer deserializer = GetSerializer(attr.serializerType);
							value = deserializer.Deserialize(value);
						}
						else
						{
							value = JsonDecode(value, field.FieldType, !field.HasAttribute<JsonIgnoreCustomImporter>());
						}

						field.SetValue(resultObject, value);
					}
#if STRICT_CASTING
					else
					{
						LogUtility.LogError(LogTags.SYSTEM, "JSONMapper", "The local field " + fi.Name + " with jsonName: " + fieldName + " does not exist in the JSON!");
					}
#endif
				}
				return resultObject;
			}
		}

		private static object DeserializeArray(object jsonObject, Type targetType)
		{
			ArrayList arr = jsonObject as ArrayList;
			/*
			 * An array in C# is either an array, or something IEnumerable. 
			 * Such as list, dictionary etc. 
			 */
			if (typeof(IEnumerable).IsAssignableFrom(targetType)) //Check if the type we are casting to is an array as well.
			{
				if (targetType.IsArray)
				{
					Type elemType = targetType.GetElementType();
					Array resultArray = Array.CreateInstance(elemType, arr.Count);
					for (int i = 0; i < arr.Count; i++)
					{
						object value = JsonDecode(arr[i], elemType);
						resultArray.SetValue(value, i);
					}
					return Convert.ChangeType(resultArray, targetType);
				}
				else if (arr.Count == 0)
				{
					return Activator.CreateInstance(targetType);
				}
				else
				{
					Type[] types = GetGenericArguments(targetType);
					Type elemType = types.Length > 0 ? types[0] : null;

					IList list = (IList)Activator.CreateInstance(targetType);

					for (int i = 0; i < arr.Count; i++)
					{
						object value = JsonDecode(arr[i], elemType);
						list.Add(value);
					}
					return list;
				}
			}
			else
			{
				throw new Exception("Cannot convert non array " + targetType + " to array " + jsonObject.GetType());
			}
		}

		//We should only serialise public or SerializeFields which are not static.
		private static FieldInfo[] GetFieldsToSerialize(Type type)
		{
			FieldInfo[] infos = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			List<FieldInfo> infosList = new List<FieldInfo>();
			for (int i = 0; i < infos.Length; i++)
			{
				FieldInfo f = infos[i];
				if (!f.HasAttribute<JsonIgnore>() && (f.IsPublic || f.HasAttribute<SerializeField>() || f.HasAttribute<JsonSerializeField>()) && !f.IsStatic)
				{
					infosList.Add(f);
				}
			}
			return infosList.ToArray();
		}

		//Gets the generic args from a Dictionary or List. We can't use System.Type.GetGenericArguments because this breaks everything on iOS.
		//Do this by finding the Add method, and returning the types of the parameters.
		public static Type[] GetGenericArguments(Type t)
		{
			MethodInfo addMethod = t.GetMethod("Add");

			if (addMethod == null)
			{
				return new Type[0];
			}
			ParameterInfo[] pinfo = addMethod.GetParameters();
			List<Type> types = new List<Type>();
			for (int i = 0; i < pinfo.Length; i++)
			{
				types.Add(pinfo[i].ParameterType);
			}
			return types.ToArray();
		}

		//Returns either the field name, or the name defined in a JsonName attribute
		private static string GetFieldJsonName(System.Reflection.FieldInfo finfo)
		{
			var attrs = finfo.GetCustomAttributes(typeof(JsonName), true);
			return (attrs.Length > 0) ? (attrs[0] as JsonName).Name : finfo.Name;
		}
		#endregion

		#region Custom exporters /importers			
		private static Dictionary<Type, Func<object, object>> importers = new Dictionary<Type, Func<object, object>>();
		private static Dictionary<Type, Func<object, object>> exporters = new Dictionary<Type, Func<object, object>>();

		//You can register custom importers for types. Just add a function, this gets called whenever 'TValue' is being imported. The argument is whatever JSON thinks the object was, so Hashtable/ArrayList/primitve
		//TValue is return type, the value in the C# class. TJson is the argument, type in JSON
		public static void RegisterImporter<TValue, TJson>(Func<TJson, TValue> importer)
		{
			Func<object, object> wrapperObj = delegate (object o) { return importer((TJson)o); };
			importers[typeof(TValue)] = wrapperObj;
		}

		//TJson is the return type, what gets put in the JSON. TValue is what the C# object is
		public static void RegisterExporter<TJson, TValue>(Func<TValue, TJson> exporter)
		{
			Func<object, object> wrapperObj = delegate (object o) { return exporter((TValue)o); };
			exporters[typeof(TValue)] = wrapperObj;
		}

		public static void UnregisterImporter(Type type)
		{
			importers.Remove(type);
		}

		public static void UnregisterExporter(Type type)
		{
			exporters.Remove(type);
		}
		#endregion

		#region Custom serializers
		public interface IJsonSerializer
		{
			string Serialize(object value);
			object Deserialize(object jsonObject);
		}

		private static Dictionary<Type, IJsonSerializer> serializers = new Dictionary<Type, IJsonSerializer>();

		private static IJsonSerializer GetSerializer(Type type)
		{
			IJsonSerializer serializer;
			if (!serializers.TryGetValue(type, out serializer))
			{
				serializer = System.Activator.CreateInstance(type) as IJsonSerializer;
				if (serializer != null)
				{
					serializers.Add(type, serializer);
				}
				else
				{
					LogUtil.Error(LogTags.DATA, "JSONMapper", "Unable to create serialzer for " + type.Name);
				}
			}
			return serializer;
		}
		#endregion

		#region Readable JSON
		public static string FormatJson(string str)
		{
			var indent = 0;
			var quoted = false;
			var sb = new StringBuilder();
			for (var i = 0; i < str.Length; i++)
			{
				var ch = str[i];
				switch (ch)
				{
					case '{':
					case '[':
						sb.Append(ch);
						if (!quoted && i < str.Length - 1 && str[i + 1] != ']' && str[i + 1] != '}')
						{
							sb.AppendLine();
							++indent;
							sb.Append('\t', indent);
						}
						break;
					case '}':
					case ']':
						if (!quoted && i > 0 && sb[sb.Length - 1] != '[' && sb[sb.Length - 1] != '{')
						{
							sb.AppendLine();
							--indent;
							sb.Append('\t', indent);
						}
						sb.Append(ch);
						break;
					case '"':
						sb.Append(ch);
						bool escaped = false;
						var index = i;
						while (index > 0 && str[--index] == '\\')
						{
							escaped = !escaped;
						}

						if (!escaped)
						{
							quoted = !quoted;
						}

						break;
					case ',':
						sb.Append(ch);
						if (!quoted)
						{
							sb.AppendLine();
							sb.Append('\t', indent);
						}
						break;
					case ':':
						sb.Append(ch);
						if (!quoted)
						{
							sb.Append(" ");
						}

						break;
					default:
						sb.Append(ch);
						break;
				}
			}
			return sb.ToString();
		}
		#endregion
	}
}