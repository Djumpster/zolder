// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Talespin.Core.Foundation.AssetHandling
{
	/// <summary>
	/// Some utility functions for asset manipulation in editor scripts.
	/// </summary>
	public static class AssetUtils
	{
		public static string GetAssetPathBasedOnSelection(string fileName)
		{
			Object selectedObject = Selection.objects.FirstOrDefault();
			string path = "Assets";
			if (selectedObject != null)
			{
				string selectedPath = AssetDatabase.GetAssetPath(selectedObject);
				if (!string.IsNullOrEmpty(selectedPath))
				{
					if (Directory.Exists(selectedPath))
					{
						path = selectedPath;
					}
					else
					{
						path = Path.GetDirectoryName(selectedPath);
					}
				}
			}
			path += string.Format("/{0}.asset", fileName);
			path = AssetDatabase.GenerateUniqueAssetPath(path);
			return path;
		}

		public static T CreateAsset<T>() where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			string path = GetAssetPathBasedOnSelection("unnamed" + typeof(T));
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
			return asset;
		}

		public static T[] LoadAllAssetsByType<T>() where T : Object
		{
			List<T> result = new List<T>();
			Type type = typeof(T);
			string[] guids = AssetDatabase.FindAssets("t:" + type.Name);
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				T asset = AssetDatabase.LoadAssetAtPath(path, type) as T;
				if (asset != null)
				{
					result.Add(asset);
				}
			}
			return result.ToArray();
		}
	}
}
#endif