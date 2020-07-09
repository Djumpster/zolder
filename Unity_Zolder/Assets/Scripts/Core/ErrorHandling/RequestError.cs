// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine.Networking;

namespace Talespin.Core.Foundation.ErrorHandling
{
	/// <summary>
	/// An error related to a web request.
	/// </summary>
	public class RequestError : Error
	{
		public readonly string RequestURL;
		public readonly Dictionary<string, string> RequestHeaders;
		public readonly Dictionary<string, string> ResponseHeaders;

		public RequestError(
			string errorType,
			string message = "",
			string requestURL = "",
			Dictionary<string, string> requestHeaders = null,
			Dictionary<string, string> responseHeaders = null)
			: base(errorType, message)
		{
			RequestURL = requestURL;
			RequestHeaders = requestHeaders;
			ResponseHeaders = responseHeaders;
		}

		public RequestError(Error error,
			string requestURL = "",
			Dictionary<string, string> requestHeaders = null,
			Dictionary<string, string> responseHeaders = null)
			: base(error.ErrorType, error.Message)
		{
			RequestURL = requestURL;
			RequestHeaders = requestHeaders;
			ResponseHeaders = responseHeaders;
		}

		public RequestError(UnityWebRequest webRequest) : base(webRequest.responseCode.ToString(), webRequest.error)
		{
		}
	}
}
