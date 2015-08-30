using System;
using System.Diagnostics;
using Nancy.Hosting.Self;
using RedRat.RaceTiming.Core.Config;
using RedRat.RaceTiming.Core.Util;

namespace RedRat.RaceTiming.Core.Web
{
	/// <summary>
	/// Manages the in-built web server (Nancy).
	/// </summary>
	public class WebController
	{
		protected NancyHost webHost;
	    protected string serverUrl = "http://localhost:1234/";

	    public WebController()
	    {
            // ToDo: Make configurable, and work out why "localhost" doesn't work on Mac.
            //if ( FeatureSet.IsMac() )
	        if ( CurrentOS.IsMac )
	        {
                serverUrl = "http://0.0.0.0:1234/";
            }
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

