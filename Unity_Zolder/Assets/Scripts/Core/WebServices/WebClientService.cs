// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.WebServices
{
	/// <summary>
	/// The webclient service creates and keeps track of webclients for all unique URL's
	/// </summary>
	public class WebClientService
	{
		private Dictionary<string, WebClient> webClients;

		public WebClientService()
		{
			webClients = new Dictionary<string, WebClient>();
		}

		public WebClient GetOrCreate(string serverURL)
		{
			WebClient webClient = null;

			if (webClients.TryGetValue(serverURL, out webClient))
			{
				return webClient;
			}

			webClient = new WebClient(serverURL);
			webClients.Add(serverURL, webClient);

			return webClient;
		}
	}
}