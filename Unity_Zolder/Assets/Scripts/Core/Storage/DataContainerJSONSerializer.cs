// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.MemoryScramble;

namespace Talespin.Core.Foundation.Storage
{
	/// <summary>
	/// Allows serialization and deserialization of DataContainer's using JSON.
	/// </summary>
	public static class DataContainerJSONSerializer
	{
		public enum SupportedType
		{
			None,
			String,
			Double,
			Bool,
			Null
		}

		public static string Serialize(IDataContainer dataContainer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			GenerateObjectNode(dataContainer, ref stringBuilder);
			return stringBuilder.ToString();
		}

		public static DataContainer Deserialize(string jsonString)
		{
			if (string.IsNullOrEmpty(jsonString))
			{
				return new DataContainer();
			}

			int currentIndex = FindFirstValidCharacterIndex(jsonString);
			IDataContainer targetDataContainer = new DataContainer();
			ParseObject(jsonString, ref currentIndex, ref targetDataContainer);

			return targetDataContainer as DataContainer;
		}

		private static int FindFirstValidCharacterIndex(string jsonString)
		{
			int currentIndex = 0;
			while (jsonString.Length > currentIndex)
			{
				char character = jsonString[currentIndex];
				currentIndex++;
				switch (character)
				{
					case '{':
						return currentIndex;
					case ' ':
						continue;
					case '[':
						throw new InvalidOperationException("JSON started with an array node, the serializer expects" +
							" the starting node to be a normal JSON node.");
					default:
						throw new InvalidOperationException($"JSON started with an invalid starting character" +
							$" '{character}' at index '{currentIndex - 1}'");
				}
			}

			return -1;
		}

		private static void GenerateObjectNode(IDataContainer dataContainer, ref StringBuilder stringBuilder)
		{
			stringBuilder.Append("{");

			if (dataContainer != null)
			{
				Dictionary<string, object> data = dataContainer.GetKeyValuePairs();
				object value;
				Type type;
				foreach (string key in data.Keys)
				{
					value = data[key];
					if (value == null)
					{
						type = typeof(string);
					}
					else
					{
						type = value.GetType();
					}

					stringBuilder.Append("\"");
					stringBuilder.Append(key);
					stringBuilder.Append("\":");

					if (type == typeof(DataContainer))
					{
						GenerateObjectNode(value as IDataContainer, ref stringBuilder);
					}
					else if (type == typeof(string[]))
					{
						GenerateStringArrayNode((string[])value, ref stringBuilder);
					}
					else if (type == typeof(DataContainer[]))
					{
						GenerateDataContainerArrayNode((DataContainer[])value, ref stringBuilder);
					}
					else if (type == typeof(SecureBool[]))
					{
						GenerateBoolArrayNode((SecureBool[])value, ref stringBuilder);
					}
					else if (type == typeof(SecureDouble[]))
					{
						GenerateDoubleArrayNode((SecureDouble[])value, ref stringBuilder);
					}
					else if (type == typeof(SecureBool))
					{
						stringBuilder.Append(((SecureBool)value).ToString().ToLower());
					}
					else if (type == typeof(SecureDouble))
					{
						stringBuilder.Append(((SecureDouble)value).ToString());
					}
					else if (value == null)
					{
						// do nothing
					}
					else
					{
						AppendString(value.ToString(), ref stringBuilder);
					}

					stringBuilder.Append(",");
				}

				if (stringBuilder[stringBuilder.Length - 1] == ',')
				{
					// remove last ,
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
				}
			}

			stringBuilder.Append("}");
		}

		private static void GenerateStringArrayNode(string[] valueArray, ref StringBuilder stringBuilder)
		{
			stringBuilder.Append("[");

			if (valueArray == null || valueArray.Length == 0)
			{
				stringBuilder.Append("]");
				return;
			}

			for (int i = 0; i < valueArray.Length; i++)
			{
				AppendString(valueArray[i], ref stringBuilder);

				stringBuilder.Append(",");
			}

			if (stringBuilder[stringBuilder.Length - 1] == ',')
			{
				// remove last ,
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}

			stringBuilder.Append("]");
		}

		private static void GenerateDataContainerArrayNode(DataContainer[] valueArray, ref StringBuilder stringBuilder)
		{
			stringBuilder.Append("[");

			if (valueArray == null || valueArray.Length == 0)
			{
				stringBuilder.Append("]");
				return;
			}

			object value;
			for (int i = 0; i < valueArray.Length; i++)
			{
				value = valueArray[i];

				GenerateObjectNode(value as IDataContainer, ref stringBuilder);

				stringBuilder.Append(",");
			}

			if (stringBuilder[stringBuilder.Length - 1] == ',')
			{
				// remove last ,
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}

			stringBuilder.Append("]");
		}

