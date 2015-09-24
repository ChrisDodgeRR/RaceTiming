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
            Get["/raceinfo"] = parameters =>
            {
                try
                {
                    return SetDefaultHeaders( Response.AsJson( GetRaceInfo( controllerFactory ) ) );
                }
                catch ( Exception ex )
                {
                    return SetDefaultHeaders(Response.AsJson(ex.Message, HttpStatusCode.InternalServerError));                    
                }
            };

            Get["/runner"] = parameters =>
            {
                var number = Request.Query["number"];
                var runner = GetRunner( controllerFactory, number );
                var statusCode = HttpStatusCode.OK;
                if ( runner == null )
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }
                return SetDefaultHeaders(Response.AsJson( new {runner = runner}, statusCode));
            };

            Get["/runners"] = parameters => SetDefaultHeaders( Response.AsJson( GetRunners( controllerFactory ) ) );

            Get["/results"] = parameters =>
            {
                try
                {
                    return SetDefaultHeaders( Response.AsJson( GetResults( controllerFactory ) ) );
                }
                catch ( Exception ex )
                {
                    return SetDefaultHeaders( Response.AsJson( ex.Message, HttpStatusCode.InternalServerError ) );
                }
            };

            Get["/result"] = parameters =>
            {
                var position = Request.Query["position"];
                var raceResult = GetResult(controllerFactory, position);
                var statusCode = HttpStatusCode.OK;
                if (raceResult == null)
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }
                return SetDefaultHeaders(Response.AsJson(new { raceResult = raceResult }, statusCode));
            };

            Post["/updateresult"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;
                var newResult = this.Bind<NewResult>();

                try
                {
                    var result = new Result()
                    {
                        Position = newResult.Position,
                        RaceNumber = newResult.RaceNumber,
                        Time = new TimeSpan( newResult.Hours, newResult.Minutes, newResult.Seconds ),
                    };
                    appController.DbService.UpdateResult( result );
                }
                catch (Exception ex)
                {
                    statusCode = HttpStatusCode.InternalServerError; // Is this correct???
                    message = ex.Message;
                    Trace.WriteLineIf(AppController.traceSwitch.TraceError, "Error updating result: " + ex.Message);
                }

                return SetDefaultHeaders(Response.AsJson(message, statusCode));
            };

            Get["/finishers"] = parameters => SetDefaultHeaders(Response.AsJson(GetFinishers(controllerFactory)));

            Get["/winners"] = parameters => SetDefaultHeaders( Response.AsJson( GetWinners( controllerFactory ) ) );

            Get["/teams"] = parameters => SetDefaultHeaders(Response.AsJson(GetTeams(controllerFactory)));

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
                        Email = newRunner.Email,
                        Number = number,
                        Club = newRunner.Club,
                        Team = newRunner.Team,
                        Urn = newRunner.Urn,
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

            Post["/updaterunner"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;
                var newRunner = this.Bind<NewRunner>();

                try
                {
                    // Check fields
                    CheckField(newRunner.FirstName, "First Name");
                    CheckField(newRunner.LastName, "Last Name");
                    CheckField(newRunner.Gender, "Gender");
                    CheckField(newRunner.DoB, "DoB");

                    int number;
                    if (!int.TryParse(newRunner.Number, out number))
                    {
                        throw new Exception("Race number format is incorrect.");
                    }
                    
                    // Update
                    var runner = new Runner
                    {
                        Number = number,
                        FirstName = newRunner.FirstName,
                        LastName = newRunner.LastName,
                        Gender = (newRunner.Gender == "Female") ? GenderEnum.Female : GenderEnum.Male,
                        DateOfBirth = DateTime.Parse(newRunner.DoB),
                        Email = newRunner.Email,
                        Club = newRunner.Club,
                        Team = newRunner.Team,
                        Urn = newRunner.Urn,
                    };

                    appController.DbService.UpdateRunner( runner );
                }
                catch ( Exception ex )
                {
                    statusCode = HttpStatusCode.InternalServerError; // Is this correct???
                    message = ex.Message;
                    Trace.WriteLineIf( AppController.traceSwitch.TraceError, "Error adding entry: " + ex.Message );
                }
                return SetDefaultHeaders( Response.AsJson( message, statusCode ) );
            };

            Post["/deleterunner"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;
                var number = this.Bind<RunnerNumber>();

                try
                {
                    appController.DbService.DeleteRunner(number.Number);
                    Trace.WriteLineIf( AppController.traceSwitch.TraceInfo,
                        string.Format( "Runner with number {0} deleted.", number.Number ) );
                }
                catch ( Exception ex )
                {
                    statusCode = HttpStatusCode.InternalServerError; // Is this correct???
                    message = ex.Message;
                    Trace.WriteLineIf( AppController.traceSwitch.TraceError, "Error deleting runner: " + ex.Message );
                }
                return SetDefaultHeaders( Response.AsJson( message, statusCode ) );
            };

            Post["/addfinishposition"] = (x) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;
                var posResult = this.Bind<PositionResult>();
                try
                {
                    if ( !appController.IsClockRunning )
                    {
                        throw new Exception( "Race is not in progress (clock not running)." );
                    }

                    if ( posResult.Position <= 0 )
                    {
                        message = "Race number must be > 0";
                        statusCode = HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        appController.AddResultRunnerNumber( posResult.Position );
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

        protected object GetRaceInfo( ControllerFactory controllerFactory )
        {
            var race = controllerFactory.AppController.CurrentRace;
            return new
            {
                name = race.Name,
                description = race.Description,
                date = race.Date,
                distance = race.Distance,
            };
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
                    r.Urn,
				} ).Cast<object>().ToList();
			return new {entrants = entrants};
		}

        protected object GetRunner(ControllerFactory controllerFactory, int runnerNumber)
        {
            var controller = controllerFactory.AppController;
            var runner = controller.GetRunners().FirstOrDefault( r => r.Number == runnerNumber );
            if ( runner == null ) return null;
            return new
            {
                runner.Number,
                runner.FirstName,
                runner.LastName,
                runner.Email,
                gender = runner.Gender.ToString(),
                dob = runner.DateOfBirth.ToString("yyyy/MM/dd"),
				runner.Club,
				runner.Team,
                runner.Urn,
            };
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
                r.DubiousResult,
                reason = r.GetDubiousResultReason(),
            }).Cast<object>().ToList();
			return new { raceResults = raceResults };
        }

        protected object GetResult(ControllerFactory controllerFactory, int position )
        {
            var result = controllerFactory.AppController.GetResults().FirstOrDefault( r => r.Position == position);
            if ( result == null ) return null;
            return new
            {
                position = position,
                time = result.Time,
                result.RaceNumber,
            };
        }

        protected object GetFinishers(ControllerFactory controllerFactory)
        {
            var finishers = controllerFactory.AppController.GetFinishers();
            return new { finishers = finishers };
        }

        private object GetWinners( ControllerFactory controllerFactory )
        {
            var winners = controllerFactory.AppController.GetWinners();
            return new { winners = winners };
        }

        private object GetTeams(ControllerFactory controllerFactory)
        {
            var teams = controllerFactory.AppController.GetTeamResults();
            return new { teams = teams };
        }

        private void CheckField(string field, string fieldname)
        {
            if ( string.IsNullOrEmpty( field ) )
            {
                throw new Exception( string.Format( "Field '{0}' cannot be empty.", fieldname ) );
            }            
        }
    }
}
