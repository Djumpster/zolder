// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using UnityEngine;

namespace Talespin.Core.Foundation.Services
{
	public interface ICoroutineService
	{
		Coroutine StartCoroutine(IEnumerator coroutine, object context, string contextName = "");
		void StartContext(object context);
		bool HasContext(object context);
		void StopContext(object context);
	}
}