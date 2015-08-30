
using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config
{
    public abstract class ConfigurationProvider
    {
        /// <summary>
        /// Facilitates top-down saving of configuration
        /// Useful for providers which must save the entire configuration at once, e.g. a file provider
        /// </summary>
        public abstract void SaveChanges();

        public abstract ConfigKey GetOrCreateKey(string path);

        public abstract ConfigValue GetOrCreateValue( string path, string name, object value, bool isReadOnly );

        public ConfigValue GetOrCreateValue( string path, string name, object value )
        {
            return GetOrCreateValue( path, name, value, isReadOnly: false );
        }
    }
}