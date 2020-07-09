// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System.Collections.Generic;

namespace Talespin.Core.Foundation.Serialization
{
	public class EntityTagResourceDrawer : StringPopupDrawer
	{
		public override IEnumerable<string> Tags
		{
			get { yield return "Tag"; }
		}

		protected override IEnumerable<string> GetOptions(DataEntry entry)
		{
			return new string[0];
		}
	}
}
#endif
