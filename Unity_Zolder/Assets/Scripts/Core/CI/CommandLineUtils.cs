// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.CI
{
	/// <summary>
	/// Utility class to parse command line parameters.
	/// Arguments starting with <c>-</c> are interpreted as identifiers.
	/// If an argument does not start with <c>-</c>, it's interpreted as the value of the previous argument.
	/// </summary>
	public static class CommandLineUtils
	{
		private static readonly Dictionary<string, string> args;

		static CommandLineUtils()
		{
			args = new Dictionary<string, string>();

			string[] arguments = Environment.GetCommandLineArgs();
			for (int i = 0; i < arguments.Length; i++)
			{
				string argument = arguments[i];

				if (argument.StartsWith("-"))
				{
					string identifier = argument.Substring(1);
					string value = "true";

					if (i + 1 < arguments.Length)
					{
						string next = arguments[i + 1];

						if (!next.StartsWith("-"))
						{
							value = next;
						}
					}

					SetString(identifier, value);
				}
			}
		}

		/// <summary>
		/// Check whether a command line argument has been set.
		/// </summary>
		/// <param name="key">The name of the argument</param>
		/// <returns><see langword="true"/> if the argument has been set.</returns>
		public static bool HasKey(string key) => args.ContainsKey(key);

		/// <summary>
		/// Inject a command line argument.
		/// If the argument already exists, the value will be overriden.
		/// </summary>
		/// <param name="key">The identifier, excluding any leading -</param>
		/// <param name="value">The value of the argument.</param>
		public static void SetString(string key, string value)
		{
			args[key] = value;
		}

		/// <summary>
		/// Attempt to get a string value from a command line argument.
		/// </summary>
		/// <param name="key">The name of the argument.</param>
		/// <param name="value">The output value</param>
		/// <returns><see langword="true"/> if the value was successfully retrieved</returns>
		public static bool TryGetString(string key, out string value)
		{
			if (HasKey(key))
			{
				value = args[key];
				return true;
			}

			value = string.Empty;
			return false;
		}

		/// <summary>
		/// Attempt to get a float value from a command line argument.
		/// </summary>
		/// <param name="key">The name of the argument.</param>
		/// <param name="value">The output value</param>
		/// <returns><see langword="true"/> if the value was successfully retrieved</returns>
		public static bool TryGetFloat(string key, out float value)
		{
			if (HasKey(key) && float.TryParse(args[key], out value))
			{
				return true;
			}

			value = 0f;
			return false;
		}

		/// <summary>
		/// Attempt to get an int value from a command line argument.
		/// </summary>
		/// <param name="key">The name of the argument.</param>
		/// <param name="value">The output value</param>
		/// <returns><see langword="true"/> if the value was successfully retrieved</returns>
		public static bool TryGetInt(string key, out int value)
		{
			if (HasKey(key) && int.TryParse(args[key], out value))
			{
				return true;
			}

			value = 0;
			return false;
		}

		/// <summary>
		/// Attempt to get a boolean value from a command line argument.
		/// </summary>
		/// <param name="key">The name of the argument.</param>
		/// <param name="value">The output value</param>
		/// <returns><see langword="true"/> if the value was successfully retrieved</returns>
		public static bool TryGetBool(string key, out bool value)
		{
			if (HasKey(key) && bool.TryParse(args[key], out value))
			{
				return true;
			}

			value = false;
			return false;
		}

		/// <summary>
		/// Get a string value from a command line argument.
		/// </summary>
		/// <param name="key">The name of the argument.</param>
		/// <returns>The value if it was successfully retrieved, an empty string if not.</returns>
		public static string GetString(string key)
		{
			TryGetString(key, out string value);
			return value;
		}

		/// <summary>
		/// Get a float value from a command line argument.
		/// </summary>
		/// <param name="key">The name of the argument.</param>
		/// <returns>The value if it was successfully retrieved, <c>0f</c> if not.</returns>
		public static float GetFloat(string key)
		{
			TryGetFloat(key, out float value);
			return value;
		}

		/// <summary>
		/// Get an int value from a command line argument.
		/// </summary>
		/// <param name="key">The name of the argument.</param>
		/// <returns>The value if it was successfully retrieved, <c>0</c> if not.</returns>
		public static int GetInt(string key)
		{
			TryGetInt(key, out int value);
			return value;
		}

		/// <summary>
		/// Get a boolean value from a command line argument.
		/// </summary>
		/// <param name="key">The name of the argument.</param>
		/// <returns>The value if it was successfully retrieved, <c>false</c> if not.</returns>
		public static bool GetBool(string key)
		{
			TryGetBool(key, out bool value);
			return value;
		}
	}
}
