using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace RedRat.RaceTiming.Core.Web
{
    public class ApiModule : NancyModule
    {
        public ApiModule( ControllerFactory controllerFactory ) : base("/api")
        {
            Get["/runners"] = parameters => Response.AsJson( GetRunners( controllerFactory ) );
            Get["/results"] = parameters => Response.AsJson(GetResults(controllerFactory));
        }

		protected object GetRunners(ControllerFactory controllerFactory)
		{
			var runners = controllerFactory.AppController.GetRunners();
			var entrants = runners.OrderBy( r => r.Number ).Select( r => new
				{
					r.Number,
					r.FirstName,
					r.LastName,
					r.DateOfBirth,
					r.Club,
					r.Team,
				} ).Cast<object>().ToList();
			return new {entrants = entrants};
		}

        protected object GetResults(ControllerFactory controllerFactory)
        {
            var results = controllerFactory.AppController.GetResults();
			var raceResults = results.OrderBy(r => r.Position).Select(r => new
            {
                r.Position,
                r.Time,
            }).Cast<object>().ToList();
			return new {raceResults = raceResults};
        }
    
    }
}
