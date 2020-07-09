// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations
{
	/// <summary>
	/// This class wraps the animator and caches its parameter to avoid having huge memory trashing as a result of
	/// animator.parameters.
	/// </summary>
	public class AnimatorWrapper
	{
		private readonly Animator animator;
		private HashSet<int> parameterHashes;

		public Vector3 DeltaPosition
		{
			get
			{
				if (!animator)
				{
					return Vector3.zero;
				}
				return animator.deltaPosition;
			}
		}

		public Quaternion DeltaRotation
		{
			get
			{
				if (!animator)
				{
					return Quaternion.identity;
				}
				return animator.deltaRotation;
			}
		}

		public bool IsValid
		{
			get
			{
				return (animator && animator.isInitialized);
			}
		}

		public bool HasRootMotion
		{
			get
			{
				if (IsValid)
				{
					return animator.hasRootMotion && animator.applyRootMotion;
				}
				return false;
			}
			set
			{
				if (IsValid && animator.hasRootMotion)
				{
					animator.applyRootMotion = value;
				}
			}
		}

		public RuntimeAnimatorController RuntimeAnimatorController
		{
			get
			{
				if (IsValid)
				{
					return animator.runtimeAnimatorController;
				}
				return null;
			}
			set
			{
				if (IsValid)
				{
					animator.runtimeAnimatorController = value;
				}
			}
		}

		public int LayerCount
		{
			get
			{
				if (IsValid)
				{
					return animator.layerCount;
				}
				return 0;
			}
		}

		public AnimatorWrapper(Animator animator)
		{
			this.animator = animator;
			InitialiseAnimator();
		}

		private void InitialiseAnimator()
		{
			AnimatorControllerParameter[] animatorControllerParameters = animator.parameters;
			parameterHashes = new HashSet<int>();
			for (int i = 0; i < animatorControllerParameters.Length; i++)
			{
				parameterHashes.Add(animatorControllerParameters[i].nameHash);
			}
		}

		/// <summary>
		/// If you don't get why you shouldn't use this outside of editor code from the function name,
		/// just don't use this function at all! 
		/// </summary>
		/// <returns>The enumeration of parameters for editor and debugging purposes only.</returns>
		public AnimatorControllerParameter[] ExpensiveEnumerationOfParametersForEditorAndDebuggingPurposesOnly()
		{
			if (!IsValid)
			{
				return new AnimatorControllerParameter[0];
			}
			return animator.parameters;
		}

		public int GetCurrentAnimatorStateHash(int layerIndex)
		{
			if (IsValid)
			{
				return animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash;
			}
			return -1;
		}

		public float GetLayerWeight(int layerIndex)
		{
			if (IsValid)
			{
				return animator.GetLayerWeight(layerIndex);
			}
			return 0;
		}

		public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex)
		{
			if (IsValid)
			{
				return animator.GetCurrentAnimatorClipInfo(layerIndex);
			}
			return new AnimatorClipInfo[] { };
		}

		public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
		{
			if (IsValid)
			{
				return animator.GetCurrentAnimatorStateInfo(layerIndex);
			}
			return new AnimatorStateInfo();
		}

		public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
		{
			if (IsValid)
			{
				return animator.GetAnimatorTransitionInfo(layerIndex);
			}

			return new AnimatorTransitionInfo();
		}

		public T GetBehaviour<T>() where T : StateMachineBehaviour
		{
			if (IsValid)
			{
				return animator.GetBehaviour<T>();
			}

			return null;
		}

		public void Play(int stateNameHash, int layerIndex, float normalizedTime)
		{
			if (IsValid)
			{
				animator.Play(stateNameHash, layerIndex, normalizedTime);
			}
		}

		public void ResampleAllLayers()
		{
			if (IsValid)
			{
				animator.Update(0f);
			}
		}

		public bool HasParameter(int paramHash)
		{
			if (!IsValid)
			{
				return false;
			}
			return animator.isInitialized && parameterHashes.Contains(paramHash);
		}

		public bool TrySetBool(int paramHash, bool value)
		{
			bool hasParam = HasParameter(paramHash);
			if (hasParam)
			{
				animator.SetBool(paramHash, value);
			}
			return hasParam;
		}

		public bool TryGetBool(int paramHash, out bool result)
		{
			bool hasParam = HasParameter(paramHash);
			result = hasParam && animator.GetBool(paramHash);
			return hasParam;
		}

		public bool TrySetInteger(int paramHash, int value)
		{
			bool hasParam = HasParameter(paramHash);
			if (hasParam)
			{
				animator.SetInteger(paramHash, value);
			}
			return hasParam;
		}

		public bool TryGetInteger(int paramHash, out int result)
		{
			bool hasParam = HasParameter(paramHash);
			result = hasParam ? animator.GetInteger(paramHash) : 0;
			return hasParam;
		}

		public bool TrySetFloat(int paramHash, float value)
		{
			bool hasParam = HasParameter(paramHash);
			if (hasParam)
			{
				animator.SetFloat(paramHash, value);
			}
			return hasParam;
		}

		public bool TryGetFloat(int paramHash, out float result)
		{
			bool hasParam = HasParameter(paramHash);
			result = (animator && hasParam) ? animator.GetFloat(paramHash) : 0f;
			return hasParam;
		}

		public bool TrySetTrigger(int paramHash)
		{
			bool hasParam = HasParameter(paramHash);
			if (hasParam)
			{
				animator.SetTrigger(paramHash);
			}

			return hasParam;
		}

		public bool TryResetTrigger(int paramHash)
		{
			bool hasParam = HasParameter(paramHash);
			if (hasParam)
			{
				animator.ResetTrigger(paramHash);
			}
			return hasParam;
		}

		public bool GetBool(int paramHash)
		{
			if (IsValid)
			{
				return animator.GetBool(paramHash);
			}
			return false;
		}

		public void SetBool(int paramHash, bool val)
		{
			if (IsValid)
			{
				animator.SetBool(paramHash, val);
			}
		}

		public float GetFloat(int paramHash)
		{
			if (IsValid)
			{
				return animator.GetFloat(paramHash);
			}
			return 0f;
		}

		public void SetFloat(int paramHash, float val)
		{
			if (IsValid)
			{
				animator.SetFloat(paramHash, val);
			}
		}

		public int GetInteger(int paramHash)
		{
			if (IsValid)
			{
				return animator.GetInteger(paramHash);
			}
			else
			{
				return 0;
			}
		}

		public void SetInteger(int paramHash, int val)
		{
			if (IsValid)
			{
				animator.SetInteger(paramHash, val);
			}
		}

		public void SetTrigger(int paramHash)
		{
			if (IsValid)
			{
				animator.SetTrigger(paramHash);
			}
		}

		public void ResetTrigger(int paramHash)
		{
			if (IsValid)
			{
				animator.ResetTrigger(paramHash);
			}
		}

		public void SetLayerWeight(int layerIndex, float weight)
		{
			if (animator && animator.isInitialized)
			{
				animator.SetLayerWeight(layerIndex, weight);
			}
		}

		public int GetLayerIndex(string layerName)
		{
			if (animator && animator.isInitialized)
			{
				return animator.GetLayerIndex(layerName);
			}
			return 0;
		}
	}
}
