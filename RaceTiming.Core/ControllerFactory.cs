using System;
using RedRat.RaceTiming.Data;

namespace RedRat.RaceTiming.Core
{
    /// <summary>
    /// Used to provide a single instance of controllers and services.
    /// </summary>
    public class ControllerFactory
    {
        private static AppController appController;
        private static DbService dbService;

        public AppController AppController
        {
            get
            {
                if (appController == null)
                {
                    appController = new AppController(this.DbService);
                }
                return appController;
            }
        }

        public DbService DbService
        {
            get
            {
                if (dbService == null)
                {
                    dbService = new DbService();
                }
                return dbService;
            }
        }
    }
}

