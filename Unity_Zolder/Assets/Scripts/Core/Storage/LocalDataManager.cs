// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Maths;
using UnityEngine;

namespace Talespin.Core.Foundation.Storage
{
	public class LocalDataManager : IDisposable
	{
		public delegate void LocalDataSavedHandler();
		public event LocalDataSavedHandler OnLocalDataSaved = delegate { };

		#region properties
		public const string DEFAULT_DATA_ID = "guestData";

		public IEnumerable<DataPacket> DataPackets { get { return dataPackets; } }
		private List<DataPacket> dataPackets;

		/// <summary>
		/// The data ID is used to be able to store data per ID.
		/// It can be used for storing separate data, say, per user.
		/// </summary>
		/// <value>The data ID.</value>
		public string DataID { get { return dataID; } }
		private string dataID;

		#endregion

		#region members
		public const int SAVE_VERSION = 0;
		private const string MAIN = "^";
		private const string MANIFEST = "DataManifest";
		private const string HKEY = "-";
		private const string VKEY = "|";
		private const string SKEY = "=";
		public static int Sequence = 0;

		private string hashKey;

		private bool hasBeenDisposed = false;

		#endregion

		#region constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Talespin.Core.Data.LocalDataManager"/> class.
		/// </summary>
		/// <param name="hashKey">The key that used to sign the data in order to prevent data tempering.</param>
		public LocalDataManager(ConfigDataSaveGame configData)
		{
			dataID = PlayerPrefs.GetString("dataID", DEFAULT_DATA_ID);
			if (configData == null)
			{
				LogUtil.Warning(LogTags.DATA, this, "ConfigDataSaveGame Configuration is missing");
				hashKey = "";
			}
			else
			{
				hashKey = configData.DataHashKey;
			}
			dataPackets = Load();
		}

		#endregion

		#region public methods
		/// <summary>
		/// Changes the ID that should be used when saving and loading data to storage.
		/// This will require you to re-initialize the LocalDataManager unless you choose to migrate the current data
		/// to the new ID. In that case the current data remains valid and usable.
		/// </summary>
		/// <param name="newDataID">The new data ID to be used.</param>
		/// <param name="migrateOldDataIfNewIDEmpty">If set to <see langword="true" /> the old data will be saved under the new
		/// ID if there is no data there yet and the old data will be removed.</param>
		/// <param name="dataMigrated">Was the old data migrated to the new dataID?</param>
		public void SwitchDataID(string newDataID, bool migrateOldDataIfNewIDEmpty, out bool dataMigrated)
		{
			if (newDataID == dataID)
			{
				dataMigrated = true;
				return;
			}

			PlayerPrefs.SetString("dataID", newDataID);

			string savedManifestString = PlayerPrefs.GetString(newDataID + MANIFEST);
			string[] identifiers = savedManifestString.Split(new string[] { "||||" }, StringSplitOptions.None);
			bool newDataIDEmpty = string.IsNullOrEmpty(savedManifestString) || identifiers.Length == 0;
			bool migrateData = migrateOldDataIfNewIDEmpty && newDataIDEmpty;
			dataMigrated = migrateData;

			if (migrateOldDataIfNewIDEmpty && !migrateData)
			{
				LogUtil.Warning(LogTags.DATA, this, "Could not migrate data, existing data was already found.");
			}

			foreach (DataPacket dataPacket in dataPackets)
			{
				if (migrateOldDataIfNewIDEmpty)
				{
					DeletePlayerPrefsEntries(dataPacket);
				}

				if (migrateData)
				{
					dataPacket.IsDirty = true;
				}
			}

			dataID = newDataID;

			if (migrateData)
			{
				Save();
			}
			else
			{
				dataPackets.Clear();
			}

			if (!newDataIDEmpty)
			{
				dataPackets = Load();
			}
		}

		public bool ContainsDataPacket(string identifier)
		{
			foreach (DataPacket dataPacket in dataPackets)
			{
				if (dataPacket.Identifier == identifier)
				{
					return true;
				}
			}

			return false;
		}

		public DataPacket GetDataPacket(string identifier, bool createIfNull = false)
		{
			foreach (DataPacket dataPacket in dataPackets)
			{
				if (dataPacket.Identifier == identifier)
				{
					return dataPacket;
				}
			}

			if (createIfNull)
			{
				DataPacket dataPacket = new DataPacket(identifier);
				AddDataPacket(dataPacket);
				return dataPacket;
			}

			return null;
		}

