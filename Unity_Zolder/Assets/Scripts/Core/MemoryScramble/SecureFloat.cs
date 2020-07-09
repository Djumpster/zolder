// Copyright 2018 Talespin, LLC. All Rights Reserved.using System;

using System;
using System.Globalization;

namespace Talespin.Core.Foundation.MemoryScramble
{
	/// <summary>
	/// Stores a float in non-consecutive memory, making sure it cannot be read easily using a memory hack tool.
	/// </summary>
	public struct SecureFloat
	{
		private int b1;
		private int b2;
		private bool used;
		private int b3;
		private int b4;
		public float Value
		{
			get
			{
				if (!used)
				{
					Value = 0f;
				}

				used = true;
				return BitConverter.ToSingle(new byte[] { (byte)b2, (byte)b3, (byte)b1, (byte)b4 }, 0);
			}
			set
			{
				used = true;
				byte[] b = BitConverter.GetBytes(value);
				b2 = b[0];
				b3 = b[1];
				b1 = b[2];
				b4 = b[3];
			}
		}

		public SecureFloat(float i) : this()
		{
			Value = i;
		}

		public override string ToString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}

		public string ToString(string formatting)
		{
			return Value.ToString(formatting);
		}

		public static SecureFloat operator +(SecureFloat a, SecureFloat b)
		{
			return new SecureFloat(a.Value + b.Value);
		}

		public static SecureFloat operator -(SecureFloat a, SecureFloat b)
		{
			return new SecureFloat(a.Value - b.Value);
		}

		public static SecureFloat operator -(SecureFloat a)
		{
			return new SecureFloat(-a.Value);
		}

		public static SecureFloat operator *(SecureFloat a, SecureFloat b)
		{
			return new SecureFloat(a.Value * b.Value);
		}

		public static SecureFloat operator /(SecureFloat a, SecureFloat b)
		{
			return new SecureFloat(a.Value / b.Value);
		}

		public static SecureFloat operator ++(SecureFloat a)
		{
			return new SecureFloat(a.Value + 1f);
		}

		public static SecureFloat operator --(SecureFloat a)
		{
			return new SecureFloat(a.Value - 1f);
		}

		public static implicit operator SecureFloat(int i)
		{
			return new SecureFloat(i);
		}

		// this should also work for converting floats.
		// need this since the JSON deserializer uses Single, not float.
		public static implicit operator SecureFloat(Single i)
		{
			return new SecureFloat(i);
		}

		public static implicit operator Single(SecureFloat i)
		{
			return i.Value;
		}
	}
}