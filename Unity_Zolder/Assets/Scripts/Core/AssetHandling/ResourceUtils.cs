// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.AssetHandling
{
	public static class ResourceUtils
	{
		private const string RESOURCE_FOLDER_NAME = "resources";

		public static string GetResourcePath(Object input)
		{
			return GetPathRelativeTo(input, RESOURCE_FOLDER_NAME);
		}

		public static string GetPathRelativeTo(Object input, string referencePath)
		{
			if (input == null)
			{
				return null;
			}

			string path = AssetDatabase.GetAssetPath(input);
			if (!string.IsNullOrEmpty(path))
			{
				bool inReferenceFolder = false;
				string newPath = string.Empty;

				char seperatorChar;
#if UNITY_EDITOR_WIN
				seperatorChar = '/';
#else
				seperatorChar = Path.DirectorySeparatorChar;
#endif
				foreach (string item in path.Split(seperatorChar))
				{
					if (inReferenceFolder)
					{
						if (!string.IsNullOrEmpty(newPath))
						{
							newPath += seperatorChar;
						}

						newPath += item;
					}
					if (item.Equals(referencePath, System.StringComparison.InvariantCultureIgnoreCase))
					{
						inReferenceFolder = true;
					}
				}
				if (inReferenceFolder)
				{
					return StripExtension(newPath);
				}
			}
			return null;
		}

		public static string StripExtension(string path)
		{
			string extension = Path.GetExtension(path);
			if (string.IsNullOrEmpty(extension))
			{
				return path;
			}

			return path.Substring(0, path.Length - extension.Length);
		}
	}
}
#endif