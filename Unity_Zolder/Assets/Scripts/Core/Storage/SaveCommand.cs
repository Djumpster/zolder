// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Misc;

namespace Talespin.Core.Foundation.Storage
{
	/// <summary>
	/// Lets you save a given packet or all data packets to local storage.
	/// </summary>
	public class SaveCommand : ICommand
	{
		#region members
		private DataPacket rootPacket;
		private LocalDataManager localDataManager;

		#endregion

		#region constructor
		public SaveCommand(LocalDataManager localDataManager, DataPacket rootPacket = null)
		{
			this.localDataManager = localDataManager;
			this.rootPacket = rootPacket;
		}

		#endregion

		#region public methods
		public void Execute()
		{
			if (rootPacket == null)
			{
				localDataManager.Save();
			}
			else
			{
				localDataManager.Save(rootPacket);
			}
		}

		public void Destroy()
		{
			localDataManager = null;
			rootPacket = null;
		}

		#endregion
	}
}
