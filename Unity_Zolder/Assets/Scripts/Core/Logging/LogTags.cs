// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Logging
{
	/// <summary>
	/// The tags to be used for filtering using ConsolePro.
	/// </summary>
	public class LogTags : LogTagsBase
	{
		// core
		/// <summary>
		/// Any non-game-specific but core system.
		/// </summary>
		public const string SYSTEM = "[System]";

		public const string FLOW = "[Flow]";
		public const string DATA = "[Data]";
		public const string NETWORK = "[Network]";
		public const string SOCIAL = "[Social]";
		public const string AUDIO = "[Audio]";
		public const string UI = "[UI]";
		public const string GRAPHICS = "[Graphics]";
		public const string INPUT = "[Input]";

		/// <summary>
		/// The game specific core, not the non-game-specific framework components.
		/// </summary>
		public const string GAME = "[Game]";

		// etc.
		public const string NOTIFICATIONS = "[Notifications]";
		public const string LOCALIZATION = "[Localization]";
		public const string ANALYTICS = "[Analytics]";
		public const string ADS = "[Ads]";
		public const string MONETIZATION = "[Monetization]";
		public const string RETENTION = "[Retention]";
		public const string ACHIEVEMENTS = "[Achievements]";
		public const string ANIMATION = "[Animation]";
		public const string OBJECTIVE = "[Objectives]";
		public const string CI = "[CI]";
		public const string WEBVIEW = "[Webview]";

		// 3rd party plugins not included in previous tags
		public const string PLUGIN = "[Plugin]";

		// Testing
		public const string TEST = "[Test]";
	}
}
