using System.Collections.Generic;
using System.Runtime.Serialization;
using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config.Unix
{
    [DataContract]
    class FileConfigKey : ConfigKey
    {
        // the actual data
        [DataMember]
        List<ConfigKey> subKeyData;

        [DataMember]
        List<ConfigValue> valueData;

        public FileConfigKey( string name ) : base( name )
        {
            subKeyData = new List<ConfigKey>();
            valueData = new List<ConfigValue>();
        }

        public override IEnumerable<ConfigKey> SubKeys
        {
            get
            {
                return subKeyData;
            }
        }

        public override ConfigKey CreateSubKey( string name )
        {
            var subKey = new FileConfigKey( name );
            subKeyData.Add( subKey );
            return subKey;
        }

        public override IEnumerable<ConfigValue> Values
        {
            get
            {
                return valueData;
            }
        }

        public override ConfigValue CreateValue( string name, object value )
        {
            var configValue = new FileConfigValue( name, value );
            valueData.Add( configValue );
            return configValue;
        }
    }
}
