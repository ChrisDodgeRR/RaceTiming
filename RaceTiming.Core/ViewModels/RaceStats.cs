namespace RedRat.RaceTiming.Core.ViewModels
{
    public class RaceStats
    {
        public struct RunnerBreakdown
        {
            public int Total { get; set; }
            public int Male { get; set; }
            public int Female { get; set; }
        }

        public RunnerBreakdown NumberEntrants { get; set; }
        public RunnerBreakdown NumberFinishers { get; set; }
        public RunnerBreakdown NumberAffiliatedEntrants { get; set; }
        public RunnerBreakdown NumberUnaffiliatedEntrants { get; set; }
    }
}
