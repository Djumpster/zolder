// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Settings
{
#if UNITY_EDITOR && !DONT_BOOTSTRAP_SERVICES
	/// <summary>
	/// Used to add a menu bar in the editor to set the quality.
	/// Adds checkboxes for current selection.
	/// </summary>
	[UnityEditor.InitializeOnLoad]
	public static class GraphicsSettingsServiceEditorUtils
	{
		private const string ITEM_PATH_BASE = "Edit/Graphic Settings/";
		private const string ITEM_PATH_LOW = ITEM_PATH_BASE + "Low";
		private const string ITEM_PATH_MEDIUM = ITEM_PATH_BASE + "Medium";
		private const string ITEM_PATH_HIGH = ITEM_PATH_BASE + "High";
		private const string ITEM_PATH_CUSTOM = ITEM_PATH_BASE + "Custom";

		private const int ITEM_PATH_BASE_PRIORITY = 250;

		static GraphicsSettingsServiceEditorUtils()
		{
			if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			{
				UnityEditor.EditorApplication.delayCall += () => EditorUpdateCheckedItem();
			}
		}

		[UnityEditor.MenuItem(ITEM_PATH_LOW, priority = ITEM_PATH_BASE_PRIORITY)]
		public static void SetLow()
		{
			GraphicsSettingsService graphicsSettingsService = GlobalDependencyLocator.Instance.Get<GraphicsSettingsService>();
			graphicsSettingsService.CurrentSetting = GraphicsMode.LOW;
			EditorUpdateCheckedItem();
		}

		[UnityEditor.MenuItem(ITEM_PATH_MEDIUM, priority = ITEM_PATH_BASE_PRIORITY + 1)]
		public static void SetMedium()
		{
			GraphicsSettingsService graphicsSettingsService = GlobalDependencyLocator.Instance.Get<GraphicsSettingsService>();
			graphicsSettingsService.CurrentSetting = GraphicsMode.MEDIUM;
			EditorUpdateCheckedItem();
		}

		[UnityEditor.MenuItem(ITEM_PATH_HIGH, priority = ITEM_PATH_BASE_PRIORITY + 2)]
		public static void SetHigh()
		{
			GraphicsSettingsService graphicsSettingsService = GlobalDependencyLocator.Instance.Get<GraphicsSettingsService>();
			graphicsSettingsService.CurrentSetting = GraphicsMode.HIGH;
			EditorUpdateCheckedItem();
		}

		[UnityEditor.MenuItem(ITEM_PATH_CUSTOM, priority = ITEM_PATH_BASE_PRIORITY + 4)]
		public static void SetCustom()
		{
			GraphicsSettingsService graphicsSettingsService = GlobalDependencyLocator.Instance.Get<GraphicsSettingsService>();
			graphicsSettingsService.CurrentSetting = GraphicsMode.CUSTOM;
			EditorUpdateCheckedItem();
		}

		[UnityEditor.MenuItem(ITEM_PATH_LOW, true)]
		private static bool ValidateLow()
		{
			return Application.isPlaying;
		}

		[UnityEditor.MenuItem(ITEM_PATH_MEDIUM, true)]
		private static bool ValidateMedium()
		{
			return Application.isPlaying;
		}

		[UnityEditor.MenuItem(ITEM_PATH_HIGH, true)]
		private static bool ValidateHigh()
		{
			return Application.isPlaying;
		}

		[UnityEditor.MenuItem(ITEM_PATH_CUSTOM, true)]
		private static bool ValidateCustom()
		{
			return Application.isPlaying;
		}

		private static void EditorUpdateCheckedItem()
		{
			GraphicsSettingsService graphicsSettingsService = GlobalDependencyLocator.Instance.Get<GraphicsSettingsService>();
			UnityEditor.Menu.SetChecked(ITEM_PATH_LOW, graphicsSettingsService.CurrentSetting == GraphicsMode.LOW);
			UnityEditor.Menu.SetChecked(ITEM_PATH_MEDIUM, graphicsSettingsService.CurrentSetting == GraphicsMode.MEDIUM);
			UnityEditor.Menu.SetChecked(ITEM_PATH_HIGH, graphicsSettingsService.CurrentSetting == GraphicsMode.HIGH);
			UnityEditor.Menu.SetChecked(ITEM_PATH_CUSTOM, graphicsSettingsService.CurrentSetting == GraphicsMode.CUSTOM);
		}
	}
#endif
}