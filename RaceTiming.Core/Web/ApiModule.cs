using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using RedRat.RaceTiming.Core.Util;
using RedRat.RaceTiming.Core.ViewModels;

namespace RedRat.RaceTiming.Core.Web
{
    public class ApiModule : NancyModule
    {
        public ApiModule( ControllerFactory controllerFactory ) : base("/api")
        {
            Get["/runners"] = parameters => SetDefaultHeaders( Response.AsJson( GetRunners( controllerFactory ) ) );

            Get["/results"] = parameters => SetDefaultHeaders( Response.AsJson( GetResults( controllerFactory ) ) );

            Get["/finishers"] = parameters => SetDefaultHeaders( Response.AsJson( GetFinishers( controllerFactory ) ) );

            Post["/addrunner"] = ( x ) =>
            {
                var message = "New entrant added.";
                var statusCode = HttpStatusCode.OK;
                var newRunner = this.Bind<NewRunner>();

                // ToDo: Add runner...
                Console.WriteLine("Have runner...");

                return SetDefaultHeaders( Response.AsJson( message, statusCode ) );
            };

            Post["/addfinishposition"] = (x) =>
            {
                var message = "";
                var statusCode = HttpStatusCode.OK;
                var posResult = this.Bind<PositionResult>();
                try
                {
                    if ( posResult.Position <= 0 )
                    {
                        message = "Race number must be > 0";
                        statusCode = HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        controllerFactory.AppController.AddResultRunnerNumber( posResult.Position );
                        Trace.WriteLineIf( AppController.traceSwitch.TraceInfo, "Runner number added: " + posResult.Position );
                    }
                }
                catch ( Exception ex )
                {
                    message = ex.Message;
                    statusCode = HttpStatusCode.InternalServerError;
                    Trace.WriteLineIf( AppController.traceSwitch.TraceError, "Error adding runner number: " + ex.Message );
                }
                return SetDefaultHeaders( Response.AsJson( message, statusCode ) );
            };
        }

        protected Response SetDefaultHeaders( Response response )
        {
            response.Headers = new Dictionary<string, string>
                {
                    {"Cache-Control", "no-cache" }
                };
            return response;           
        }

		protected object GetRunners(ControllerFactory controllerFactory)
		{
		    var controller = controllerFactory.AppController;
			var runners = controller.GetRunners();
			var entrants = runners.OrderBy( r => r.Number ).Select( r => new
				{
					r.Number,
					r.FirstName,
					r.LastName,
					r.DateOfBirth,
                    agegroup = AgeGroup.GetAgeGroup( controller.CurrentRace.Date, r.DateOfBirth, r.Gender ).ToString(),
					r.Club,
					r.Team,
				} ).Cast<object>().ToList();
			return new {entrants = entrants};
		}

        /// <summary>
        /// Returns the list of finishing times and positions
        /// </summary>
        protected object GetResults(ControllerFactory controllerFactory)
        {
            var results = controllerFactory.AppController.GetResults();
			var raceResults = results.OrderByDescending(r => r.Position).Select(r => new
            {
                r.Position,
                Time = r.Time.TotalMilliseconds,
                r.RaceNumber,
            }).Cast<object>().ToList();
			return new {raceResults = raceResults};
        }

        protected object GetFinishers( ControllerFactory controllerFactory )
        {
            var r = new[]
            {
                new
                {
                    Position = 1,
                    FirstName = "Bilbo",
                    LastName = "Baggins",
                    Time = "00:38:23",
                    Category = "MV40",
                    CategoryPosition = "1",
                    Club = "Shire Strollers",
                    Team = "Ring Fellowship",
                    wma = "76.37",
                },
                new
                {
                    Position = 2,
                    FirstName = "Gandalf",
                    LastName = "The Grey",
                    Time = "00:45:67",
                    Category = "MV60",
                    CategoryPosition = "1",
                    Club = "Shire Strollers",
                    Team = "Ring Fellowship",
                    wma = "84.22",

                }
            };
            return new {finishers = r};
        }
    }
}
