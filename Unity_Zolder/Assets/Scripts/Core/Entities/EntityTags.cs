// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Entities
{
	public class EntityTags : EntityTagsBase
	{
		public const string PLAYER = "Player";
		public const string AI = "AI";
		public const string CAMERA = "CAMERA";
		public const string PLAYER_FACTION = "PLAYER_FACTION";
		public const string OPPOSING_FACTION = "OPPOSING_FACTION";
		public const string INPUT_RECEIVER = "INPUT_RECEIVER";
		public const string UPGRADE = "UPGRADE";
		public const string PICKUP = "PICKUP";

		public readonly static HostilityTable Hostility = new HostilityTable(new string[] { PLAYER_FACTION }, new string[] { OPPOSING_FACTION });
	}
}
