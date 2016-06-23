using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config.Unix
{
    class FileConfigurationProvider : ConfigurationProvider
    {
        readonly string configFolderName;
        readonly string configFilePath;
        FileConfigKey rootKey;
        private readonly object fileLock = new object();

        DataContractSerializer serializer = new DataContractSerializer( typeof( FileConfigKey ) );

        public FileConfigurationProvider()
        {
            // Personal folder is "Documents" on windows and home directory on linux
            configFolderName = Environment.GetFolderPath( Environment.SpecialFolder.Personal ) + "/.redrat";
            configFilePath = configFolderName + "/racetimingconfig";
        }

        internal void LoadConfig()
        {
            lock ( fileLock )
            {
                var fileInfo = new FileInfo( configFilePath );
                if ( fileInfo.Exists )
                {
                    // Read into mem stream and desrialize from that. As the data is deserialized, it sets values
                    // causing writes back to file.
                    var memoryStream = new MemoryStream();
                    using ( var reader = new FileStream( configFilePath, FileMode.Open ) )
                    {
                        reader.CopyTo( memoryStream );
                    }
                    memoryStream.Position = 0;
                    rootKey = (FileConfigKey) serializer.ReadObject( memoryStream );
                }
                // ensure we have something
                rootKey = rootKey ?? new FileConfigKey( string.Empty );
            }
        }

        void SaveConfig()
        {
            lock ( fileLock )
            {
                // TODO -- honour deleted values
                if ( !Directory.Exists( configFolderName ) )
                {
                    Directory.CreateDirectory( configFolderName );
                }
                if ( !File.Exists(configFilePath) )
                {
                    var fs = File.Create(configFilePath);
                    fs.Close();
                }
                using ( var writer = new FileStream( configFilePath, FileMode.Truncate ) )  // Set to zero size before writing.
                {
                    serializer.WriteObject( writer, rootKey );
                }
            }
        }

        public override void SaveChanges()
        {
            SaveConfig();
        }

        public override ConfigKey GetOrCreateKey( string path )
        {
            var parts = path.Split( new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries );
            var key = (ConfigKey)rootKey;
            foreach ( var part in parts )
            {
                var subKey = key.SubKeys.FirstOrDefault( k => k.Name.Equals( part, StringComparison.InvariantCultureIgnoreCase ) );
                key = subKey ?? key.CreateSubKey( part );
            }
            return key;
        }

        public override ConfigValue GetOrCreateValue( string path, string name, object value, bool isReadOnly )
        {
            var key = GetOrCreateKey( path );
            var configValue = key.Values.FirstOrDefault( v => v.Name.Equals( name, StringComparison.InvariantCultureIgnoreCase ) );
            return configValue ?? key.CreateValue( name, value );
        }
    }
}
