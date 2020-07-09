// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Misc;

namespace Talespin.Core.Foundation.Graphics
{
	/// <summary>
	/// <para>This service is the runtime representation of <see cref="CubemapPositionData"/>.</para> 
	/// </summary>
	public class CubemapPositionService
	{
		private readonly Dictionary<IPositioned, UnityEngine.Cubemap> cubemaps;

		public CubemapPositionService(CubemapPositionData data)
		{
			cubemaps = new Dictionary<IPositioned, UnityEngine.Cubemap>();

			foreach (CubemapPositionData.CubemapEntry entry in data.Entries)
			{
				IPositioned position = entry.Position as IPositioned;

				if (position == null)
				{
					LogUtil.Error(LogTags.SYSTEM, this, "Cubemap position does not inherit from IPositioned");
					continue;
				}

				if (cubemaps.ContainsKey(position))
				{
					LogUtil.Error(LogTags.SYSTEM, this, "Already contains a value for: " + position);
					continue;
				}

				cubemaps.Add(position, entry.Cubemap);
			}
		}

		/// <summary>
		/// Find the cubemap assigned to the specified object.
		/// </summary>
		/// <param name="position">The object.</param>
		/// <returns>The cubemap if one exists, null otherwise.</returns>
		public UnityEngine.Cubemap FindCubemap(IPositioned position)
		{
			if (cubemaps.ContainsKey(position))
			{
				return cubemaps[position];
			}

			return null;
		}
	}
}
