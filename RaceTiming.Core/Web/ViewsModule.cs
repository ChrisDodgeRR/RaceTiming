using Nancy;
using RedRat.RaceTiming.Data.Model;
using System;

namespace RedRat.RaceTiming.Core.Web
{
	public class ViewsModule : NancyModule
	{
		// ToDo: Pass in the app controller.
		public ViewsModule (ControllerFactory controllerFactory)
		{
			Get ["/"] = parameters => {
                return View ["Index", CreateIndexModel(controllerFactory)];
			};
		}

        private IndexModel CreateIndexModel(ControllerFactory controllerFactory)
        {
			var model = new IndexModel ();
            if (controllerFactory.AppController.CurrentRace == null) 
			{
				model.RaceName = " No race loaded";
			} else 
			{
                model.RaceName = controllerFactory.AppController.CurrentRace.Name;
			}
			return model;
		}
	}
}

