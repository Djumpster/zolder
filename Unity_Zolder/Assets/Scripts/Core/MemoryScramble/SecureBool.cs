// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.MemoryScramble
{
	/// <summary>
	/// Stores a bool using an int as backing field through an Xor with a random number, 
	/// making sure it cannot be read easily using a memory hack tool.
	/// </summary>
	public struct SecureBool
	{
		private static int KEY { get; set; }
		private int _val;
		private bool used;
		public bool Value
		{
			get
			{
				if (!used)
				{
					_val = 0 ^ KEY;
				}
				used = true;
				int val = _val ^ KEY;
				return val == 1;
			}
			set
			{
				used = true;
				int intValue = 0;
				if (value == true)
				{
					intValue = 1;
				}
				_val = intValue ^ KEY;
			}
		}

		static SecureBool()
		{
			KEY = 0;
			for (int i = 0; i < 32; i++)
			{
				int v = new System.Random().NextDouble() > .5 ? 1 : 0;
				KEY |= v << i;
			}
		}

		public SecureBool(bool i) : this()
		{
			Value = i;
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public static implicit operator SecureBool(bool i)
		{
			return new SecureBool(i);
		}

		public static implicit operator bool(SecureBool i)
		{
			return i.Value;
		}
	}
}