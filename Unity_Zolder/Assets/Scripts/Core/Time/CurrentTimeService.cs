// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Storage;

namespace Talespin.Core.Foundation.TimeKeeping
{
	/// <summary>
	/// Allows you to retrieve the current time with added time spans in order to simulate it being another time.
	/// </summary>
	public class CurrentTimeService
	{
		public TimeSpan Offset { get; private set; }

		public DateTime UTCNow
		{
			get
			{
				return DateTime.UtcNow + Offset;
			}
		}

		/// <summary>
		/// Local Now.
		/// </summary>
		/// <value>The local now.</value>
		public DateTime Now
		{
			get
			{
				return DateTime.Now + Offset;
			}
		}

		private LocalDataManager localDataManager;
		private DataPacket timeOffsetData;

		public CurrentTimeService(LocalDataManager localDataManager, DataPacket timeOffsetData)
		{
			this.localDataManager = localDataManager;
			this.timeOffsetData = timeOffsetData;

			long ticks = (long)this.timeOffsetData.GetDouble("offset", 0);
			Offset = new TimeSpan(ticks);
		}

		public void AddOffset(TimeSpan offset)
		{
			Offset += offset;

			SaveOffset();
		}

		public void ResetOffset()
		{
			Offset = new TimeSpan();

			SaveOffset();
		}

		private void SaveOffset()
		{
			timeOffsetData.Set("offset", (double)Offset.Ticks);
			localDataManager.Save(timeOffsetData);
		}
	}
}