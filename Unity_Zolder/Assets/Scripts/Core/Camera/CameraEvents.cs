// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras
{
	public class CameraEvents : MonoBehaviour
	{
		public event Action PostRender = delegate { };
		public event Action PreCull = delegate { };

		private void OnPostRender()
		{
			PostRender();
		}

		private void OnPreCull()
		{
			PreCull();
		}
	}
}
