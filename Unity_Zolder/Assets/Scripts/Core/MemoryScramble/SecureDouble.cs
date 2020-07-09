// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Globalization;

namespace Talespin.Core.Foundation.MemoryScramble
{
	/// <summary>
	/// Stores a double in non-consecutive memory, making sure it cannot be read easily using a memory hack tool.
	/// </summary>
	public struct SecureDouble
	{
		private int b1;
		private int b2;
		private int b3;
		private int b4;
		private bool used;
		private int b5;
		private int b6;
		private int b7;
		private int b8;
		public double Value
		{
			get
			{
				if (!used)
				{
					Value = 0f;
				}

				return System.BitConverter.ToDouble(
					new byte[]
					{
						(byte) b2,
						(byte) b3,
						(byte) b1,
						(byte) b4,

						(byte) b6,
						(byte) b7,
						(byte) b5,
						(byte) b8
					},
					0);
			}
			set
			{
				used = true;
				byte[] b = System.BitConverter.GetBytes(value);
				b2 = b[0];
				b3 = b[1];
				b1 = b[2];
				b4 = b[3];

				b6 = b[4];
				b7 = b[5];
				b5 = b[6];
				b8 = b[7];
			}
		}

		public SecureDouble(double i) : this()
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

		public static SecureDouble operator +(SecureDouble a, SecureDouble b)
		{
			return new SecureDouble(a.Value + b.Value);
		}

		public static SecureDouble operator -(SecureDouble a, SecureDouble b)
		{
			return new SecureDouble(a.Value - b.Value);
		}

		public static SecureDouble operator -(SecureDouble a)
		{
			return new SecureDouble(-a.Value);
		}

		public static SecureDouble operator *(SecureDouble a, SecureDouble b)
		{
			return new SecureDouble(a.Value * b.Value);
		}

		public static SecureDouble operator /(SecureDouble a, SecureDouble b)
		{
			return new SecureDouble(a.Value / b.Value);
		}

		public static SecureDouble operator ++(SecureDouble a)
		{
			return new SecureDouble(a.Value + 1f);
		}

		public static SecureDouble operator --(SecureDouble a)
		{
			return new SecureDouble(a.Value - 1f);
		}

		public static implicit operator SecureDouble(int i)
		{
			return new SecureDouble(i);
		}

		public static implicit operator SecureDouble(float i)
		{
			return new SecureDouble(i);
		}

		public static implicit operator SecureDouble(double i)
		{
			return new SecureDouble(i);
		}

		public static implicit operator double(SecureDouble i)
		{
			return i.Value;
		}

		public static implicit operator float(SecureDouble i)
		{
			return (float)i.Value;
		}

		public static implicit operator int(SecureDouble i)
		{
			return (int)i.Value;
		}
	}
}