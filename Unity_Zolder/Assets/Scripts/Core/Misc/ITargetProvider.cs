// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Misc
{
	public interface ITargetProvider : IDisposable
	{
		IPositioned Target { get; }
		bool HasTarget { get; }
	}
}