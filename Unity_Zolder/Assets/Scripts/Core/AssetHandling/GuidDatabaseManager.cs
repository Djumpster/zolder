// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using Talespin.Core.Foundation.Reflection;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Extensions;

#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEditor.Build;
#endif

namespace Talespin.Core.Foundation.AssetHandling
{
	/// <summary>
	/// Asset lookup table used to allow serialization of assets via GUID and 
	/// resolving them to a path within Resources at runtime.
	///
	/// This allows serialization of resource assets via strings while still supporting
	/// renaming of assets.
	///
	/// The mapping is stored in separate scriptable objects per GUID. This avoids
	/// merge conflicts that would happen if multiple people add new GUIDs (in completely
	/// different contexts). 
	/// </summary>
	public class GuidDatabaseManager
	{
		/// <summary>
		/// Get the instance of the GuidDatabaseManager.
		/// This is deliberately not implemented as a Service. Services are great at run-time,
		/// but most of the access to this asset happens at compile time, where using services
		/// is iffy at best. 
		/// </summary>
		public static GuidDatabaseManager Instance
		{
			get
			{
				if (instance == null)
				{
#if UNITY_EDITOR
					Dictionary<string, string[]> table = LoadEditorLookupFiles();
#else
					Dictionary<string, string[]> table = new Dictionary<string, string[]>();
					GuidDatabaseObject guidDatabaseObject = Resources.Load<GuidDatabaseObject>(BUILD_LOOKUP_FILE_NAME);

					foreach(GuidDatabaseObject.Asset asset in guidDatabaseObject.Assets)
					{
						table.Add(asset.GUID, asset.Value);
					}
#endif

					instance = new GuidDatabaseManager(table);
				}

				return instance;
			}
		}

		public const string BUILD_LOOKUP_FILE_PATH = "Assets/Data/Resources/";
		public const string BUILD_LOOKUP_FILE_NAME = "GuidDatabase";

		public const string RESOURCES_FOLDER = "Resources/";

		public const string CS_EXTENSION = ".cs";
		public const char TYPE_SEPARATOR = '#';

		public const byte TYPE_SCRIPT = 0;
		public const byte TYPE_ASSET = 1;

		private static GuidDatabaseManager instance;

		private readonly Dictionary<string, string[]> table;
		private readonly Dictionary<string, Type> typeCache;

		private GuidDatabaseManager(IDictionary<string, string[]> table)
		{
			this.table = new Dictionary<string, string[]>(table);
			typeCache = new Dictionary<string, Type>();
		}

		#region Lookup

		/// <summary>
		/// Map a GUID to a Unity Resource Request which will load in the background.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public ResourceRequest MapGuidToObjectAsync(string guid)
		{
			if (string.IsNullOrEmpty(guid))
			{
				throw new ArgumentException("GUID can not be null", nameof(guid));
			}

			string currentPath = MapGuidToPath(guid);

			ResourceRequest resourceRequest = Resources.LoadAsync(currentPath);

			if (resourceRequest == null)
			{
				throw new InvalidOperationException("[MapGUIDToObject] Unable to load: " + guid + " (maps to: " + currentPath);
			}

			return resourceRequest;
		}
		
		/// <summary>
		/// Map a GUID to a Unity Object.
		/// </summary>
		/// <param name="guid">The GUID get the asset for.</param>
		/// <param name="type">The type to cast the resulting object to.</param>
		/// <returns>The asset with that specific GUID.</returns>
		/// <exception cref="ArgumentException">Thrown when either guid or type is null.</exception>
		/// <seealso cref="MapGuidToObject{T}"/>
		public Object MapGuidToObject(string guid, Type type)
		{
			if (string.IsNullOrEmpty(guid))
			{
				throw new ArgumentException("GUID can not be null", nameof(guid));
			}

			if (type == null)
			{
				throw new ArgumentException("Type can not be null", nameof(type));
			}

			string currentPath = MapGuidToPath(guid);
			Object resource = Resources.Load(currentPath, type);

			if (resource == null)
			{
				throw new InvalidOperationException("[MapGUIDToObject] Unable to load: " + guid + " (maps to: " + currentPath + ", type=" + type + ")");
			}

			return resource;
		}

