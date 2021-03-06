﻿using System;
using System.Diagnostics;
using Nancy.Hosting.Self;

namespace RedRat.RaceTiming.Core.Web
{
	/// <summary>
	/// Manages the in-built web server (Nancy).
	/// </summary>
	public class WebController
	{
		protected NancyHost webHost;
	    protected string serverUrl;

	    public WebController( string serverUrl )
	    {
	        this.serverUrl = serverUrl;
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

			webHost = new NancyHost ( hostConfig, new Uri (serverUrl));

			webHost.Start();

			Trace.WriteLineIf( AppController.traceSwitch.TraceInfo, "Web server started on " + serverUrl);
		}
	}
}

