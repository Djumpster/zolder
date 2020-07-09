// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Events
{
	public interface IMap<T> where T : class
	{
		void AddMapping(T from, T to);
		bool HasMapping(T source);
		T Map(T source);
	}
}