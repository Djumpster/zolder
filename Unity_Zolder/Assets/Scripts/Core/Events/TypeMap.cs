// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Filter;
using Talespin.Core.Foundation.Serialization;
using UnityEngine;

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// The Type Map is used to map types to other types.
	/// One is for this class is when mapping events in the EventDispatcher system from one type to another.
	///
	/// This class is fully unit tested, so please re-run tests if you make any changes.  
	/// </summary>
	[Serializable]
	public class TypeMap : IMap<Type>
	{
		[Serializable]
		public class FilterEntry
		{
			[SerializeField, TypeFilter(typeof(IEvent)), TypeTag(typeof(IEvent))]
			private string from;
			[SerializeField, TypeFilter(typeof(IEvent)), TypeTag(typeof(IEvent))]
			private string to;

			private Type fromType;
			private Type toType;

			public Type From { get { if (fromType == null) { fromType = from.LoadType(); } return fromType; } }
			public Type To { get { if (toType == null) { toType = to.LoadType(); } return toType; } }

			public FilterEntry(Type from, Type to)
			{
				fromType = from;
				toType = to;
			}
		}

		[SerializeField, CreateTag(typeof(FilterEntry))] private List<FilterEntry> mapping = new List<FilterEntry>();

		/// <summary>
		/// Adds a mapping from one event to another.
		/// </summary>
		/// <param name="from">From type</param>
		/// <param name="to">To type</param>
		public void AddMapping(Type from, Type to)
		{
			mapping.Add(new FilterEntry(from, to));
		}

		public bool HasAnyMapping()
		{
			return mapping.Count > 0;
		}

		public Type Map(Type source)
		{
			foreach (FilterEntry entry in mapping)
			{
				if (entry.From == source)
				{
					return entry.To;
				}
			}
			return null;
		}

		public bool HasMapping(Type source)
		{
			return Map(source) != null;
		}
	}
}