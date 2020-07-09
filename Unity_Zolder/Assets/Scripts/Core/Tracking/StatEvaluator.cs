// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Misc;

namespace Talespin.Core.Foundation.Tracking
{
	/// <summary>
	/// IStatEvaluator listens to changes to tracked statistics and evaluates if a check is passed.
	/// If the check is passed the passed the passed commands are performed (such as unlock).
	/// evaluation.
	/// </summary>
	public abstract class StatEvaluator
	{
		protected StatLoggerService statLogger;

		protected ICommand[] onEvalPassedCommands;

		protected StatEvaluator(StatLoggerService statLogger, ICommand[] onEvalPassedCommands)
		{
			if (statLogger == null)
			{
				throw new NullReferenceException("statTracker cannot be null.");
			}

			if (onEvalPassedCommands == null)
			{
				throw new NullReferenceException("onEvalPassedActions cannot be null.");
			}

			this.statLogger = statLogger;
			this.onEvalPassedCommands = onEvalPassedCommands;

			this.statLogger.StatChangedEvent += OnStatChangedEvent;
		}

		public void Destroy()
		{
			statLogger = null;

			if (onEvalPassedCommands != null)
			{
				for (int i = 0; i < onEvalPassedCommands.Length; i++)
				{
					onEvalPassedCommands[i].Destroy();
				}
			}

			onEvalPassedCommands = null;
		}

		/// <summary>
		/// Evaluate the statTracker and check if the condition is passed.
		/// </summary>
		protected abstract bool Evaluate();

		protected void OnStatChangedEvent(string id, string type, float oldAmount, float newAmount)
		{
			if (!Evaluate())
			{
				return;
			}

			statLogger.StatChangedEvent -= OnStatChangedEvent;

			for (int i = 0; i < onEvalPassedCommands.Length; i++)
			{
				onEvalPassedCommands[i].Execute();
			}
		}
	}
}
