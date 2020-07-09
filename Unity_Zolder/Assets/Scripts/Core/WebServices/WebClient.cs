// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Talespin.Core.Foundation.WebServices
{
	/// <summary>
	/// A class for sending web requests and receiving responses to the specified URL.
	/// </summary>
	public class WebClient
	{
		public delegate void ResponseCallback(UnityWebRequestAsyncOperation responseMessage);

		public readonly string ServerURL;

		private readonly Dictionary<string, string> clientHeaders;

		public WebClient(string serverURL)
		{
			this.ServerURL = serverURL;
			clientHeaders = new Dictionary<string, string>();
		}

		/// <summary>
		/// Add (or update if exists already) a header value that persists through the lifetime of the client
		/// </summary>
		/// <param name="name">The key of the header to be set. Case-sensitive.</param>
		/// <param name="value">The header's intended value.</param>
		public void SetClientHeader(string name, string value)
		{
			clientHeaders[name] = value;
		}

		public void RemoveClientHeader(string name)
		{
			if (clientHeaders.ContainsKey(name))
			{
				clientHeaders.Remove(name);
			}
		}

		/// <summary>
		/// Creates a GET request to the provided endpoint.
		/// </summary>
		/// <param name="endPointURL">Endpoint of the url to make the call to</param>
		/// <param name="headers">Additional headers for this request</param>
		/// <param name="responseCallback">Callback that is fired when the request is done</param>
		/// <returns>UnityWebRequestAsyncOperation which contains data specific to this request</returns>
		public UnityWebRequestAsyncOperation Get(string endPointURL, Dictionary<string, string> headers = null, ResponseCallback responseCallback = null)
		{
			UnityWebRequest unityWebRequest = CreateWebRequest(endPointURL, UnityWebRequest.kHttpVerbGET, null, new DownloadHandlerBuffer(), headers);

			UnityWebRequestAsyncOperation asyncOperation = unityWebRequest.SendWebRequest();

			if (responseCallback != null)
			{
				asyncOperation.completed += w => { responseCallback((UnityWebRequestAsyncOperation)w); };
			}

			return asyncOperation;
		}

		/// <summary>
		/// Creates a POST request to the provided endpoint.
		/// </summary>
		/// <param name="endPointURL">Endpoint of the url to make the call to</param>
		/// <param name="content">Data you want to send along with the request</param>
		/// <param name="contentType">Type of content</param>
		/// <param name="headers">Additional headers for this request</param>
		/// <param name="responseCallback">Callback that is fired when the request is done</param>
		/// <returns>UnityWebRequestAsyncOperation which contains data specific to this request</returns>
		public UnityWebRequestAsyncOperation Post(string endPointURL, byte[] content, string contentType, int timeout, Dictionary<string, string> headers = null, ResponseCallback responseCallback = null)
		{
			UnityWebRequest unityWebRequest = CreateWebRequest(endPointURL, UnityWebRequest.kHttpVerbPOST, new UploadHandlerRaw(content), new DownloadHandlerBuffer(), headers);

			unityWebRequest.SetRequestHeader("Content-Type", contentType);
			unityWebRequest.timeout = timeout;

			UnityWebRequestAsyncOperation asyncOperation = unityWebRequest.SendWebRequest();

			if (responseCallback != null)
			{
				asyncOperation.completed += w => { responseCallback((UnityWebRequestAsyncOperation)w); };
			}

			return asyncOperation;
		}

		/// <summary>
		/// Creates a PUT request to the provided endpoint.
		/// </summary>
		/// <param name="endPointURL">Endpoint of the url to make the call to</param>
		/// <param name="content">Data you want to send along with the request</param>
		/// <param name="contentType">Type of content</param>
		/// <param name="headers">Additional headers for this request</param>
		/// <param name="responseCallback">Callback that is fired when the request is done</param>
		/// <returns>UnityWebRequestAsyncOperation which contains data specific to this request</returns>
		public UnityWebRequestAsyncOperation Put(string endPointURL, byte[] content, string contentType, Dictionary<string, string> headers = null, ResponseCallback responseCallback = null)
		{
			UnityWebRequest unityWebRequest = CreateWebRequest(endPointURL, UnityWebRequest.kHttpVerbPUT, new UploadHandlerRaw(content), new DownloadHandlerBuffer(), headers);

			unityWebRequest.SetRequestHeader("Content-Type", contentType);

			UnityWebRequestAsyncOperation asyncOperation = unityWebRequest.SendWebRequest();

			if (responseCallback != null)
			{
				asyncOperation.completed += w => { responseCallback((UnityWebRequestAsyncOperation)w); };
			}

			return asyncOperation;
		}

		/// <summary>
		/// Creates a UnityWebRequest with the provided arguments. This does not send out a request.
		/// </summary>
		/// <param name="endPointURL">Endpoint of the url to make the call to</param>
		/// <param name="method">Request method for this request (POST, GET, etc)</param>
		/// <param name="uploadHandler">Uploadhandler with a data buffer to send with the request</param>
		/// <param name="downloadHandler">Download handler with the resulting data from the request</param>
		/// <param name="headers">Additional headers for this request</param>
		/// <returns>A UnityWebRequest object that is ready to send out the request</returns>
		public UnityWebRequest CreateWebRequest(string endPointURL, string method, UploadHandler uploadHandler, DownloadHandler downloadHandler, Dictionary<string, string> headers = null)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(GetURI(endPointURL), method, downloadHandler, uploadHandler);

			ProcessClientHeaders(unityWebRequest, headers);

			return unityWebRequest;
		}

		private void ProcessClientHeaders(UnityWebRequest unityWebRequest, Dictionary<string, string> headers = null)
		{
			foreach (KeyValuePair<string, string> headerValues in clientHeaders)
			{
				unityWebRequest.SetRequestHeader(headerValues.Key, headerValues.Value);
			}

			if (headers != null)
			{
				foreach (KeyValuePair<string, string> headerValues in headers)
				{
					unityWebRequest.SetRequestHeader(headerValues.Key, headerValues.Value);
				}
			}
		}

		private Uri GetURI(string endPointURL)
		{
			return new Uri(ServerURL + "/" + endPointURL);
		}
	}
}
