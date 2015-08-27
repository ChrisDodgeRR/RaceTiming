using System.Collections.Generic;
using Nancy;
using RedRat.RaceTiming.Data.Model;

namespace RedRat.RaceTiming.Core.Web
{
    public class ApiModule : NancyModule
    {
        public ApiModule( ControllerFactory controllerFactory ) : base("/api")
        {
            Get["/runners"] = parameters => Response.AsJson(GetRunners());
        }

        protected List<Runner> GetRunners()
        {
            return new List<Runner>
            {
                new Runner {FirstName = "Bilbo", LastName = "Baggins"}
            };
        }
    }
}
