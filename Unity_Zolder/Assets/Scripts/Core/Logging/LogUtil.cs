// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Talespin.Core.Foundation.Extensions;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Talespin.Core.Foundation.Logging
{
	/// <summary>
	/// <para>Utility class to handle application logging.</para>
	/// 
	/// <para>Allows disabling of logs for builds with the
	/// <c>DISABLE_LOGS</c> scripting define symbol.</para>
	/// 
	/// <para>You can enable verbose logging by adding
	/// the <c>ENABLE_VERBOSE_LOGS</c> scripting define. This will
	/// enable use of the <see cref="Info"/> methods.</para>
	/// </summary>
	public static class LogUtil
	{
		// Because there is no inverse ConditionalAttribute,
		// we have to create a conditional that will NEVER be defined.
		private const string DISABLE_LOGS_CONDITIONAL = "ZZZZZ_DISABLED_LOGGING_ZZZZZ";

		public static IEnumerable<string> FilterTags => filterTags;

		private static List<string> filterTags;

		static LogUtil()
		{
			filterTags = LogTags.GetAvailableTypes();

#if !DISABLE_LOGS && !UNITY_EDITOR
			if (!Debug.isDebugBuild)
			{
				Warning(LogTags.SYSTEM, "The DISABLE_LOGS scripting define has not been set and this is not a development build. Performance may be degraded.");
			}
#endif
		}

		public static bool ContainsFilterTag(string tag)
		{
			return filterTags.Contains(tag);
		}

		public static void RegisterFilterTag(string tag)
		{
			if (!filterTags.Contains(tag))
			{
				filterTags.Add(tag);
			}
		}

		public static void UnregisterFilterTag(string tag)
		{
			filterTags.Remove(tag);
		}

		/// <summary>
		/// Log an info message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// By default this function does not exist, you can enable it by using
		/// the <c>ENABLE_VERBOSE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#else
		[Conditional("ENABLE_VERBOSE_LOGS")]
#endif
		[DebuggerHidden]
		public static void Info(object message, Object context = null)
		{
			if (context == null)
			{
				SendUnityLog(Debug.Log, LogTags.TEST, null, message);
			}
			else
			{
				SendUnityLog(Debug.Log, LogTags.TEST, null, message, context);
			}
		}

		/// <summary>
		/// Log an info message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// By default this function does not exist, you can enable it by using
		/// the <c>ENABLE_VERBOSE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="tag">The filter tag the message should have, can by any of the values specified in <see cref="LogTags"/>,
		/// or custom tags registered with <see cref="RegisterFilterTag(string)"/></param>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#else
		[Conditional("ENABLE_VERBOSE_LOGS")]
#endif
		[DebuggerHidden]
		public static void Info(string tag, object message, Object context = null)
		{
			if (context == null)
			{
				SendUnityLog(Debug.Log, tag, null, message);
			}
			else
			{
				SendUnityLog(Debug.Log, tag, null, message, context);
			}
		}

		/// <summary>
		/// Log an info message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// By default this function does not exist, you can enable it by using
		/// the <c>ENABLE_VERBOSE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="tag">The filter tag the message should have, can by any of the values specified in <see cref="LogTags"/>,
		/// or custom tags registered with <see cref="RegisterFilterTag(string)"/></param>
		/// <param name="caller">The caller </param>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
		/// <seealso cref="LogTags"/>
		/// <seealso cref="RegisterFilterTag(string)"/>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#else
		[Conditional("ENABLE_VERBOSE_LOGS")]
#endif
		[DebuggerHidden]
		public static void Info(string tag, object caller, object message, Object context = null)
		{
			context = context ?? GetContextFromCaller(caller);

			if (context == null)
			{
				SendUnityLog(Debug.Log, tag, caller, message);
			}
			else
			{
				SendUnityLog(Debug.Log, tag, caller, message, context);
			}
		}

		/// <summary>
		/// Log a regular message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// This function may not always be present outside of the editor.
		/// It can be disabled by adding the <c>DISABLE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#endif
		[DebuggerHidden]
		public static void Log(object message, Object context = null)
		{
			if (context == null)
			{
				SendUnityLog(Debug.Log, LogTags.TEST, null, message);
			}
			else
			{
				SendUnityLog(Debug.Log, LogTags.TEST, null, message, context);
			}
		}

		/// <summary>
		/// Log a regular message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// This function may not always be present outside of the editor.
		/// It can be disabled by adding the <c>DISABLE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="tag">The filter tag the message should have, can by any of the values specified in <see cref="LogTags"/>,
		/// or custom tags registered with <see cref="RegisterFilterTag(string)"/></param>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#endif
		[DebuggerHidden]
		public static void Log(string tag, object message, Object context = null)
		{
			if (context == null)
			{
				SendUnityLog(Debug.Log, tag, null, message);
			}
			else
			{
				SendUnityLog(Debug.Log, tag, null, message, context);
			}
		}

		/// <summary>
		/// Log a regular message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// This function may not always be present outside of the editor.
		/// It can be disabled by adding the <c>DISABLE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="tag">The filter tag the message should have, can by any of the values specified in <see cref="LogTags"/>,
		/// or custom tags registered with <see cref="RegisterFilterTag(string)"/></param>
		/// <param name="caller">The caller </param>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
		/// <seealso cref="LogTags"/>
		/// <seealso cref="RegisterFilterTag(string)"/>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#endif
		[DebuggerHidden]
		public static void Log(string tag, object caller, object message, Object context = null)
		{
			context = context ?? GetContextFromCaller(caller);

			if (context == null)
			{
				SendUnityLog(Debug.Log, tag, caller, message);
			}
			else
			{
				SendUnityLog(Debug.Log, tag, caller, message, context);
			}
		}

		/// <summary>
		/// Log a warning message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// This function may not always be present outside of the editor.
		/// It can be disabled by adding the <c>DISABLE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#endif
		[DebuggerHidden]
		public static void Warning(object message, Object context = null)
		{
			if (context == null)
			{
				SendUnityLog(Debug.LogWarning, LogTags.TEST, null, message);
			}
			else
			{
				SendUnityLog(Debug.LogWarning, LogTags.TEST, null, message, context);
			}
		}

		/// <summary>
		/// Log a warning message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// This function may not always be present outside of the editor.
		/// It can be disabled by adding the <c>DISABLE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="tag">The filter tag the message should have, can by any of the values specified in <see cref="LogTags"/>,
		/// or custom tags registered with <see cref="RegisterFilterTag(string)"/></param>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#endif
		[DebuggerHidden]
		public static void Warning(string tag, object message, Object context = null)
		{
			if (context == null)
			{
				SendUnityLog(Debug.LogWarning, tag, null, message);
			}
			else
			{
				SendUnityLog(Debug.LogWarning, tag, null, message, context);
			}
		}

		/// <summary>
		/// Log a warning message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// This function may not always be present outside of the editor.
		/// It can be disabled by adding the <c>DISABLE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="tag">The filter tag the message should have, can by any of the values specified in <see cref="LogTags"/>,
		/// or custom tags registered with <see cref="RegisterFilterTag(string)"/></param>
		/// <param name="caller">The caller </param>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
		/// <seealso cref="LogTags"/>
		/// <seealso cref="RegisterFilterTag(string)"/>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#endif
		[DebuggerHidden]
		public static void Warning(string tag, object caller, object message, Object context = null)
		{
			context = context ?? GetContextFromCaller(caller);

			if (context == null)
			{
				SendUnityLog(Debug.LogWarning, tag, caller, message);
			}
			else
			{
				SendUnityLog(Debug.LogWarning, tag, caller, message, context);
			}
		}

		/// <summary>
		/// Log an error message to the Unity console and player logs.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
		[DebuggerHidden]
		public static void Error(object message, Object context = null)
		{
			if (context == null)
			{
				SendUnityLog(Debug.LogError, LogTags.TEST, null, message);
			}
			else
			{
				SendUnityLog(Debug.LogError, LogTags.TEST, null, message, context);
			}
		}

		/// <summary>
		/// Log an error message to the Unity console and player logs.
		/// </summary>
		/// <remarks>
		/// This function may not always be present outside of the editor.
		/// It can be disabled by adding the <c>DISABLE_LOGS</c> scripting define symbol.
		/// </remarks>
		/// <param name="tag">The filter tag the message should have, can by any of the values specified in <see cref="LogTags"/>,
		/// or custom tags registered with <see cref="RegisterFilterTag(string)"/></param>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
#if DISABLE_LOGS && !UNITY_EDITOR
		[Conditional(DISABLE_LOGS_CONDITIONAL)]
#endif
		[DebuggerHidden]
		public static void Error(string tag, object message, Object context = null)
		{
			if (context == null)
			{
				SendUnityLog(Debug.LogError, tag, null, message);
			}
			else
			{
				SendUnityLog(Debug.LogError, tag, null, message, context);
			}
		}

		/// <summary>
		/// Log an error message to the Unity console and player logs.
		/// </summary>
		/// <param name="tag">The filter tag the message should have, can by any of the values specified in <see cref="LogTags"/>,
		/// or custom tags registered with <see cref="RegisterFilterTag(string)"/></param>
		/// <param name="caller">The caller </param>
		/// <param name="message">The message to log.</param>
		/// <param name="context">An optional context, specifying this parameter highlights the object in the scene hierarchy.</param>
		/// <seealso cref="LogTags"/>
		/// <seealso cref="RegisterFilterTag(string)"/>
		[DebuggerHidden]
		public static void Error(string tag, object caller, object message, Object context = null)
		{
			context = context ?? GetContextFromCaller(caller);

			if (context == null)
			{
				SendUnityLog(Debug.LogError, tag, caller, message);
			}
			else
			{
				SendUnityLog(Debug.LogError, tag, caller, message, context);
			}
		}

		[DebuggerHidden]
		private static void SendUnityLog(Action<object> messageFunc, string tag, object caller, object message)
		{
			if (!filterTags.Contains(tag))
			{
				return;
			}

			messageFunc.Invoke(ConstructFullMessage(tag, caller, message));
		}

		[DebuggerHidden]
		private static void SendUnityLog(Action<object, Object> messageFunc, string tag, object caller, object message, Object context)
		{
			if (!filterTags.Contains(tag))
			{
				return;
			}

			messageFunc.Invoke(ConstructFullMessage(tag, caller, message), context);
		}

		private static string ConstructFullMessage(string tag, object caller, object message)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append(tag);

			if (caller != null)
			{
				builder.Append(" ");
				builder.Append(GetTypeNameFromCaller(caller));
				builder.Append(":");
			}

			builder.Append(" ");
			builder.Append(message);

			return builder.ToString();
		}

		private static Object GetContextFromCaller(object caller)
		{
			if (caller is Object)
			{
				return caller as Object;
			}

			return null;
		}

		private static string GetTypeNameFromCaller(object caller)
		{
			if (caller is string)
			{
				return caller.ToString();
			}

			if (caller != null)
			{
				return caller.GetType().GetSanitizedTypeNameString();
			}

			return string.Empty;
		}
	}
}