		private static void GenerateBoolArrayNode(SecureBool[] valueArray, ref StringBuilder stringBuilder)
		{
			stringBuilder.Append("[");

			if (valueArray == null || valueArray.Length == 0)
			{
				stringBuilder.Append("]");
				return;
			}

			for (int i = 0; i < valueArray.Length; i++)
			{
				stringBuilder.Append(valueArray[i].ToString().ToLower());
				stringBuilder.Append(",");
			}

			if (stringBuilder[stringBuilder.Length - 1] == ',')
			{
				// remove last ,
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}

			stringBuilder.Append("]");
		}

		private static void GenerateDoubleArrayNode(SecureDouble[] valueArray, ref StringBuilder stringBuilder)
		{
			stringBuilder.Append("[");

			if (valueArray == null || valueArray.Length == 0)
			{
				stringBuilder.Append("]");
				return;
			}

			for (int i = 0; i < valueArray.Length; i++)
			{
				stringBuilder.Append(valueArray[i]);
				stringBuilder.Append(",");
			}

			if (stringBuilder[stringBuilder.Length - 1] == ',')
			{
				// remove last ,
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}

			stringBuilder.Append("]");
		}

		private static void AppendString(string value, ref StringBuilder stringBuilder)
		{
			stringBuilder.Append('"');

			EscapeStringValue(value, stringBuilder);

			stringBuilder.Append('"');
		}

		private static void EscapeStringValue(string value, StringBuilder targetStringBuilder)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}

			char character = '\0';
			int i;
			int len = value.Length;
			String t;

