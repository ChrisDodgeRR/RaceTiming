using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
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
                    return Response.AsJson( GetRaceInfo( controllerFactory ) );
                }
                catch ( Exception ex )
                {
                    return Response.AsJson(ex.Message, HttpStatusCode.InternalServerError);
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
                return Response.AsJson( new {runner = runner}, statusCode);
            };

            Get["/runners"] = parameters => Response.AsJson( GetRunners( controllerFactory ) );

            Get["/results"] = parameters =>
            {
                try
                {
                    var resultsToList = 0;
                    int.TryParse( Request.Query["resultsToList"], out resultsToList );
                    if ( resultsToList == 0 )
                    {
                        resultsToList = int.MaxValue;
                    }
                    return Response.AsJson( GetResults( controllerFactory, resultsToList ) );
                }
                catch ( Exception ex )
                {
                    return HandleException(ex);
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
                return Response.AsJson(new { raceResult = raceResult }, statusCode);
            };

            Post["/updateresult"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;

                try
                {
                    var newResult = this.Bind<NewResult>();

                    var result = new Result()
                    {
                        Position = newResult.Position,
                        RaceNumber = newResult.RaceNumber,
                        Time = new TimeSpan( newResult.Hours, newResult.Minutes, newResult.Seconds ),
                    };
                    appController.UpdateResult( result );
                    return Response.AsJson(message, statusCode);
                }
                catch (Exception ex)
                {
                    return HandleException(ex);
                }
            };

            Post["/addresult"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;

                try
                {
                    var newResult = this.Bind<NewResult>();

                    var result = new Result()
                    {
                        Position = newResult.Position,
                        RaceNumber = newResult.RaceNumber,
                        Time = new TimeSpan( newResult.Hours, newResult.Minutes, newResult.Seconds ),
                    };
                    appController.InsertResult( result );

                    return Response.AsJson( message, statusCode );
                }
                catch ( Exception ex )
                {
                    return HandleException( ex );
                }
            };

            Post["/deleteresult"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;

                try
                {
                    var deleteResult = this.Bind<DeleteResult>();

                    // Neither selected
                    if ( !deleteResult.DeleteNumber && !deleteResult.DeleteTime )
                    {
                        throw new Exception("Nothing selected to delete...");
                    }

                    // Both selected
                    if ( deleteResult.DeleteNumber && deleteResult.DeleteTime )
                    {
                        appController.DeleteResultAtPosition( deleteResult.Position );
                    }

                    // Delete just the number - shifting numbers down
                    else if ( deleteResult.DeleteNumber && !deleteResult.DeleteTime )
                    {
                        appController.DeleteRunnerNumberShiftDown(deleteResult.Position);
                    }

                    // Delete the result time, shifting times down
                    else if (!deleteResult.DeleteNumber && deleteResult.DeleteTime)
                    {
                        appController.DeleteTimeShiftDown(deleteResult.Position);
                    }

                    return Response.AsJson(message, statusCode);
                }
                catch (Exception ex)
                {
                    return HandleException( ex );
                }
            };

            Get["/finishers"] = parameters => Response.AsJson( GetFinishers( controllerFactory ) );

            Get["/winners"] = parameters => Response.AsJson( GetWinners( controllerFactory ) );

            Get["/teams"] = parameters => Response.AsJson( GetTeams( controllerFactory ) );

            Post["/addrunner"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;

                try
                {
                    var newRunner = this.Bind<NewRunner>();

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
                    runner.ToUpperCase();

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
                    return Response.AsJson( message, statusCode );
                }
                catch ( Exception ex )
                {
                    return HandleException(ex);
                }
            };

            Post["/updaterunner"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;

                try
                {
                    var newRunner = this.Bind<NewRunner>();

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

                    int newNumber = 0;
                    var haveNewNumber = false;
                    if ( !string.IsNullOrEmpty( newRunner.NewNumber ) )
                    {
                        if ( !int.TryParse( newRunner.NewNumber, out newNumber ) )
                        {
                            throw new Exception( "The format of the new race number is not correct." );
                        }
                        haveNewNumber = true;
                    }

                    // Update
                    var runner = new Runner
                    {
                        Number = number,
                        FirstName = newRunner.FirstName,
                        LastName = newRunner.LastName,
                        Gender = ( newRunner.Gender == "Female" ) ? GenderEnum.Female : GenderEnum.Male,
                        DateOfBirth = DateTime.Parse( newRunner.DoB ),
                        Email = newRunner.Email,
                        Club = newRunner.Club,
                        Team = newRunner.Team,
                        Urn = newRunner.Urn,
                        HaveNewNumber = haveNewNumber,
                        NewNumber = newNumber,
                    };
                    runner.ToUpperCase();

                    appController.DbService.UpdateRunner( runner );

                    return Response.AsJson( message, statusCode );
                }
                catch ( Exception ex )
                {
                    return HandleException( ex );
                }
            };

            Post["/deleterunner"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;

                try
                {
                    var number = this.Bind<RunnerNumber>();

                    appController.DbService.DeleteRunner(number.Number);
                    Trace.WriteLineIf( AppController.traceSwitch.TraceInfo,
                        string.Format( "Runner with number {0} deleted.", number.Number ) );

                    return Response.AsJson( message, statusCode );
                }
                catch ( Exception ex )
                {
                    return HandleException(ex);
                }
            };

            Post["/addfinishposition"] = ( x ) =>
            {
                var appController = controllerFactory.AppController;
                var message = "";
                var statusCode = HttpStatusCode.OK;
                try
                {
                    var posResult = this.Bind<PositionResult>();

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
                    return Response.AsJson( message, statusCode );
                }
                catch ( Exception ex )
                {
                    return HandleException( ex );
                }
            };
        }

        protected Response HandleException(Exception ex)
        {
            var aex = ex as AggregateException;
            if (aex != null)
            {
                var str = AggregateExceptionToString(aex, true);
                Trace.WriteLineIf( AppController.traceSwitch.TraceError, str);
                return Response.AsJson(str, HttpStatusCode.InternalServerError);
            }

            Trace.WriteLineIf( AppController.traceSwitch.TraceError, ex );
            return Response.AsJson(ex.Message, HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Converts an AggregateException to a printable form, including inner exceptions etc.
        /// </summary>
        public static string AggregateExceptionToString(AggregateException aex, bool includeStackTrace)
        {
            var sb = new StringBuilder();

            aex.Handle(ex =>
            {
                if (ex is TargetInvocationException)
                {
                    while (ex.InnerException != null)
                    {
                        sb.Append(ex.InnerException.Message + "\n");
                        if (includeStackTrace) sb.Append(ex.InnerException.StackTrace + "\n");
                        ex = ex.InnerException;
                    }
                }
                else
                {
                    sb.Append(ex.Message + "\n");
                    if (includeStackTrace) sb.Append(ex.StackTrace + "\n");
                }
                return true;
            });

            return sb.ToString();
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
        protected object GetResults(ControllerFactory controllerFactory, int number)
        {
            var results = controllerFactory.AppController.GetResults();
			var raceResults = results.OrderByDescending(r => r.Position).Take( number ).Select(r => new
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
