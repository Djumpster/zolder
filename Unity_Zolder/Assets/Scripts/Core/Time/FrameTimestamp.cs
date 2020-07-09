// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.TimeKeeping
{
	/// <summary>
	/// A time stamp you can create to used track when a certain event happened.
	/// It contains both a frame number and a sequence number within that frame.
	/// The latter allows you to oreder timestamps within a frame.
	/// </summary>
	public struct FrameTimestamp : IComparable
	{
		/// <summary>
		/// The frame number where the timestamp was created.
		/// </summary>
		public readonly int FrameNumber;
		/// <summary>
		/// The sequence number within the frame (timestamps with a higher number 
		/// start later in the frame).
		/// </summary>
		public readonly int SequenceNumber;

		private static int lastCount = -1;
		private static int currentSequenceNumber;

		private FrameTimestamp(int frameNumber, int sequenceNumber)
		{
			FrameNumber = frameNumber;
			SequenceNumber = sequenceNumber;
		}

		public int CompareTo(object obj)
		{
			if (obj is FrameTimestamp)
			{
				var other = (FrameTimestamp)obj;
				int result = FrameNumber.CompareTo(other.FrameNumber);
				if (result == 0)
				{
					result = SequenceNumber.CompareTo(other.SequenceNumber);
				}
				return -result;
			}
			else
			{
				throw new ArgumentException("Argument is not a FrameTimestamp: " + obj.GetType());
			}
		}

		public override bool Equals(object obj)
		{
			return CompareTo(obj) == 0;
		}

		public override int GetHashCode()
		{
			return FrameNumber.GetHashCode() ^ SequenceNumber.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", FrameNumber, SequenceNumber);
		}

		/// <summary>
		/// Create a FrameTimestamp for the current moment in time
		/// </summary>
		public static FrameTimestamp Create()
		{
			currentSequenceNumber++;
			var count = Time.frameCount;
			if (count != lastCount)
			{
				lastCount = count;
				currentSequenceNumber = 0;
			}
			return new FrameTimestamp(count, currentSequenceNumber);
		}

		public static bool operator <(FrameTimestamp stampA, FrameTimestamp stampB)
		{
			return stampA.CompareTo(stampB) < 0;
		}

		public static bool operator >(FrameTimestamp stampA, FrameTimestamp stampB)
		{
			return stampA.CompareTo(stampB) > 0;
		}

		public static bool operator <=(FrameTimestamp stampA, FrameTimestamp stampB)
		{
			return stampA.CompareTo(stampB) < 1;
		}

		public static bool operator >=(FrameTimestamp stampA, FrameTimestamp stampB)
		{
			return stampA.CompareTo(stampB) > -1;
		}

		public static bool operator ==(FrameTimestamp stampA, FrameTimestamp stampB)
		{
			return stampA.CompareTo(stampB) == 0;
		}

		public static bool operator !=(FrameTimestamp stampA, FrameTimestamp stampB)
		{
			return stampA.CompareTo(stampB) != 0;
		}
	}
}