			for (i = 0; i < len; i += 1)
			{
				character = value[i];
				switch (character)
				{
					case '\\':
					case '"':
						targetStringBuilder.Append('\\');
						targetStringBuilder.Append(character);
						break;
					case '/':
						targetStringBuilder.Append('\\');
						targetStringBuilder.Append(character);
						break;
					case '\b':
						targetStringBuilder.Append("\\b");
						break;
					case '\t':
						targetStringBuilder.Append("\\t");
						break;
					case '\n':
						targetStringBuilder.Append("\\n");
						break;
					case '\f':
						targetStringBuilder.Append("\\f");
						break;
					case '\r':
						targetStringBuilder.Append("\\r");
						break;
					default:
						if (character < ' ')
						{
							t = String.Format("X", character);
							t = t.Substring(t.Length - 1);
							targetStringBuilder.Append("\\u");
							targetStringBuilder.Append("000");
							targetStringBuilder.Append(t);
						}
						else
						{
							targetStringBuilder.Append(character);
						}
						break;
				}
			}
		}

		private static string[] ParseStringArray(string jsonString, ref int currentIndex)
		{
			List<string> list = new List<string>();

			SupportedType valueType = SupportedType.None;
			string nextValue = "";
			char currentCharacter;
			while (currentIndex < jsonString.Length)
			{
				currentCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				// add a new entry if a quoted string value can be found.
				if (currentCharacter == '"')
				{
					nextValue = GetNextValue(jsonString, ref currentIndex, out valueType);
					list.Add(nextValue);
					currentCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				}

				switch (currentCharacter)
				{
					case ',':
						break;

					case ']':
						// done
						currentIndex++;
						return list.ToArray();

					default:
						throw new InvalidOperationException("Invalid JSON by " + currentCharacter);
				}

				currentIndex++;
			}

			return list.ToArray();
		}

		private static double[] ParseDoubleArray(string jsonString, ref int currentIndex)
		{
			List<double> list = new List<double>();

			SupportedType valueType = SupportedType.None;
			string nextValue = "";
			double nextValueDouble;
			bool success;
			char currentCharacter;
			while (currentIndex < jsonString.Length)
			{
				currentCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				// add a new entry if a quoted string value can be found.
				if (char.IsNumber(currentCharacter))
				{
					nextValue = GetNextValue(jsonString, ref currentIndex, out valueType);
					success = double.TryParse(nextValue, out nextValueDouble);
					if (success)
					{
						list.Add(nextValueDouble);
					}
					else
					{
						throw new InvalidOperationException("Invalid JSON by " + nextValue);
					}
					currentCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				}

				switch (currentCharacter)
				{
					case ',':
						break;

					case ']':
						// done
						currentIndex++;
						return list.ToArray();

					default:
						throw new InvalidOperationException("Invalid JSON by " + currentCharacter);
				}

				currentIndex++;
			}

			return list.ToArray();
		}

		private static bool[] ParseBoolArray(string jsonString, ref int currentIndex)
		{
			List<bool> list = new List<bool>();

			SupportedType valueType = SupportedType.None;
			string nextValue = "";
			bool nextValueBool;
			bool success;
			char currentCharacter;
			while (currentIndex < jsonString.Length)
			{
				currentCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				// add a new entry if a quoted string value can be found.
				if (currentCharacter == 't' ||
					currentCharacter == 'f' ||
					currentCharacter == 'T' ||
					currentCharacter == 'F')
				{
					nextValue = GetNextValue(jsonString, ref currentIndex, out valueType);
					success = bool.TryParse(nextValue, out nextValueBool);
					if (success)
					{
						list.Add(nextValueBool);
					}
					else
					{
						throw new InvalidOperationException("Invalid JSON by " + nextValue);
					}
					currentCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				}

				switch (currentCharacter)
				{
					case ',':
						break;

					case ']':
						// done
						currentIndex++;
						return list.ToArray();

					default:
						throw new InvalidOperationException("Invalid JSON by " + currentCharacter);
				}

				currentIndex++;
			}

			return list.ToArray();
		}

		private static DataContainer[] ParseDataContainerArray(string jsonString, ref int currentIndex)
		{
			List<DataContainer> list = new List<DataContainer>();

			IDataContainer nextValueDataContainer;
			char currentCharacter;
			while (currentIndex < jsonString.Length)
			{
				currentCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				// add a new entry if a quoted string value can be found.
				if (currentCharacter == '{')
				{
					currentIndex++;
					nextValueDataContainer = new DataContainer();
					ParseObject(jsonString, ref currentIndex, ref nextValueDataContainer);

					list.Add(nextValueDataContainer as DataContainer);

					currentCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				}

				switch (currentCharacter)
				{
					case ',':
						break;

					case ']':
						// done
						currentIndex++;
						return list.ToArray();

					default:
						throw new InvalidOperationException("Invalid JSON by " + currentCharacter);
				}

				currentIndex++;
			}

			return list.ToArray();
		}

		/// <summary>
		/// Modified version of same method used for SimpleJSON lib written by Bunny83.
		/// </summary>
		private static void ParseObject(string jsonString, ref int currentIndex, ref IDataContainer targetDataContainer)
		{
			SupportedType valueType = SupportedType.None;
			string nextKey = "";
			string nextValue = "";
			char currentJSONCharacter;
			IDataContainer newDataContainer;
			while (currentIndex < jsonString.Length)
			{
				nextKey = GetNextValue(jsonString, ref currentIndex, out valueType);
				currentJSONCharacter = GetNextJSONChar(jsonString, ref currentIndex);
				switch (currentJSONCharacter)
				{
					case '}':
						// done
						currentIndex++;
						return;

					case ',':
						break;

					case ':':
						currentIndex++;

						char nextJSONChar = GetNextJSONChar(jsonString, ref currentIndex);
						switch (nextJSONChar)
						{
							case '[':
								currentIndex++;
								AddArray(nextKey, jsonString, ref currentIndex, ref targetDataContainer);
								currentIndex--;
								break;

							case '{':
								currentIndex++;
								newDataContainer = new DataContainer();
								ParseObject(jsonString, ref currentIndex, ref newDataContainer);
								targetDataContainer.Set(nextKey, newDataContainer as DataContainer);
								currentIndex--;
								break;

							default:
								// it appears not to be an array or object, add normal value.
								nextValue = GetNextValue(jsonString, ref currentIndex, out valueType);
								AddValue(nextKey, nextValue, valueType, ref targetDataContainer);
								currentIndex--;
								break;
						}
						break;

					case '[':
					case '{':
					case ']':
						throw new InvalidOperationException("Invalid JSON by character " +
							currentJSONCharacter + " at index " + currentIndex + ", nextKey: " + nextKey +
							", in string: " + jsonString + ", remaining string: " +
							jsonString.Substring(currentIndex));

					default:
						throw new InvalidOperationException("Invalid JSON by character " +
							currentJSONCharacter + " at index " + currentIndex + ", nextKey: " + nextKey +
							", in string: " + jsonString + ", remaining string: " +
							jsonString.Substring(currentIndex));
				}

				currentIndex++;
			}
		}

		private static void AddArray(string key, string jsonString, ref int currentIndex, ref IDataContainer targetDataContainer)
		{
			char nextJSONChar = GetNextJSONChar(jsonString, ref currentIndex);
			switch (nextJSONChar)
			{
				case '{':
					targetDataContainer.Set(key, ParseDataContainerArray(jsonString, ref currentIndex));
					break;

				case '"':
					targetDataContainer.Set(key, ParseStringArray(jsonString, ref currentIndex));
					break;

				case 'f':
				case 'F':
				case 't':
				case 'T':
					targetDataContainer.Set(key, ParseBoolArray(jsonString, ref currentIndex));
					break;

				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					targetDataContainer.Set(key, ParseDoubleArray(jsonString, ref currentIndex));
					break;

				case ']':
					// skip this array, since we cannot determine the correct element type.
					currentIndex++;
					break;

				default:
					throw new InvalidOperationException("Invalid JSON by character " + nextJSONChar);
			}
		}

		private static void AddValue(string key, string value, SupportedType valueType, ref IDataContainer targetDataContainer)
		{
			key = key.Trim();
			value = value.Trim();

			if (valueType == SupportedType.Null)
			{
				// don't add it.
				return;
			}
			else if (valueType == SupportedType.String)
			{
				targetDataContainer.Set(key, value);
				return;
			}
			else if (valueType == SupportedType.Double)
			{
				double doubleValue;
				if (double.TryParse(value, out doubleValue))
				{
					targetDataContainer.Set(key, doubleValue);
					return;
				}
			}
			else if (valueType == SupportedType.Bool)
			{
				bool boolValue;
				if (bool.TryParse(value, out boolValue))
				{
					targetDataContainer.Set(key, boolValue);
					return;
				}
			}

			LogUtil.Error(LogTags.DATA, "DataContainerJSONSerializer", "Parsing failed for key: " + key + ", value: " +
				value);
		}

		/// <summary>
		/// Returns the next character in the string that holds meaning to JSON.
		/// e.g.: it skips white spaces, tabs, line breaks, etc.
		/// Returns ' ' if nothing can be found.
		/// </summary>
		/// <param name="jsonString"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		private static char GetNextJSONChar(string jsonString, ref int currentIndex)
		{
			char currentCharacter;
			while (currentIndex < jsonString.Length)
			{
				currentCharacter = jsonString[currentIndex];
				switch (currentCharacter)
				{
					case ' ':
					case '\r':
					case '\n':
					case '\t':
						break;

					default:
						return currentCharacter;
				}

				currentIndex++;
			}

			return ' ';
		}

		private static StringBuilder valueStringBuilder = new StringBuilder();

		private static string GetNextValue(string jsonString, ref int currentIndex, out SupportedType valueType)
		{
			// recycle string builder, empty it
			valueStringBuilder.Length = 0;
			StringBuilder nextValue = valueStringBuilder;

			valueType = SupportedType.None;

			bool quoteMode = false;
			char currentCharacter;
			while (currentIndex < jsonString.Length)
			{
				currentCharacter = jsonString[currentIndex];
				switch (currentCharacter)
				{
					case 'n':
						if (!quoteMode && jsonString[currentIndex + 1] == 'u' && jsonString[currentIndex + 2] == 'l' && jsonString[currentIndex + 3] == 'l')
						{
							valueType = SupportedType.Null;
							currentIndex += 3;
							return "null";
						}
						nextValue.Append(currentCharacter);
						break;

					case 'f':
					case 'F':
					case 't':
					case 'T':
						nextValue.Append(currentCharacter);
						if (!quoteMode)
						{
							valueType = SupportedType.Bool;
						}
						break;

					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case '.':
						nextValue.Append(currentCharacter);
						if (!quoteMode)
						{
							valueType = SupportedType.Double;
						}
						break;

					case '{':
					case '[':
					case ':':
						if (quoteMode)
						{
							nextValue.Append(currentCharacter);
							break;
						}
						valueType = SupportedType.None;
						return nextValue.ToString().Trim();

					case ']':
					case '}':
					case ',':
						if (quoteMode)
						{
							nextValue.Append(currentCharacter);
							break;
						}

						return nextValue.ToString().Trim();

					case '"':
						quoteMode ^= true;
						if (quoteMode)
						{
							valueType = SupportedType.String;
						}
						else
						{
							currentIndex++;
							valueType = SupportedType.String;
							return nextValue.ToString().Trim();
						}
						break;

					case ' ':
					case '\t':
					case '\r':
					case '\n':
						if (quoteMode)
						{
							nextValue.Append(currentCharacter);
							break;
						}
						break;

					case '\\':
						currentIndex++;
						if (quoteMode)
						{
							char nextChar = jsonString[currentIndex];
							switch (nextChar)
							{
								case 't': nextValue.Append('\t'); break;
								case 'r': nextValue.Append('\r'); break;
								case 'n': nextValue.Append('\n'); break;
								case 'b': nextValue.Append('\b'); break;
								case 'f': nextValue.Append('\f'); break;
								case 'u':
								{
									string s = jsonString.Substring(currentIndex + 1, 4);
									nextValue.Append((char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier));
									currentIndex += 4;
									break;
								}

								default:
									nextValue.Append(nextChar);
									break;
							}
						}
						break;

					default:
						nextValue.Append(currentCharacter);
						break;
				}
				currentIndex++;
			}

			return nextValue.ToString().Trim();
		}
	}
}
