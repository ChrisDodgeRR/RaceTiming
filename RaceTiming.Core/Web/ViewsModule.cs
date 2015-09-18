using Nancy;
using RedRat.RaceTiming.Core.ViewModels;

namespace RedRat.RaceTiming.Core.Web
{
	public class ViewsModule : NancyModule
	{
		public ViewsModule ()
		{
			Get ["/"] = parameters => View ["Index"];

            Get["/addrunner"] = parameters => View["RaceEntry"];

            Get["/runners"] = parameters => View["Runners"];

            Get["/enterpositions"] = parameters => View["EnterPositions"];

            Get["/allresults"] = parameters => View["Results"];

            Get["/winners"] = parameters => View["Winners"];
        }
	}
}

