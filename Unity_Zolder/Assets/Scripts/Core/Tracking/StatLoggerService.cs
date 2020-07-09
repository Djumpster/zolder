// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Storage;

namespace Talespin.Core.Foundation.Tracking
{
	/// <summary>
	/// The stat logger can log statistics (as floats) by specified "id" labels.
	/// They can be grouped by an arbitrary "type" label.
	/// It dispatches an event when any tracked id changes.
	/// </summary>
	public class StatLoggerService : IStatLogger
	{
		public event StatChangedHandler StatChangedEvent;

		private DataPacket loggedData;
		private LocalDataManager localDataManager;

		public StatLoggerService(DataPacket loggedData, LocalDataManager localDataManager)
		{
			this.loggedData = loggedData;
			this.localDataManager = localDataManager;
		}

		/// <summary>
		/// Logs change of item with given id.
		/// The change can also be a substraction.
		/// </summary>
		/// <param name="id">A unique identifier for the item. Example: ApplesCollected, KeysUsed, etc.
		/// It cannot use the ~ symbol, this is a reserved delimiter.</param>
		/// <param name="change">The amount of change. Can be positive or negative.</param>
		/// <param name="isNewValue">Should the change amount be added or should it be the new value?</param>
		public void Log(string id, float change, bool isNewValue = false)
		{
			Log(id, "None", change, isNewValue);
		}

		/// <summary>
		/// Logs change of item with given id.
		/// The change can also be a substraction.
		/// </summary>
		/// <param name="id">A unique identifier for the item. Example: ApplesCollected, KeysUsed, etc.
		/// It cannot use the ~ symbol, this is a reserved delimiter.</param>
		/// <param name="type">The type of item. Used to specify a collection of items. Example: Fruit. 
		/// All fruit stats can then be retrieved in a list.
		/// It cannot use the ~ symbol, this is a reserved delimiter.</param>
		/// <param name="change">The amount of change. Can be positive or negative.</param>
		/// <param name="isNewValue">Should the change amount be added or should it be the new value?</param>
		public void Log(string id, string type = "None", float change = 1f, bool isNewValue = false)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id cannot be null or empty.");
			}

			if (id.Contains("~") || type.Contains("~"))
			{
				throw new InvalidOperationException("Usage of ~ is not permitted in id or type.");
			}

			DataContainer typeContainer = loggedData.GetDataContainer(type, new DataContainer());
			float currentAmount = typeContainer.GetFloat(id, 0);
			float oldAmount = currentAmount;
			if (isNewValue)
			{
				currentAmount = change;
			}
			else
			{
				currentAmount += change;
			}

			typeContainer.Set(id, currentAmount);

			//LogUtility.Log(this.GetType().Name + ": Logged stat with id: " + id + ", type: " + type + ", old value: " + 
			//          oldAmount + ", new value: " + currentAmount);

			DispatchStatChangedEvent(id, type, oldAmount, currentAmount);
		}

		/// <summary>
		/// Returns stat for the given id. 
		/// Returns 0 if the id does not yet exist.
		/// </summary>
		/// <returns>The stats.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="type">Type.</param>
		public float GetStat(string id, string type = "None")
		{
			DataContainer typeContainer = loggedData.GetDataContainer(type, new DataContainer());
			return typeContainer.GetFloat(id, 0);
		}

		public Dictionary<string, float> GetStatsOfType(string type)
		{
			Dictionary<string, float> foundStats = new Dictionary<string, float>();

			DataContainer typeContainer = loggedData.GetDataContainer(type, new DataContainer());
			string[] keys = typeContainer.GetKeys();
			foreach (string key in keys)
			{
				foundStats.Add(key, typeContainer.GetFloat(key));
			}

			return foundStats;
		}

		public float GetTotalForStatsOfType(string type)
		{
			DataContainer typeContainer = loggedData.GetDataContainer(type, new DataContainer());
			string[] keys = typeContainer.GetKeys();
			float totalValue = 0;
			foreach (string key in keys)
			{
				totalValue += typeContainer.GetFloat(key);
			}

			return totalValue;
		}

		public void ResetStat(string id, string type = "None")
		{
			DataContainer typeContainer = loggedData.GetDataContainer(type, new DataContainer());
			float oldAmount = typeContainer.GetFloat(id, 0);
			typeContainer.RemoveKey(id);

			DispatchStatChangedEvent(id, type, oldAmount, 0);
		}

		public void ResetAllStatsOfType(string type)
		{
			Dictionary<string, float> idsReset = new Dictionary<string, float>();

			DataContainer typeContainer = loggedData.GetDataContainer(type, new DataContainer());
			string[] keys = typeContainer.GetKeys();
			foreach (string key in keys)
			{
				idsReset.Add(key, typeContainer.GetFloat(key));
			}
			typeContainer.Clear();

			foreach (string key in idsReset.Keys)
			{
				DispatchStatChangedEvent(key, type, idsReset[key], 0);
			}
		}

		public void Save()
		{
			localDataManager.Save(loggedData);
		}

		private void DispatchStatChangedEvent(string id, string type, float oldAmount, float newAmount)
		{
			StatChangedEvent?.Invoke(id, type, oldAmount, newAmount);
		}
	}
}
