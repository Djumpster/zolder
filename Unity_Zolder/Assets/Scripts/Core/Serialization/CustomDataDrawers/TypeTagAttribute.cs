// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Serialization
{
	public class TypeTagAttribute : DataTagAttribute
	{
		public TypeTagAttribute(Type baseType) : base("type_" + baseType.AssemblyQualifiedName) { }
	}

	public class CreateTagAttribute : DataTagAttribute
	{
		public CreateTagAttribute(Type baseType) : base("create_" + baseType.AssemblyQualifiedName) { }
	}
}
