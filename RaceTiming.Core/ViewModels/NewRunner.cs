﻿
namespace RedRat.RaceTiming.Core.ViewModels
{
    /// <summary>
    /// Data posted from web page when adding a new runner.
    /// </summary>
    public class NewRunner
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DoB { get; set; }
        public string Email { get; set; }
        public string Club { get; set; }
        public string Urn { get; set; }
        public string Team { get; set; }
        public string Number { get; set; }

        // If the runner's number is being changed
        public string NewNumber { get; set; }
    }
}
