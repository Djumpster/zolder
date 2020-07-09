// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Reflection;
using UnityEngine;

namespace Talespin.Core.Foundation.Filter
{
	public sealed class TypeFilterAttribute : PropertyAttribute
	{
		public readonly Type[] BaseTypes;
		public readonly string NamespaceFilter;
		public readonly InclusionFlags InclusionFlags;

		public TypeFilterAttribute(params Type[] baseTypes) : this(baseTypes, null)
		{
		}

		public TypeFilterAttribute(Type baseType, string namespaceFilter = null, InclusionFlags inclusionFlags = InclusionFlags.Default) : this(new Type[] { baseType }, namespaceFilter, inclusionFlags)
		{
		}

		public TypeFilterAttribute(Type[] baseTypes, string namespaceFilter = null, InclusionFlags inclusionFlags = InclusionFlags.Default)
		{
			BaseTypes = baseTypes;
			NamespaceFilter = namespaceFilter;
			InclusionFlags = inclusionFlags;
		}
	}
}
