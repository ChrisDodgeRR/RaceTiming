using RedRat.RaceTiming.Data.Model;
using System;

namespace RedRat.RaceTiming.Core
{
	public class IndexModel
	{
		public IndexModel()
		{
			// Need to setup empty values in case we haven't loaded data
			CurrentRace = new Race{ Name = "--" };
		}

		public Race CurrentRace { get; set; }
	}
}

