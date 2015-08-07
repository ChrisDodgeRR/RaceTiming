using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

using System;

namespace RedRat.RaceTiming.Core.Web
{
	public class RtBootStrapper: DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;
		}
	}
}

