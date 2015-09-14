using Nancy;

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

            Get["/allresults"] = parameters => View["Results"];

            Get["/winners"] = parameters => View["Winners"];
        }

	    private IndexModel CreateIndexModel( ControllerFactory controllerFactory )
	    {
	        var model = new IndexModel();
	        if ( controllerFactory.AppController.CurrentRace == null )
	        {
	            model.RaceName = "No race loaded";
	        }
	        else
	        {
	            model.RaceName = controllerFactory.AppController.CurrentRace.Name;
	        }
	        return model;
	    }
	}
}

