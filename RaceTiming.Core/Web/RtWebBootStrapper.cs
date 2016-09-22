using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace RedRat.RaceTiming.Core.Web
{
	public class RtWebBootStrapper: DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;

            // Prevent result caching
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx =>
            {
                ctx.Response.WithHeader("Cache-Control", "no-cache");
            });
		}
	}
}
