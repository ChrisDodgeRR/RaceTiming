using Nancy.Hosting.Self;
using RedRat.RaceTiming.Core;

using System;

namespace RedRat.RaceTiming.Core.Web
{
	/// <summary>
	/// Manages the in-built web server (Nancy).
	/// </summary>
	public class WebController
	{
		protected NancyHost webHost;
		protected AppController appController;

		public WebController(AppController appController)
		{
			this.appController = appController;
		}

		public void Start()
		{
			// This is needed for Windows so that local access to the web address is allowed.
			var hostConfig = new HostConfiguration
			{
				UrlReservations = new UrlReservations
				{
					CreateAutomatically = true,
				}
			};

			webHost = new NancyHost ( hostConfig, new Uri ("http://0.0.0.0:1234/"));

			webHost.Start();

			Console.WriteLine("Web server started....");
		}
	}
}