		public void AddDataPacket(DataPacket dataPacket)
		{
			if (dataPackets.Contains(dataPacket))
			{
				return;
			}

			if (ContainsDataPacket(dataPacket.Identifier))
			{
				throw new InvalidOperationException(LogTags.DATA + " Attempting to add data packet with identifier " +
					dataPacket.Identifier + " but another data packet with that identifier already exists!");
			}

			dataPackets.Add(dataPacket);

			dataPacket.IsDirty = true;
		}

		public void UpdateDataPacket(DataPacket dataPacket, bool addIfNoData)
		{
			int id = -1;
			for (int i = 0; i < dataPackets.Count; i++)
			{
				if (dataPackets[i].Identifier == dataPacket.Identifier)
				{
					id = i;
					break;
				}
			}

			if (id >= 0)
			{
				dataPackets[id] = dataPacket;
				dataPackets[id].IsDirty = true;
				dataPackets[id].IsHierarchyDirty = true;
			}
			else
			{
				if (addIfNoData)
				{
					AddDataPacket(dataPacket);
				}
			}
		}

		public void RemoveDataPacket(DataPacket dataPacket)
		{
			dataPackets.Remove(dataPacket);

			if (hasBeenDisposed)
			{
				LogUtil.Warning(LogTags.DATA, this, "Attempting to save data but this LocalDataManager instance has " +
					"been disposed!");
				return;
			}

			DeletePlayerPrefsEntries(dataPacket);
		}

		public void Save(DataPacket dataPacketToSave)
		{
			Save(new List<DataPacket> { dataPacketToSave });
		}

		/// <summary>
		/// Save the specified packets, leave null for all dirty packets.
		/// </summary>
		/// <param name="packets">Packets.</param>
		public void Save(List<DataPacket> packets = null)
		{
			if (hasBeenDisposed)
			{
				LogUtil.Warning(LogTags.DATA, this, "Attempting to save data but this LocalDataManager instance has " +
					"been disposed!");
				return;
			}

			if (packets != null)
			{
				foreach (DataPacket dp in packets)
				{
					if (dataPackets.Contains(dp) == false)
					{
						dataPackets.Add(dp);
					}
				}
			}
			else
			{
				LogUtil.Log(LogTags.DATA, this, "Saving ALL packets for data ID " + dataID + ", this could cause" +
					" a lag spike. It is recommended to pass specific packet(s) you want to save to make the spike smaller.");
				packets = dataPackets;
			}

			RemoveMissingIdentifiersFromPlayerPrefs();

			List<DataPacket> dirtyPackets = GetDirtyPackets(packets);

			int count = 0;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < dirtyPackets.Count; i++)
			{
				DataPacket dataPacket = dirtyPackets[i];

				if (dataPacket.Count == 0)
				{
					dataPacket.IsHierarchyDirty = false;
					PlayerPrefs.DeleteKey(dataID + MAIN + dataPacket.Identifier);
					PlayerPrefs.DeleteKey(dataID + HKEY + dataPacket.Identifier);
					continue;
				}

				count++;

				string dataString = DataContainerJSONSerializer.Serialize(dataPacket);
				string hash = Encryption.Hash(dataString, hashKey);
				PlayerPrefs.SetString(dataID + MAIN + dataPacket.Identifier, dataString);
				PlayerPrefs.SetString(dataID + HKEY + dataPacket.Identifier, hash);
				dataPacket.IsHierarchyDirty = false;
				sb.Append(dataPacket.Identifier);

				sb.Append(", ");
			}

			if (count > 0)
			{
				sb.Remove(sb.Length - 2, 2);
			}

			PlayerPrefs.SetString(dataID + MANIFEST, GenerateManifestString());

			PlayerPrefs.SetInt(dataID + SKEY, Sequence);
			PlayerPrefs.SetInt(dataID + VKEY, SAVE_VERSION);

			PlayerPrefs.Save();

			OnLocalDataSaved();
		}

