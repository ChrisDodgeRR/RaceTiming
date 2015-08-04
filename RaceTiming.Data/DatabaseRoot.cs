
using RedRat.RaceTiming.Data.Model;
using Volante;

namespace RedRat.RaceTiming.Data
{
    public class DatabaseRoot : Persistent
    {
        public IIndex<string, Race> raceNameIndex;
        public IIndex<string, Runner> runnerFirstNameIndex;
    }
}
