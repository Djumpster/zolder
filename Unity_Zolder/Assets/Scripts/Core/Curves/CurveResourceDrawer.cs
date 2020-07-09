// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Serialization;

namespace Talespin.Core.Foundation.Curves
{
	public class CurveResourceDrawer : ResourceDrawer
	{
		public const string TAG = "CompoundCurve";

		public override IEnumerable<string> Tags
		{
			get { yield return TAG; }
		}

		protected override Type ResourceType
		{
			get { return typeof(CompoundCurve); }
		}
	}
}
#endif
