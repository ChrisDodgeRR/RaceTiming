using RedRat.RaceTiming.Data.Model;
using System;
using System.Collections.Generic;

namespace RedRat.RaceTiming.Core
{
	public interface IAppController
	{
		bool IsDbOpen { get; }

		void CreateNewRace (Race race, string dbFilename);

		void OpenRace (string dbFilename);

		Race CurrentRace { get; }

		void UpdateCurrentRace (Race newRaceDetails);

		IList<Runner> GetRunners ();

		void LoadCsvFile (string filename);
	}
}

