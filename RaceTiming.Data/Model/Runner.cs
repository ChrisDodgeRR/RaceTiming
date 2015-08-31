
using System;
using Volante;

namespace RedRat.RaceTiming.Data.Model
{
    public class Runner : Persistent
    {
        public enum GenderEnum { Male, Female }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Club { get; set; }
        public string Team { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int Number { get; set; }

        public override string ToString()
        {
            return string.Format( "{0} {1} - {2}", FirstName, LastName, Gender );
        }
    }
}
