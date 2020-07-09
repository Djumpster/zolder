// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Injection
{
	public class InjectionIdentifierAttribute : Attribute
	{
		public readonly string Identifier;
		public InjectionIdentifierAttribute(string identifier)
		{
			Identifier = identifier;
		}
	}
}