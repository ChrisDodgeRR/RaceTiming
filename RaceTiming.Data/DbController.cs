
using System;
using RedRat.RaceTiming.Data.Model;
using Volante;

namespace RedRat.RaceTiming.Data
{
    public class DbController : IDisposable
    {
        private IDatabase db;
        private DatabaseRoot dbRoot;

        public void Dispose()
        {
            Close();
        }

        public void Open(string dbFileName)
        {
            if ( db != null ) return;

            db = DatabaseFactory.CreateDatabase();
            db.Open(dbFileName);

            if (db.Root != null)
            {
                dbRoot = (DatabaseRoot)db.Root;
            }
            else
            {
                // Only create root the first time
                dbRoot = new DatabaseRoot
                {
                    runnerFirstNameIndex = db.CreateIndex<string, Runner>( IndexType.NonUnique )
                };
                db.Root = dbRoot;
                // changing the root marks database as modified but it's
                // only modified in memory. Commit to persist changes to disk.
                db.Commit();
            }
        }

        public void Close()
        {
            if ( db != null )
            {
                //db.Close();
                db = null;
            }
        }

        public void TestInsertData()
        {
            var runner = new Runner {FirstName = "Chris", LastName = "Dodge"};
            dbRoot.runnerFirstNameIndex.Put( runner.FirstName, runner );
            db.Commit();
        }

        public void PrintRunners()
        {
            foreach (var runner in dbRoot.runnerFirstNameIndex)
            {
                Console.WriteLine("{0}: {1}", runner.FirstName, runner.LastName);
            }
        }
    }
}
