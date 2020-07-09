// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Globalization;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Storage;

namespace Talespin.Core.Foundation.Settings
{
	// T should be an Enum!
	// Unfortunately, we cannot use where T : System.enum.
	// Therefore we enforce as many interfaces from System.Enum as possible.
	public abstract class SettingsService<T> : IBootstrappable where T : IConvertible, IFormattable, IComparable
	{
		public delegate void SettingChangedHandler(T value);
		public event SettingChangedHandler SettingChangedEvent = delegate { };

		protected int mode;
		protected abstract string Identifier { get; }
		protected abstract T DefaultValue { get; }

		public T CurrentSetting
		{
			get
			{
				return (T)(object)mode;
			}
			set
			{
				int newVal = value.ToInt32(CultureInfo.InvariantCulture);
				if (mode != newVal)
				{
					mode = newVal;
					SaveMode(mode);
					ApplyMode((T)(object)mode);

					SettingChangedEvent.Invoke((T)(object)mode);
				}
			}
		}

		protected SettingsService(bool applyMode = true)
		{
			mode = LoadMode();

			if (applyMode)
			{
				ApplyMode((T)(object)mode);
			}
		}

		protected int LoadMode()
		{
			return LazyPlayerPrefs.GetInt(Identifier, DefaultValue.ToInt32(CultureInfo.InvariantCulture));
		}

		private void SaveMode(int mode)
		{
			LazyPlayerPrefs.SetInt(Identifier, mode);
		}

		protected abstract void ApplyMode(T mode);
	}
}
