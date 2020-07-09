#if UNITY_EDITOR
// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Talespin.Core.Foundation.Logging;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.AssetHandling
{
	public struct GuidResourceFile
	{
		public string Guid;
		public string AssetPath;
		public string Identifier;
		public bool Exists;
	}

	public static class GuidDatabaseUtility
	{
		public const string ROOT_DIRECTORY = "Library/GuidResources";
		public const string FILE_EXTENSION = ".guidbin";

		public static string DataPath
		{
			get
			{
				string dataPath = Application.dataPath;
				return dataPath.Substring(0, dataPath.Length - 7) + "/";
			}
		}

		public static string RootPath => DataPath + ROOT_DIRECTORY;

		public static GuidResourceFile GetLookup(string path)
		{
			GuidResourceFile resourceFile = new GuidResourceFile
			{
				AssetPath = path
			};

			string guid = GetAssetGUID(path);

			if (!string.IsNullOrEmpty(guid))
			{
				FileInfo file = new FileInfo(RootPath + "/" + path + FILE_EXTENSION);

				if (file.Exists)
				{
					resourceFile.Exists = true;
					ParseLookupFile(ref resourceFile, file);
				}
			}

			return resourceFile;
		}

		public static GuidResourceFile CreateLookup(string path)
		{
			GuidResourceFile resourceFile = new GuidResourceFile
			{
				AssetPath = path
			};

			string guid = GetAssetGUID(path);

			if (!string.IsNullOrEmpty(guid))
			{
				string guidDirectory = guid.Substring(0, 2);
				resourceFile.Guid = guid;

				if (EnsurePath(guidDirectory + "/"))
				{
					string contents = ConstructLookupFileStringContents(path, out resourceFile.Identifier);

					if (!string.IsNullOrEmpty(resourceFile.Identifier))
					{
						FileInfo file = new FileInfo(RootPath + "/" + guidDirectory + "/" + guid.Substring(2) + FILE_EXTENSION);

						if (file.Exists)
						{
							file.Delete();
						}

						using (BinaryWriter writer = new BinaryWriter(file.Create()))
						{
							writer.Write(contents);
						}

						resourceFile.Exists = true;
					}

					return resourceFile;
				}

				LogUtil.Error(LogTags.DATA, "Unable to ensure path for: " + path);
				return resourceFile;
			}

			return resourceFile;
		}

		public static GuidResourceFile DeleteLookup(string path)
		{
			string guid = GetAssetGUID(path);

			string lookupFilePath = RootPath + "/" + guid.Substring(0, 2) + "/" + guid.Substring(2) + FILE_EXTENSION;
			FileInfo file = new FileInfo(lookupFilePath);

			GuidResourceFile resourceFile = new GuidResourceFile
			{
				AssetPath = path
			};

			if (file.Exists)
			{
				DirectoryInfo directory = file.Directory;

				resourceFile.Exists = true;
				ParseLookupFile(ref resourceFile, file);
				file.Delete();

				if (directory.GetFiles().Length == 0 && directory.GetDirectories().Length == 0)
				{
					directory.Delete();
				}
			}

			return resourceFile;
		}

		public static IEnumerable<GuidResourceFile> LoadDatabase()
		{
			List<GuidResourceFile> result = new List<GuidResourceFile>();
			Queue<DirectoryInfo> queue = new Queue<DirectoryInfo>();

			if (EnsurePath(""))
			{
				queue.Enqueue(new DirectoryInfo(RootPath));

				while (queue.Count > 0)
				{
					DirectoryInfo directory = queue.Dequeue();

					DirectoryInfo[] children = directory.GetDirectories();
					for (int i = 0; i < children.Length; i++)
					{
						queue.Enqueue(children[i]);
					}

					FileInfo[] files = directory.GetFiles();
					ParseLookupFiles(files, result);
				}
			}

			return result;
		}

		private static void ParseLookupFiles(FileInfo[] files, ICollection<GuidResourceFile> result)
		{
			for (int i = 0; i < files.Length; i++)
			{
				GuidResourceFile resourceFile = new GuidResourceFile
				{
					AssetPath = files[i].FullName.Substring(RootPath.Length)
				};
				resourceFile.AssetPath = resourceFile.AssetPath.Substring(0, resourceFile.AssetPath.Length - FILE_EXTENSION.Length);

				if (ParseLookupFile(ref resourceFile, files[i]))
				{
					result.Add(resourceFile);
				}
			}
		}

		private static bool ParseLookupFile(ref GuidResourceFile resourceFile, FileInfo file)
		{
			if (file.Exists)
			{
				resourceFile.Guid = file.Directory.Name + Path.GetFileNameWithoutExtension(file.Name);

				using (BinaryReader reader = new BinaryReader(file.OpenRead()))
				{
					return ParseLookupFileContents(ref resourceFile, reader.ReadString());
				}
			}

			return false;
		}

		private static bool ParseLookupFileContents(ref GuidResourceFile resourceFile, string contents)
		{
			if (contents.StartsWith("version=1"))
			{
				string[] parts = contents.Split('\\');

				for (int j = 0; j < parts.Length; j++)
				{
					string[] token = parts[j].Split('=');

					if (token.Length == 1)
					{
						LogUtil.Error(LogTags.DATA, "Token length == 1" + " " + resourceFile.AssetPath + " (" + parts[j] + ")");
						continue;
					}

					string key = token[0];
					string value = token[1];

					switch (key)
					{
						case "identifier":
							resourceFile.Identifier = value;
							break;
					}
				}

				return true;
			}

			LogUtil.Error(LogTags.DATA, "Invalid lookup file version, unable to read: " + resourceFile.AssetPath);
			return false;
		}

		private static string ConstructLookupFileStringContents(string path, out string identifier)
		{
			string result = "version=1\\identifier=";

			if (Path.GetExtension(path) == ".cs")
			{
				identifier = ParseScriptFile(path);
				return result + identifier;
			}
			else
			{
				string[] parts = path.Split(new string[] { "Resources/" }, StringSplitOptions.RemoveEmptyEntries);
				int index = parts.Length - 1;
				identifier = parts[index];

				int extIndex = parts[index].LastIndexOf('.');
				if (extIndex != -1)
				{
					identifier = identifier.Substring(0, extIndex);
				}

				return result + identifier;
			}
		}

		/// <summary>
		/// Construct a string containing all valid identifiers in a script file,
		/// separated by "#".
		/// </summary>
		/// <param name="path">The path of the script file.</param>
		/// <returns>A string containing all valid identifiers in the script file.</returns>
		private static string ParseScriptFile(string path)
		{
			List<string> types = FindIdentifiersInFile(path);

			if (types.Count == 0)
			{
				return "";
			}

			StringBuilder builder = new StringBuilder();

			foreach (string type in types)
			{
				builder.Append(type);
				builder.Append(GuidDatabaseManager.TYPE_SEPARATOR);
			}

			return builder.ToString();
		}

		/// <summary>
		/// Find all valid identifiers in a script file.
		/// </summary>
		/// <param name="file">The path of the script file.</param>
		/// <returns>A list containing all valid identifiers in the script file.</returns>
		private static List<string> FindIdentifiersInFile(string file)
		{
			MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/" + file);
			List<string> result = new List<string>();

			if (script == null)
			{
				return result;
			}

			char[] seperatorChars = { ' ', '\n', '\t', '\r', ';', '\r', ':' };
			string[] tokenized = script.text.Split(seperatorChars, StringSplitOptions.RemoveEmptyEntries);

			List<string> namespaces = new List<string>();
			List<int> namespaceDepths = new List<int>();

			int currentDepth = 0;

			for (int i = 0; i < tokenized.Length; i++)
			{
				string token = tokenized[i];

				if (token == "namespace" && i + 1 < tokenized.Length)
				{
					namespaces.Add(tokenized[i + 1]);
					namespaceDepths.Add(currentDepth);
				}
				else if (token == "{")
				{
					currentDepth++;
				}
				else if (token == "}")
				{
					currentDepth--;

					for (int j = namespaces.Count - 1; j >= 0; j--)
					{
						if (namespaceDepths[j] == currentDepth)
						{
							namespaces.RemoveAt(j);
							namespaceDepths.RemoveAt(j);
						}
					}
				}
				else if (token == "public" &&
					((tokenized[i + 1] == "class" || (tokenized[i + 1] == "sealed" && tokenized[i + 2] == "class") || (tokenized[i + 1] == "abstract" && tokenized[i + 2] == "class")) ||
					tokenized[i + 1] == "interface" ||
					tokenized[i + 1] == "enum" ||
					tokenized[i + 1] == "struct"))
				{
					string className = (tokenized[i + 1] == "sealed" || tokenized[i + 1] == "abstract") ? tokenized[i + 3] : tokenized[i + 2];

					if (namespaces.Count > 0)
					{
						string completeNamespace = "";

						for (int j = 0; j < namespaces.Count; j++)
						{
							completeNamespace += namespaces[j];

							if (j < namespaces.Count - 1)
							{
								completeNamespace += ".";
							}
						}

						result.Add(completeNamespace + "." + className);
					}
					else
					{
						result.Add(className);
					}
				}
			}

			return result;
		}

		private static string GetAssetGUID(string path)
		{
			return AssetDatabase.AssetPathToGUID("Assets/" + path);
		}

		private static bool EnsurePath(string path)
		{
			string directory = string.IsNullOrEmpty(path) ? "" : Path.GetDirectoryName(path);

			DirectoryInfo info = Directory.CreateDirectory(RootPath + "/" + directory);
			return info.Exists;
		}
	}
}
#endif
