// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class AnimatorExtensions
	{
		public static bool HasParameter(this Animator animator, string paramName)
		{
			int paramCount = animator.parameterCount;
			AnimatorControllerParameter[] parameters = animator.parameters;

			for (int i = 0; i < paramCount; i++)
			{
				if (parameters[i].name.Equals(paramName, System.StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasParameter(this Animator animator, int hash)
		{
			int paramCount = animator.parameterCount;
			AnimatorControllerParameter[] parameters = animator.parameters;

			for (int i = 0; i < paramCount; i++)
			{
				if (parameters[i].nameHash == hash)
				{
					return true;
				}
			}
			return false;
		}

		public static bool TrySetBool(this Animator animator, string paramName, bool value)
		{
			bool hasParam = HasParameter(animator, paramName);
			if (hasParam)
			{
				animator.SetBool(paramName, value);
			}

			return hasParam;
		}

		public static bool TrySetBool(this Animator animator, int paramHash, bool value)
		{
			bool hasParam = HasParameter(animator, paramHash);
			if (hasParam)
			{
				animator.SetBool(paramHash, value);
			}

			return hasParam;
		}

		public static bool TryGetBool(this Animator animator, string paramName, out bool result)
		{
			bool hasParam = HasParameter(animator, paramName);
			result = hasParam ? animator.GetBool(paramName) : false;
			return hasParam;
		}

		public static bool TryGetBool(this Animator animator, int paramHash, out bool result)
		{
			bool hasParam = HasParameter(animator, paramHash);
			result = hasParam ? animator.GetBool(paramHash) : false;
			return hasParam;
		}

		public static bool TrySetInteger(this Animator animator, string paramName, int value)
		{
			bool hasParam = HasParameter(animator, paramName);
			if (hasParam)
			{
				animator.SetInteger(paramName, value);
			}

			return hasParam;
		}

		public static bool TrySetInteger(this Animator animator, int paramHash, int value)
		{
			bool hasParam = HasParameter(animator, paramHash);
			if (hasParam)
			{
				animator.SetInteger(paramHash, value);
			}

			return hasParam;
		}

		public static bool TryGetInteger(this Animator animator, string paramName, out int result)
		{
			bool hasParam = HasParameter(animator, paramName);
			result = hasParam ? animator.GetInteger(paramName) : 0;
			return hasParam;
		}

		public static bool TryGetInteger(this Animator animator, int paramHash, out int result)
		{
			bool hasParam = HasParameter(animator, paramHash);
			result = hasParam ? animator.GetInteger(paramHash) : 0;
			return hasParam;
		}

		public static bool TrySetFloat(this Animator animator, string paramName, float value)
		{
			bool hasParam = HasParameter(animator, paramName);
			if (hasParam)
			{
				animator.SetFloat(paramName, value);
			}

			return hasParam;
		}

		public static bool TrySetFloat(this Animator animator, int paramHash, float value)
		{
			bool hasParam = HasParameter(animator, paramHash);
			if (hasParam)
			{
				animator.SetFloat(paramHash, value);
			}

			return hasParam;
		}

		public static bool TryGetFloat(this Animator animator, string paramName, out float result)
		{
			bool hasParam = HasParameter(animator, paramName);
			result = hasParam ? animator.GetFloat(paramName) : 0f;
			return hasParam;
		}

		public static bool TryGetFloat(this Animator animator, int paramHash, out float result)
		{
			bool hasParam = HasParameter(animator, paramHash);
			result = hasParam ? animator.GetFloat(paramHash) : 0f;
			return hasParam;
		}

		public static bool TrySetTrigger(this Animator animator, string paramName)
		{
			bool hasParam = HasParameter(animator, paramName);
			if (hasParam)
			{
				animator.SetTrigger(paramName);
			}

			return hasParam;
		}

		public static bool TrySetTrigger(this Animator animator, int paramHash)
		{
			bool hasParam = HasParameter(animator, paramHash);
			if (hasParam)
			{
				animator.SetTrigger(paramHash);
			}

			return hasParam;
		}

		public static bool TryResetTrigger(this Animator animator, string paramName)
		{
			bool hasParam = HasParameter(animator, paramName);
			if (hasParam)
			{
				animator.ResetTrigger(paramName);
			}

			return hasParam;
		}

		public static bool TryResetTrigger(this Animator animator, int paramHash)
		{
			bool hasParam = HasParameter(animator, paramHash);
			if (hasParam)
			{
				animator.ResetTrigger(paramHash);
			}

			return hasParam;
		}
	}
}