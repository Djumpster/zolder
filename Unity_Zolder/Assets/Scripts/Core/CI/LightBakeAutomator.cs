// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using Talespin.Core.Foundation.Logging;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Talespin.Core.Foundation.CI
{
	public static class LightBakeAutomator
	{
		[MenuItem("Jenkins/Bake Lightmaps")]
		public static void Bake()
		{
			string[] scenePaths = GetEnabledScenes();
			string scenes;

			if (CommandLineUtils.TryGetString("scenes", out scenes))
			{

				string[] sceneNames = scenes.Split(',');
				List<string> paths = new List<string>();

				foreach (string sceneName in sceneNames)
				{
					foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
					{
						if (scene.path.EndsWith("/" + sceneName + ".unity"))
						{
							paths.Add(scene.path);
						}
					}
				}

				scenePaths = paths.ToArray();
			}

			LogTargetScenes(scenePaths);

			foreach (string scene in scenePaths)
			{
				LogUtil.Log(LogTags.CI, "Start Baking Scene: " + scene);

				EditorSceneManager.OpenScene(scene, OpenSceneMode.Single);

				if (Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.OnDemand)
				{
					bool result = Lightmapping.Bake();

					if (!result)
					{
						throw new Exception(LogTags.CI + ": Failed to build lightmap data");
					}

					LogUtil.Log(LogTags.CI, "Finish Baking Scene: " + scene);
				}
				else
				{
					LogUtil.Log(LogTags.CI, "Skipping scene because GI workflow mode is not set to OnDemand");
				}
			}

			LogUtil.Log(LogTags.CI, "Finished light bake");
		}

		private static void LogTargetScenes(string[] scenes)
		{
			StringBuilder log = new StringBuilder();

			log.AppendLine("[LightBakeAutomator] Scenes = {");

			for (int i = 0; i < scenes.Length; i++)
			{
				log.Append("\t");
				log.Append(scenes[i]);

				if (i + 1 < scenes.Length)
				{
					log.AppendLine(",\n");
				}
			}

			log.AppendLine("\n}");

			LogUtil.Log(LogTags.CI, log.ToString());
		}

		private static string[] GetEnabledScenes()
		{
			List<string> scenes = new List<string>();

			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if (!scene.enabled)
				{
					continue;
				}

				scenes.Add(scene.path);
			}

			return scenes.ToArray();
		}
	}
}
#endif