// Copyright 2020 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Events;

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// Event fired when keyboard input was received.
	/// </summary>
	public class KeyboardInputEvent : IEvent
	{
		/// <summary>
		/// The sender that has send the keyboard event.
		/// </summary>
		public object Sender { get; }

		/// <summary>
		/// The value of the keyboard input. Can be any character,
		/// or any of these special values:
		/// <list type="bullet">
		/// <item><c>Backspace</c></item>
		/// <item><c>Enter</c></item>
		/// <item><c>Tab</c></item>
		/// <item><c>ArrowUp</c></item>
		/// <item><c>ArrowRight</c></item>
		/// <item><c>ArrowDown</c></item>
		/// <item><c>ArrowLeft</c></item>
		/// </list>
		/// </summary>
		public string Value { get; }

		public KeyboardInputEvent(object sender, string value)
		{
			Sender = sender;
			Value = value;
		}
	}
}
