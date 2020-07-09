// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class LayerMaskExtensions
	{
		public static int ToLayerNumber(this LayerMask mask)
		{
			for (int i = 0; i < 32; i++)
			{
				if ((1 << i) == mask)
				{
					return i;
				}
			}
			return 0;
		}

		public static string ToLayerName(this LayerMask mask)
		{
			return LayerMask.LayerToName(ToLayerNumber(mask));
		}

		public static LayerMask[] EnumerateLayers(this LayerMask mask)
		{
			List<LayerMask> ret = new List<LayerMask>();
			for (int i = 0; i < 32; i++)
			{
				if (((1 << i) & mask.value) != 0)
				{
					ret.Add(1 << i);
				}
			}
			return ret.ToArray();
		}

		private static LayerMask GetLayerMask(this int number)
		{
			return 1 << number;
		}

		private static LayerMask GetLayerMask(this string name)
		{
			return 1 << LayerMask.NameToLayer(name);
		}

		public static LayerMask Add(this LayerMask mask, params string[] layerNames)
		{
			foreach (string l in layerNames)
			{
				mask |= l.GetLayerMask();
			}
			return mask;
		}

		public static LayerMask Add(this LayerMask mask, params int[] layers)
		{
			foreach (int l in layers)
			{
				mask |= l.GetLayerMask();
			}
			return mask;
		}

		public static LayerMask Remove(this LayerMask mask, params string[] layerNames)
		{
			foreach (string l in layerNames)
			{
				mask &= ~l.GetLayerMask();
			}
			return mask;
		}

		public static LayerMask Remove(this LayerMask mask, params int[] layers)
		{
			foreach (int l in layers)
			{
				mask &= ~l.GetLayerMask();
			}
			return mask;
		}

		public static bool Contains(this LayerMask mask, string layerName)
		{
			return (mask & layerName.GetLayerMask()) != 0;
		}

		public static bool Contains(this LayerMask mask, int layer)
		{
			return (mask & layer.GetLayerMask()) != 0;
		}

		public static bool ContainsMask(this LayerMask mask, LayerMask otherMask)
		{
			return (mask.value & otherMask.value) != 0;
		}
	}
}
