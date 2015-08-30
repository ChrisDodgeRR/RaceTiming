using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;
using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config.Win
{
    class RegistryConfigKey : ConfigKey, IDisposable
    {
        readonly RegistryKey regKey;

        public RegistryConfigKey( string path ) : base( LastKey( path ) )
        {
            var isAbsolutePath = path.StartsWith( Registry.CurrentUser.Name );
            // if absolute path, knock off 'HKEY_CURRENT_USER\'
            var subKeyPath = isAbsolutePath ? path.Substring( startIndex: Registry.CurrentUser.Name.Length + 1 ) : path;

            regKey = Registry.CurrentUser.OpenSubKey( subKeyPath, writable: true );
            if ( regKey == null )
            {
                // create
                regKey = Registry.CurrentUser.CreateSubKey( subKeyPath );
            }
        }

        static string LastKey( string path )
        {
            var index = path.LastIndexOf( '\\' );
            return index == -1 ? path : path.Substring( index + 1 );
        }

        public override IEnumerable<ConfigKey> SubKeys
        {
            get
            {
                var subKeyNames = regKey.GetSubKeyNames();
                foreach ( var name in subKeyNames )
                {
                    yield return new RegistryConfigKey( string.Format( "{0}\\{1}", regKey.Name, name ) );
                }
            }
        }

        public override ConfigKey CreateSubKey( string name )
        {
            return new RegistryConfigKey( string.Format( "{0}\\{1}", regKey.Name, name ) );
        }

        public override IEnumerable<ConfigValue> Values
        {
            get
            {
                var valueNames = regKey.GetValueNames();
                foreach ( var name in valueNames )
                {
                    yield return new RegistryConfigValue( this, name, null );
                }
            }
        }

        public override ConfigValue CreateValue( string name, object value )
        {
            return new RegistryConfigValue( this, name, value );
        }

        public object GetValue( string name )
        {
            return regKey.GetValue( name );
        }

        public void SetValue( string name, object value )
        {
            regKey.SetValue( name, value );
        }

        public void DeleteValue( string name )
        {
            regKey.DeleteValue( name );
        }

        public void Dispose()
        {
            regKey.Close();
        }
    }
}
