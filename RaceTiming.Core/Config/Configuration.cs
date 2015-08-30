using RedRat.RaceTiming.Core.Config.Unix;
using RedRat.RaceTiming.Core.Config.Win;
using RedRat.RaceTiming.Core.Util;

namespace RedRat.RaceTiming.Core.Config
{
    public static class Configuration
    {
        static ConfigurationProvider theProvider;

        public static ConfigurationProvider Provider
        {
            get
            {
                return theProvider ?? CreateProvider();
            }
        }

        static ConfigurationProvider CreateProvider()
        {
            if ( CurrentOS.IsWindows )
            {
                theProvider = new RegistryConfigurationProvider();
            }
            else
            {
                theProvider = new FileConfigurationProvider();
                ( (FileConfigurationProvider) theProvider ).LoadConfig();
            }
            return theProvider;
        }
    }
}
