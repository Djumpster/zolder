// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.ErrorHandling
{
	public class Error
	{
		public readonly string ErrorType;
		public readonly string Message;

		public Error(string errorType, string message = "")
		{
			ErrorType = errorType;
			Message = string.IsNullOrEmpty(message) ? errorType : message;
		}

		public override string ToString()
		{
			return "ErrorType: " + ErrorType + (!string.IsNullOrEmpty(Message) ? ", message: " + Message : "");
		}
	}
}