		/// <summary>
		/// Maps a GUID to a UnityObject asset of a specific type T.
		/// </summary>
		/// <param name="guid">The GUID get the asset for.</param>
		/// <returns>The asset with that specific GUID.</returns>
		/// <exception cref="ArgumentException">Thrown when guid is null.</exception>
		/// <seealso cref="MapGuidToObject"/>
		public T MapGuidToObject<T>(string guid) where T : Object
		{
			if (string.IsNullOrEmpty(guid))
			{
				throw new ArgumentException("Guid can not be null", nameof(guid));
			}

			return (T)MapGuidToObject(guid, typeof(T));
		}


		public bool ContainsEntryForTypeGuid(string typeIdentifier)
		{
			if (!string.IsNullOrEmpty(typeIdentifier))
			{
				string[] parts = typeIdentifier.Split(new char[] { TYPE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

				string guid = parts[0];

				if (guid.Contains(TYPE_SEPARATOR.ToString()))
				{
					guid = guid.Substring(0, guid.IndexOf(TYPE_SEPARATOR));
				}

				return table.ContainsKey(guid);
			}

			return false;
		}

		public Type MapGuidToType(string typeIdentifier)
		{
			if (!string.IsNullOrEmpty(typeIdentifier))
			{
				string[] parts = typeIdentifier.Split(new char[] { TYPE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

				string guid = parts[0];
				int index = parts.Length > 1 ? int.Parse(parts[1]) : 0;

				Type[] types = MapGuidToTypes(guid);

				if (index < types.Length)
				{
					return types[index];
				}
			}

			return null;
		}

		/// <summary>
		/// Runtime function to map a serialized GUID to a type.
		/// </summary>
		/// <param name="guid">GUID of a script that contains a class with the same name (the type).</param>
		/// <returns>The type string for that particular GUID.</returns>
		/// <exception cref="ArgumentException">Thrown when guid is null.</exception>
		public Type[] MapGuidToTypes(string guid)
		{
			if (string.IsNullOrEmpty(guid))
			{
				throw new ArgumentException("Guid can not be null", nameof(guid));
			}

			List<Type> result = new List<Type>();

			if (guid.Contains(TYPE_SEPARATOR.ToString()))
			{
				guid = guid.Substring(0, guid.IndexOf(TYPE_SEPARATOR));
			}

			if (table.ContainsKey(guid))
			{
				string[] types = table[guid];

				foreach (string type in types)
				{
					Type value;
					if (typeCache.TryGetValue(type, out value))
					{
						result.Add(value);
					}
					else
					{
						value = Reflect.GetType(type);
						typeCache.Add(type, value);
						result.Add(value);
					}
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Runtime function to map a serialized GUID to the current 
		/// path of the corresponding asset in resources.
		/// </summary>
		/// <param name="guid">GUID of an asset.</param>
		/// <returns>The path for that particular GUID.</returns>
		/// <exception cref="ArgumentException">Thrown when guid is null.</exception>
		public string MapGuidToPath(string guid)
		{
			if (string.IsNullOrEmpty(guid))
			{
				throw new ArgumentException("GUID can not be null", nameof(guid));
			}

			if (table.ContainsKey(guid))
			{
				return table[guid][0];
			}

			throw new InvalidOperationException("[MapGUIDToPath] No path found for GUID: " + guid);
		}
		#endregion

#if UNITY_EDITOR
		#region Editor Lookup
		/// <summary>
		/// <para>Function does not exist at runtime.</para>
		/// Maps a UnityObject asset to the corresponding GUID.
		/// </summary>
		/// <param name="asset">The asset to get the GUID for.</param>
		/// <returns>The asset's assigned GUID.</returns>
		/// <exception cref="ArgumentException">Thrown when asset is null.</exception>
		public string MapAssetToGuid(Object asset)
		{
			if (!asset)
			{
				throw new ArgumentException("Asset can not be null", nameof(asset));
			}

			string path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
			return MapAssetPathToGuid(path);
		}

		/// <summary>
		/// <para>Function does not exist at runtime.</para>
		/// Maps a UnityObject asset to the corresponding GUID.
		/// </summary>
		/// <param name="path">The asset path to get the GUID for.</param>
		/// <returns>The asset's assigned GUID.</returns>
		/// <exception cref="ArgumentException">Thrown when path is null.</exception>
		public string MapAssetPathToGuid(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Path can not be null", nameof(path));
			}

			return AssetDatabase.AssetPathToGUID(path);
		}


		/// <summary>
		/// <para>Function does not exist at runtime.</para>
		/// Maps a Type to the GUID of it's containing script.
		/// </summary>
		/// <param name="type">The type to get the GUID for.</param>
		/// <returns>The guid for that script.</returns>
		/// <exception cref="ArgumentException">Thrown when type is null.</exception>
		public string MapTypeToGuid(Type type)
		{
			if (type == null)
			{
				throw new ArgumentException("Type can not be null", nameof(type));
			}

			return MapTypeToGuid(type.FullName);
		}

		/// <summary>
		/// <para>Function does not exist at runtime.</para>
		/// Maps a Type to the GUID of it's containing script.
		/// </summary>
		/// <param name="fullName">The typename to get the GUID for.</param>
		/// <returns>The guid for that script.</returns>
		public string MapTypeToGuid(string fullName)
		{
			if (string.IsNullOrEmpty(fullName))
			{
				throw new ArgumentException("FullName can not be null", nameof(fullName));
			}

			string result = LookForType(fullName);

			if (result == null)
			{
				LogUtil.Error(LogTags.INPUT, this, "The GuidDatabaseManager cannot find the GUID" +
					" mapped to the specified type with fullName " + fullName + " , try reimporting the script asset.");
				return "";
			}

			return result;
		}
		#endregion

		#region Database

		/// <summary>
		/// <para>Function does not exist at runtime.</para>
		/// Create an asset. This is called automatically by AssetPostProcessWatcher
		/// whenever an asset has been created.
		/// </summary>
		/// <param name="path">The path of the imported asset.</param>
		public void Import(string path)
		{
			if (!IsValidAssetPath(path, true))
			{
				return;
			}

			path = path.Substring("Assets/".Length);
			GuidResourceFile file = GuidDatabaseUtility.CreateLookup(path);

			if (file.Exists)
			{
				if (table.ContainsKey(file.Guid))
				{
					table[file.Guid] = file.Identifier.Split(new char[] { TYPE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
				}
				else
				{
					table.Add(file.Guid, file.Identifier.Split(new char[] { TYPE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries));
				}
			}
		}

		/// <summary>
		/// <para>Function does not exist at runtime.</para>
		/// Update the path of an asset. This is called automatically by AssetPostProcessWatcher
		/// whenever an asset has been moved.
		/// </summary>
		/// <param name="src">The source path of the asset.</param>
		/// <param name="dest">The new path of the asset.</param>
		public void Update(string src, string dest)
		{
			if (IsValidAssetPath(dest))
			{
				Import(dest);
			}
			else
			{
				if (IsValidAssetPath(src))
				{
					dest = dest.Substring("Assets/".Length);
					GuidResourceFile file = GuidDatabaseUtility.DeleteLookup(dest);

					if (file.Exists)
					{
						table.Remove(file.Guid);
					}
				}
			}
		}

		/// <summary>
		/// <para>Function does not exist at runtime.</para>
		/// Delete an asset. This is called automatically by AssetPostProcessWatcher
		/// whenever an asset has been deleted.
		/// </summary>
		/// <param name="path">The path of the deleted asset.</param>
		public void Delete(string path)
		{
			if (!IsValidAssetPath(path))
			{
				return;
			}

			path = path.Substring("Assets/".Length);
			GuidResourceFile file = GuidDatabaseUtility.DeleteLookup(path);

			if (file.Exists)
			{
				table.Remove(file.Guid);
			}
		}

		/// <summary>
		/// <para>Function does not exist at runtime.</para>
		/// Check whether or not the asset at path should be tracked.
		/// </summary>
		/// <param name="path">The path of the asset to check.</param>
		/// <returns>True if the asset should be tracked.</returns>
		public static bool IsValidAssetPath(string path, bool showDialogs = false)
		{
			// It's a folder
			if (AssetDatabase.IsValidFolder(path))
			{
				return false;
			}

			int type = path.EndsWith(CS_EXTENSION) ? TYPE_SCRIPT : TYPE_ASSET;
			bool res = false;

			// It's a script file
			if (type == TYPE_SCRIPT)
			{
				// It's an editor script
				if (path.Contains("/Editor/"))
				{
					return false;
				}

				res = true;
			}

			// It's an asset file
			if (type == TYPE_ASSET)
			{
				// It's not in a resources folder
				if (!path.Contains("/" + RESOURCES_FOLDER))
				{
					return false;
				}

				// It resides in ProjectSettings/
				if (path.Contains("/ProjectSettings/"))
				{
					return false;
				}

				// It's a built-in Unity resource
				if (path.Contains("com.unity."))
				{
					return false;
				}

				res = true;
			}

			return res;
		}

		private string LookForType(string fullName)
		{
			foreach (KeyValuePair<string, string[]> kvp in table)
			{
				string[] types = kvp.Value;

				for (int i = 0; i < types.Length; i++)
				{
					if (types[i] == fullName)
					{
						return kvp.Key + TYPE_SEPARATOR + i;
					}
				}
			}

			return null;
		}

		#endregion
#endif

#if UNITY_EDITOR
		private static Dictionary<string, string[]> LoadEditorLookupFiles()
		{
			IEnumerable<GuidResourceFile> files = GuidDatabaseUtility.LoadDatabase();

			Dictionary<string, string[]> lookups = new Dictionary<string, string[]>();

			foreach (GuidResourceFile file in files)
			{
				lookups.Add(file.Guid, file.Identifier.Split(new char[] { TYPE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries));
			}

			return lookups;
		}

		[MenuItem("Utils/Asset Database/Refresh Asset Database")]
		public static void ForceDatabaseRefresh()
		{
			instance = null;

			AssetDatabase.Refresh();

			string[] assets = AssetDatabase.GetAllAssetPaths();

			foreach (string asset in assets)
			{
				if (asset.StartsWith("Assets/"))
				{
					string guid = AssetDatabase.AssetPathToGUID(asset);

					if (IsValidAssetPath(asset) && !Instance.table.ContainsKey(guid))
					{
						Instance.Import(asset);
					}
				}
			}
		}

		[MenuItem("Utils/Asset Database/Rebuild Asset Database")]
		public static void ForceDatabaseRebuild()
		{
			string src = GuidDatabaseUtility.RootPath;
			string backup = GuidDatabaseUtility.RootPath + "-Backup";

			if (Directory.Exists(backup))
			{
				Directory.Delete(backup, true);
			}

			if (Directory.Exists(src))
			{
				Directory.Move(src, backup);
			}

			ForceDatabaseRefresh();
		}

		[MenuItem("Utils/Asset Database/Create Runtime Library", true)]
		private static bool CreateRuntimeLibraryValidate()
		{
			return BUILD_LOOKUP_FILE_NAME.LoadIfAvailable<GuidDatabaseObject>() == null;
		}

		[MenuItem("Utils/Asset Database/Create Runtime Library")]
		public static void CreateRuntimeLibrary()
		{
			GuidDatabaseObject lookup = ScriptableObject.CreateInstance<GuidDatabaseObject>();
			lookup.Assets = new List<GuidDatabaseObject.Asset>();

			foreach (KeyValuePair<string, string[]> asset in Instance.table)
			{
				lookup.Assets.Add(new GuidDatabaseObject.Asset(asset.Key, asset.Value));
			}

			FileUtility.EnsurePath(BUILD_LOOKUP_FILE_PATH);

			AssetDatabase.CreateAsset(lookup, BUILD_LOOKUP_FILE_PATH + BUILD_LOOKUP_FILE_NAME + ".asset");
			AssetDatabase.ImportAsset(BUILD_LOOKUP_FILE_PATH + BUILD_LOOKUP_FILE_NAME + ".asset", ImportAssetOptions.ForceUpdate);

			LogUtil.Log(LogTags.DATA, "Created run-time library: " + BUILD_LOOKUP_FILE_PATH + BUILD_LOOKUP_FILE_NAME + ".asset");
		}

		[MenuItem("Utils/Asset Database/Delete Runtime Library", true)]
		private static bool DeleteRuntimeLibraryValidate()
		{
			return BUILD_LOOKUP_FILE_NAME.LoadIfAvailable<GuidDatabaseObject>() != null;
		}

		[MenuItem("Utils/Asset Database/Delete Runtime Library")]
		public static void DeleteRuntimeLibrary()
		{
			AssetDatabase.DeleteAsset(BUILD_LOOKUP_FILE_PATH + BUILD_LOOKUP_FILE_NAME + ".asset");
			AssetDatabase.Refresh();

			LogUtil.Log(LogTags.DATA, "Deleted run-time library: " + BUILD_LOOKUP_FILE_PATH + BUILD_LOOKUP_FILE_NAME + ".asset");
		}
#endif
	}
}
