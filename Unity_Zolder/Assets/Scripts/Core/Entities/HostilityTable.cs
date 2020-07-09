// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Entities
{
	public class HostilityTable : IHostilityTable
	{
		private struct HostileFactions
		{
			public string[] groupA;
			public string[] groupB;
			public HostileFactions(string[] groupA, string[] groupB)
			{
				this.groupA = groupA;
				this.groupB = groupB;
			}
		}

		private List<HostileFactions> hostilities = new List<HostileFactions>();

		public HostilityTable(string[] groupA, string[] groupB)
		{
			Add(groupA, groupB);
		}

		public void Add(string[] groupA, string[] groupB)
		{
			hostilities.Add(new HostileFactions(groupA, groupB));
		}

		public bool AreHostile(string entityTagA, string entityTagB)
		{
			// all spelled out, since List.Contains causes a high amount of memory trashing
			for (int i = 0; i < hostilities.Count; i++)
			{
				for (int j = 0; j < hostilities[i].groupA.Length; j++)
				{
					if (hostilities[i].groupA[j] == entityTagA)
					{
						for (int k = 0; k < hostilities[i].groupB.Length; k++)
						{
							if (hostilities[i].groupB[k] == entityTagB)
							{
								return true;
							}
						}
					}
					if (hostilities[i].groupA[j] == entityTagB)
					{
						for (int k = 0; k < hostilities[i].groupB.Length; k++)
						{
							if (hostilities[i].groupB[k] == entityTagA)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}
}