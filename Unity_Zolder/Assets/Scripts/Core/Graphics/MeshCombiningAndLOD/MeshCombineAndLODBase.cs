// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	public class MeshCombineAndLODBase : MonoBehaviour
	{
		public bool IsValid
		{
			get { return GetUncombinedPrefabMeshes().Count == 0; }
		}

		public bool IsCombined
		{
			get { return IsValid && GetCombinedMeshResults().Count > 0; }
		}

		[SerializeField] private bool _disableBoxLOD;
		public bool DisableBoxLOD { get { return _disableBoxLOD; } }

		public List<Renderer> GetCombinedMeshResults()
		{
			List<Renderer> ret = new List<Renderer>();
			Renderer[] rs = GetComponentsInChildren<Renderer>();
			foreach (Renderer r in rs)
			{
				if (r.tag == "CombinedMesh")
				{
					ret.Add(r);
				}
			}
			return ret;
		}

		public List<Renderer> GetCombinedPrefabMeshes()
		{
			List<Renderer> ret = new List<Renderer>();
			Renderer[] rs = GetComponentsInChildren<Renderer>();
			foreach (Renderer r in rs)
			{
				if (r.tag != "CombinedMesh" && !r.enabled)
				{
					ret.Add(r);
				}
			}
			return ret;
		}

		public List<Renderer> GetUncombinedPrefabMeshes()
		{
			List<Renderer> ret = new List<Renderer>();
			Renderer[] rs = GetComponentsInChildren<Renderer>();
			foreach (Renderer r in rs)
			{
				if (r.tag != "CombinedMesh" && r.enabled)
				{
					ret.Add(r);
				}
			}
			return ret;
		}

		IEnumerator Start()
		{
			// make forgetting to combine a big nuissance!
			if (!IsValid)
			{
				while (true)
				{
					LogUtil.Error(LogTags.SYSTEM, this, "The object " + gameObject.name + " has not been combined!", gameObject);
					yield return null;
				}
			}
		}
	}
}