using System.Collections.Generic;
using System.Runtime.Serialization;
using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config
{
    /// <summary>
    /// A configuration "key"
    /// Keys contain a number of sub-keys and a number of values
    /// </summary>
    [DataContract]
    public abstract class ConfigKey
    {
        public ConfigKey( string name )
        {
            Name = name;
        }

        /// <summary>
        /// Name of this key, i.e. NOT the full path
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        public abstract IEnumerable<ConfigKey> SubKeys { get; }

        public abstract ConfigKey CreateSubKey( string name );

        public abstract IEnumerable<ConfigValue> Values { get; }

        public abstract ConfigValue CreateValue( string name, object value );
    }
}
