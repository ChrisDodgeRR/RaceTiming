
using RedRat.RaceTiming.Data.Model;
using Volante;

namespace RedRat.RaceTiming.Data
{
    public class RtDatabaseRoot : Persistent
    {
        public IIndex<string, Race> raceNameIndex;
        public IIndex<string, Runner> runnerFirstNameIndex;
        public IIndex<string, Runner> runnerLastNameIndex;
        public IIndex<int, Runner> runnerNumberIndex;
    }
}
