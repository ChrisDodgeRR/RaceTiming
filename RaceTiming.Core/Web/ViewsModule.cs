using Nancy;
using RedRat.RaceTiming.Data.Model;
using System;

namespace RedRat.RaceTiming.Core.Web
{
	public class ViewsModule : NancyModule
	{
		// ToDo: Pass in the app controller.
		public ViewsModule ()
		{
			
			Get ["/"] = parameters => {
				return View ["Index", 
					new IndexModel { CurrentRace = new Race { Name = "some race" }}];
			};
		}
	}
}

