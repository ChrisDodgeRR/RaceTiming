using Nancy;
using RedRat.RaceTiming.Core.ViewModels;

namespace RedRat.RaceTiming.Core.Web
{
	public class ViewsModule : NancyModule
	{
		public ViewsModule (ControllerFactory controllerFactory)
		{
			Get ["/"] = parameters => View ["Index", CreateIndexModel(controllerFactory)];

            Get["/addrunner"] = parameters => View["RaceEntry"];

            Get["/runners"] = parameters => View["Runners"];

            Get["/enterpositions"] = parameters => View["EnterPositions"];

            Get["/allresults"] = parameters => View["Results", CreateResultsModel(controllerFactory)];

            Get["/winners"] = parameters => View["Winners"];
        }

	    private IndexModel CreateIndexModel( ControllerFactory controllerFactory )
	    {
	        var model = new IndexModel();
            var race = controllerFactory.AppController.CurrentRace;
            if (race == null)
	        {
	            model.RaceName = "No race loaded";
	        }
	        else
	        {
	            model.RaceName = race.Name;
                model.RaceDate = race.Date.ToLongDateString();
            }
	        return model;
        }

        private ResultsModel CreateResultsModel(ControllerFactory controllerFactory)
        {
            var model = new ResultsModel();
            var race = controllerFactory.AppController.CurrentRace;
            if ( race == null)
            {
                model.RaceName = "No race loaded";
            }
            else
            {
                model.RaceName = race.Name;
                model.RaceDate = race.Date.ToLongDateString();
            }
            return model;
        }

	}
}

