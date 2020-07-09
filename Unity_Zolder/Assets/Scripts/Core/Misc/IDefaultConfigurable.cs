// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Misc
{
	/// <summary>
	/// Marks the object as being able to set itself to a default configuration.
	/// </summary>
	public interface IDefaultConfigurable
	{
#if UNITY_EDITOR
		void SetDefaultConfiguration();
#endif
	}
}