// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Talespin.Core.Foundation.Maths
{
	public static class Encryption
	{
		private static Dictionary<Type, HashAlgorithm> instances = new Dictionary<Type, HashAlgorithm>();
		private static StringBuilder textBuilder = new StringBuilder();

		public enum Type
		{
			MD5,
			SHA256,
			HMAC_SHA256
		}

		public static HashAlgorithm GetAlgorithm(Type type)
		{
			HashAlgorithm algorithm;
			if (!instances.TryGetValue(type, out algorithm))
			{
				switch (type)
				{
					case Type.MD5:
						algorithm = MD5CryptoServiceProvider.Create();
						break;
					case Type.SHA256:
						algorithm = new SHA256Managed();
						break;
					case Type.HMAC_SHA256:
						algorithm = new HMACSHA256();
						break;
					default:
						throw new ArgumentException("[Encryption] Unsupported algorithm.");
				}
				instances.Add(type, algorithm);
			}
			return algorithm;
		}

		public static string Hash(string data, string key, Type type = Type.HMAC_SHA256)
		{
			Encoding encoding = Encoding.UTF8;
			HashAlgorithm algorithm = GetAlgorithm(type);
			KeyedHashAlgorithm keyedAlgorithm = algorithm as KeyedHashAlgorithm;
			byte[] prevKey = null;
			if (keyedAlgorithm == null)
			{
				data = string.Concat(data, key);
			}
			else
			{
				prevKey = keyedAlgorithm.Key;
				keyedAlgorithm.Key = encoding.GetBytes(key);
			}

			byte[] dataBytes = encoding.GetBytes(data);
			byte[] hash = algorithm.ComputeHash(dataBytes);

			if (keyedAlgorithm != null)
			{
				keyedAlgorithm.Key = prevKey;
			}
			return ByteArrayToHexString(hash);
		}

		public static string XorCrypt(string data, string key)
		{
			textBuilder.Length = 0;
			textBuilder.Capacity = data.Length;
			for (int i = 0; i < data.Length; i++)
			{
				char encrypted = (char)(data[i] ^ key[i % key.Length]);
				textBuilder.Append(encrypted);
			}
			return textBuilder.ToString();
		}

		public static string ByteArrayToHexString(byte[] byteArray)
		{
			textBuilder.Length = 0;
			textBuilder.Capacity = byteArray.Length * 2;
			foreach (byte b in byteArray)
			{
				textBuilder.AppendFormat("{0:x2}", b);
			}
			return textBuilder.ToString();
		}

		public static byte[] HexStringToByteArray(string hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return bytes;
		}
	}
}