		public List<DataPacket> GetDataPacketsFromString(string manifestString)
		{
			float startTime = Time.realtimeSinceStartup;

			//string manifestString = PlayerPrefs.GetString(this.dataID + MANIFEST);
			string[] identifiers = manifestString.Split(new string[] { "||||" }, StringSplitOptions.None);

			List<DataPacket> dataPackets = new List<DataPacket>();

			if (identifiers.Length == 0)
			{
				LogUtil.Warning(LogTags.DATA, this, "No save data found.");
				return dataPackets;
			}

			foreach (string identifier in identifiers)
			{
				if (string.IsNullOrEmpty(identifier))
				{
					continue;
				}

				string m = PlayerPrefs.GetString(dataID + MAIN + identifier);
				string k = PlayerPrefs.GetString(dataID + HKEY + identifier);
				string h = Encryption.Hash(m, hashKey);

				if (!string.IsNullOrEmpty(m) && h == k)
				{
					DataContainer data = DataContainerJSONSerializer.Deserialize(m);

					if (data == null || data.GetKeys() == null)
					{
						throw new InvalidOperationException(LogTags.DATA + " Invalid JSON string: " + m);
					}

					DataPacket packet = new DataPacket(identifier, data);
					dataPackets.Add(packet);
					packet.IsHierarchyDirty = false;
				}
				else
				{
					LogUtil.Warning(LogTags.DATA, this, "No save data found for id: " + identifier + ", string: " + m);
				}
			}

			Sequence = PlayerPrefs.GetInt(dataID + SKEY, 0);
			int v = PlayerPrefs.GetInt(dataID + VKEY);

			if (v != SAVE_VERSION)
			{
				dataPackets = ConvertVersion(dataPackets, v, SAVE_VERSION);
			}

			float endTime = Time.realtimeSinceStartup;

			LogUtil.Log(LogTags.DATA, this, "Finished loading " + identifiers.Length +
				" identifiers, took " + (endTime - startTime) + " seconds, generated " + dataPackets.Count + " packets.");

			return dataPackets;
		}

		public void Dispose()
		{
			hasBeenDisposed = true;
		}

#if UNITY_EDITOR
		public void Reload()
		{
			if (Application.isPlaying == false)
			{
				dataPackets = Load();
			}
		}

		public void Clear()
		{
			dataPackets = new List<DataPacket>();
		}

#endif
		#endregion

		#region private methods
		private void DeletePlayerPrefsEntries(DataPacket dataPacket)
		{
			PlayerPrefs.DeleteKey(dataID + MAIN + dataPacket.Identifier);
			PlayerPrefs.DeleteKey(dataID + HKEY + dataPacket.Identifier);
			PlayerPrefs.SetString(dataID + MANIFEST, GenerateManifestString());
			PlayerPrefs.Save();
		}

		private void RemoveMissingIdentifiersFromPlayerPrefs()
		{
			string savedManifestString = PlayerPrefs.GetString(dataID + MANIFEST);
			string[] identifiers = savedManifestString.Split(new string[] { "||||" }, StringSplitOptions.None);

			List<string> existingIdentifiers = new List<string>();
			foreach (DataPacket dp in dataPackets)
			{
				existingIdentifiers.Add(dp.Identifier);
			}

			for (int i = identifiers.Length - 1; i >= 0; i--)
			{
				string id = identifiers[i];
				if (existingIdentifiers.Contains(id) == false)
				{
					PlayerPrefs.DeleteKey(dataID + MAIN + id);
					PlayerPrefs.DeleteKey(dataID + HKEY + id);
				}
			}
		}

		private List<DataPacket> GetDirtyPackets(List<DataPacket> packets)
		{
			List<DataPacket> dirtPackets = new List<DataPacket>();

			foreach (DataPacket packet in packets)
			{
				if (packet.IsHierarchyDirty)
				{
					dirtPackets.Add(packet);
				}
			}

			return dirtPackets;
		}

		private List<DataPacket> Load()
		{
			string manifestString = PlayerPrefs.GetString(dataID + MANIFEST);
			return GetDataPacketsFromString(manifestString);
		}

		private List<DataPacket> ConvertVersion(List<DataPacket> dataPackets, int oldVersion, int currentVersion)
		{
			// no conversion required as of yet.
			return dataPackets;
		}

		private string GenerateManifestString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < dataPackets.Count; i++)
			{
				DataPacket dp = dataPackets[i];

				if (dp.Count > 0 && PlayerPrefs.HasKey(dataID + MAIN + dp.Identifier))
				{
					sb.Append(dp.Identifier);
					if (i < dataPackets.Count - 1)
					{
						sb.Append("||||");
					}
				}
			}

			return sb.ToString();
		}

		#endregion
	}
}
