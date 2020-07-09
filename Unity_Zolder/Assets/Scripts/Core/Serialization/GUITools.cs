// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Serialization
{
	public static class GUITools
	{
		public enum Command
		{
			Copy,
			Cut,
			Paste,
			Delete,
			FrameSelected,
			Duplicate,
			SelectAll,
			SoftDelete
		}

		public static bool IsCommand(Command command)
		{
			string com = command.ToString();

			if (Event.current.commandName == com && Event.current.type == EventType.ValidateCommand)
			{
				Event.current.Use();
			}
			if (Event.current.commandName == com && Event.current.type == EventType.ExecuteCommand)
			{
				Event.current.Use();
				return true;
			}
			return false;
		}
	}
}