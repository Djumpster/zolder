// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.MemoryScramble
{
	/// <summary>
	/// Stores an int through an Xor with a random number, making sure it cannot be read easily using a memory hack tool.
	/// </summary>
	public struct SecureInt
	{
		private static int KEY { get; set; }
		private int _val;
		private bool used;
		public int Value
		{
			get
			{
				if (!used)
				{
					_val = 0 ^ KEY;
				}

				used = true;
				return _val ^ KEY;
			}
			set
			{
				used = true;
				_val = value ^ KEY;
			}
		}

		static SecureInt()
		{
			KEY = 0;
			for (int i = 0; i < 32; i++)
			{
				int v = new System.Random().NextDouble() > .5 ? 1 : 0;
				KEY |= v << i;
			}
		}

		public SecureInt(int i) : this()
		{
			Value = i;
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public static SecureInt operator +(SecureInt a, SecureInt b)
		{
			return new SecureInt(a.Value + b.Value);
		}

		public static SecureInt operator -(SecureInt a, SecureInt b)
		{
			return new SecureInt(a.Value - b.Value);
		}

		public static SecureInt operator -(SecureInt a)
		{
			return new SecureInt(-a.Value);
		}

		public static SecureInt operator *(SecureInt a, SecureInt b)
		{
			return new SecureInt(a.Value * b.Value);
		}

		public static SecureInt operator /(SecureInt a, SecureInt b)
		{
			return new SecureInt(a.Value / b.Value);
		}

		public static SecureInt operator ++(SecureInt a)
		{
			return new SecureInt(a.Value + 1);
		}

		public static SecureInt operator --(SecureInt a)
		{
			return new SecureInt(a.Value - 1);
		}

		public static implicit operator int(SecureInt i)
		{
			return i.Value;
		}

		public static implicit operator SecureInt(int i)
		{
			return new SecureInt(i);
		}

		public static explicit operator SecureInt(Single i)
		{
			return new SecureInt(Mathf.RoundToInt(i));
		}

		public static explicit operator Single(SecureInt i)
		{
			return i.Value;
		}
	}
}