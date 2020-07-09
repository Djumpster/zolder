// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Tracking
{
	public delegate void StatChangedHandler(string id, string type, float oldAmount, float newAmount);

	/// <summary>
	/// The stat logger can log statistics (as floats) by specified "id" labels.
	/// They can be grouped by an arbitrary "type" label.
	/// It dispatches an event when any tracked id changes.
	/// </summary>
	public interface IStatLogger
	{
		event StatChangedHandler StatChangedEvent;

		/// <summary>
		/// Logs change of item with given id.
		/// The change can also be a substraction.
		/// </summary>
		/// <param name="id">A unique identifier for the item. Example: ApplesCollected, KeysUsed, etc.</param>
		/// <param name="type">The type of item. Used to specify a collection of items. Example: Fruit. 
		/// All fruit stats can then be retrieved in a list.</param>
		/// <param name="change">The amount of change. Can be positive or negative.</param>
		/// <param name="isNewValue">Should the change amount be added or should it be the new value?</param>
		void Log(string id, string type = "None", float change = 1f, bool isNewValue = false);

		/// <summary>
		/// Returns stat for the given id. 
		/// Returns 0 if the id does not yet exist.
		/// </summary>
		/// <returns>The stats.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="type">Type.</param>
		float GetStat(string id, string type = "None");

		Dictionary<string, float> GetStatsOfType(string type);

		float GetTotalForStatsOfType(string type);

		void ResetStat(string id, string type = "None");

		void ResetAllStatsOfType(string type);

		void Save();
	}
}
