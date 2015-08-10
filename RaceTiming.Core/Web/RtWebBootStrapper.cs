using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

using System;

using RedRat.RaceTiming.Data;

namespace RedRat.RaceTiming.Core.Web
{
	public class RtWebBootStrapper: DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;
		}
	}
}
