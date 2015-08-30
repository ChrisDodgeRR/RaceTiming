using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config.Win
{
    /// <summary>
    /// Configuration provider using the windows registry for storage 
    /// </summary>
    class RegistryConfigurationProvider : ConfigurationProvider
    {
        const string RegistryKeyBase = @"Software\RedRat\";

        public override void SaveChanges()
        {
            // handled by values, so nothing to do
        }

        public override ConfigKey GetOrCreateKey( string path )
        {
            return new RegistryConfigKey( RegistryKeyBase + path );
        }

        public override ConfigValue GetOrCreateValue( string path, string name, object value, bool isReadOnly )
        {
            return new RegistryConfigValue( (RegistryConfigKey)GetOrCreateKey( path ), name, value, isReadOnly );
        }
    }
}
