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
        }

        protected List<object> GetRunners(ControllerFactory controllerFactory)
        {
            var runners = controllerFactory.AppController.GetRunners();
            return runners.Select( r => new
            {
                r.FirstName,
                r.LastName,
                r.DateOfBirth,
                r.Club,
                r.Team,
            } ).Cast<object>().ToList();
        }
    }
}
