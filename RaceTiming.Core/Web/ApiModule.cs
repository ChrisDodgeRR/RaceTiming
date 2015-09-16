using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using RedRat.RaceTiming.Core.Util;
using RedRat.RaceTiming.Core.ViewModels;
using RedRat.RaceTiming.Data;
using RedRat.RaceTiming.Data.Model;

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
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;
                var newRunner = this.Bind<NewRunner>();

                try
                {
                    // Check fields
                    CheckField( newRunner.FirstName, "First Name" );
                    CheckField( newRunner.LastName, "Last Name" );
                    CheckField( newRunner.Gender, "Gender" );
                    CheckField( newRunner.DoB, "DoB" );

                    int number;
                    if ( !int.TryParse( newRunner.Number, out number ) )
                    {
                        throw new Exception( "Race number format is incorrect." );
                    }

                    var runners = appController.GetRunners();
                    if ( runners.Any( r => r.Number == number ) )
                    {
                        throw new Exception("A runner with this number already exists.");
                    }

                    var runner = new Runner
                    {
                        FirstName = newRunner.FirstName,
                        LastName = newRunner.LastName,
                        Gender = ( newRunner.Gender == "F" ) ? GenderEnum.Female : GenderEnum.Male,
                        DateOfBirth = DateTime.Parse( newRunner.DoB ),
                        Number = number,
                        Club = newRunner.Club,
                        Team = newRunner.Team,
                    };

                    var db = appController.DbService;
                    if ( !db.TestDuplicate( runner ) )
                    {
                        db.AddRunner( runner );
                        message = string.Format( "'{0}' added to database OK.", runner.ToString() );
                    }
                    else
                    {
                        throw new Exception(string.Format("'{0}' with this DoB already exists in database.", runner.ToString()));
                    }
                }
                catch ( Exception ex )
                {
                    statusCode = HttpStatusCode.InternalServerError; // Is this correct???
                    message = ex.Message;
                    Trace.WriteLineIf( AppController.traceSwitch.TraceError, "Error adding entry: " + ex.Message );
                }
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
                    statusCode = HttpStatusCode.InternalServerError;    // Is this correct???
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
            var controller = controllerFactory.AppController;

            var finishers = controller
                .GetResults()
                .OrderBy( r => r.Position )
                .Select( r => new
                {
                    Position = r.Position,
                    Name = "",
                    Number = r.RaceNumber,
                    Time = r.Time.TotalMilliseconds,
                    Category = "",
                    CategoryPosition = "",
                    Club = "",
                    Team = "",
                    wma = "",
                }).Cast<object>().ToList();

            var runners = controller.GetRunners();

            foreach ( var finisher in finishers )
            {
                // ToDo - get the runner information...
                //var runner = runners.FirstOrDefault( r => r.Number == finisher.Number );
            }

            return new { finishers = finishers };

            /*var r = new[]
            {
                new
                {
                    Position = 1,
                    Name = "Bilbo Baggins",
                    Number = "123",
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
                    Name = "Gandalf The Grey",
                    Number = "123",
                    Time = "00:45:67",
                    Category = "MV60",
                    CategoryPosition = "1",
                    Club = "Shire Strollers",
                    Team = "Ring Fellowship",
                    wma = "84.22",

                }
            };
            return new {finishers = r}; */
        }

        private void CheckField( string field, string fieldname )
        {
            if ( string.IsNullOrEmpty( field ) )
            {
                throw new Exception( string.Format( "Field '{0}' cannot be empty.", fieldname ) );
            }
            
        }
    }
}
