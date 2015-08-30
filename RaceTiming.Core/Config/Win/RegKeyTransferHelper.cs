
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace RedRat.RaceTiming.Core.Config.Win
{
    /// <summary>
    /// In V2.X and earlier versions of the RedRat code, device information was stored in HKLM registry hive.
    /// This is awkward as it requires elevated permissions to access from Vista onwards. So from V3.X, keys are
    /// stored in HKCU. A disadvantage of this is that device name settings etc. are only seen by the user
    /// that sets them.
    /// 
    /// This class helps transfer keys from HLM to HKCU. A problem is that if running on 64-bit systems, the
    /// older versions of software were 32-bit, so had the keys mapped to the Wow6432Node. We can only get at
    /// this using p-invoke calls as we assume that this is now being run in 64-bit mode on x64 machines as 
    /// the 32-bit keys can't be read via .NET calls.
    /// 
    /// It supports execution in both .NET 4 and .NET 3.5 and lower, despite API changes.
    /// </summary>
    class RegKeyTransferHelper
    {
        #region P-Invoke defs
        enum RegWow64Options
        {
            None = 0,
            KEY_WOW64_64KEY = 0x0100,
            KEY_WOW64_32KEY = 0x0200
        }

        enum RegistryRights
        {
            ReadKey = 131097,
            WriteKey = 131078
        }

        [DllImport( "advapi32.dll", CharSet = CharSet.Auto )]
        static extern int RegOpenKeyEx( IntPtr hKey, string subKey, int ulOptions, int samDesired, out int phkResult );

        #endregion

        /// <summary>
        /// Gets whether we are running as a 64-bit process.
        /// </summary>
        static bool Is64BitProcess
        {
            get { return IntPtr.Size == 8 ? true : false; }
        }

        /// <summary>
        /// Open a registry key using the Wow64 node instead of the default 32-bit node.
        /// </summary>
        /// <param name="parentKey">Parent key to the key to be opened.</param>
        /// <param name="subKeyName">Name of the key to be opened</param>
        /// <param name="writable">Whether or not this key is writable</param>
        /// <param name="options">32-bit node or 64-bit node</param>
        /// <returns></returns>
        static RegistryKey OpenSubKey( RegistryKey parentKey, string subKeyName, bool writable, RegWow64Options options )
        {
            //Sanity check
            if ( parentKey == null || GetRegistryKeyHandle( parentKey ) == IntPtr.Zero )
            {
                return null;
            }

            // Set rights
            var rights = (int)RegistryRights.ReadKey;
            if ( writable )
            {
                rights = (int) RegistryRights.WriteKey;
            }

            // Call the native function
            int subKeyHandle, result = RegOpenKeyEx( GetRegistryKeyHandle( parentKey ), subKeyName, 0, rights | (int)options, out subKeyHandle );

            // If we error return null
            if ( result != 0 )
            {
                return null;
            }

            // Get the key represented by the pointer returned by RegOpenKeyEx
            var subKey = PointerToRegistryKey( (IntPtr)subKeyHandle, writable, false, options );
            return subKey;
        }

        /// <summary>
        /// Get a pointer to a registry key.
        /// </summary>
        /// <param name="registryKey">Registry key to obtain the pointer of.</param>
        /// <returns>Pointer to the given registry key.</returns>
        static IntPtr GetRegistryKeyHandle( RegistryKey registryKey )
        {
            // Get the type of the RegistryKey
            var registryKeyType = typeof( RegistryKey );

            // Get the FieldInfo of the 'hkey' member of RegistryKey
            var fieldInfo = registryKeyType.GetField( "hkey", BindingFlags.NonPublic | BindingFlags.Instance );

            // Get the handle held by hkey
            var handle = (SafeHandle)fieldInfo.GetValue( registryKey );

            // Get the unsafe handle
            var dangerousHandle = handle.DangerousGetHandle();
            return dangerousHandle;
        }

        /// <summary>
        /// Get a registry key from a pointer.
        /// </summary>
        /// <param name="hKey">Pointer to the registry key</param>
        /// <param name="writable">Whether or not the key is writable.</param>
        /// <param name="ownsHandle">Whether or not we own the handle.</param>
        /// <returns>Registry key pointed to by the given pointer.</returns>
        static RegistryKey PointerToRegistryKey( IntPtr hKey, bool writable, bool ownsHandle, RegWow64Options options )
        {
            // Get the BindingFlags for private and public constructors
            const BindingFlags constructorFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            // Get the Type for the SafeRegistryHandle
            var safeRegistryHandleType = typeof( Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid ).Assembly.GetType( "Microsoft.Win32.SafeHandles.SafeRegistryHandle" );

            // Get the array of types matching the args of the ctor we want
            var safeRegistryHandleCtorTypes = new Type[] { typeof( IntPtr ), typeof( bool ) };

            //G et the constructor info for our object
            var safeRegistryHandleCtorInfo = safeRegistryHandleType.GetConstructor( constructorFlags, null, safeRegistryHandleCtorTypes, null );
            // Invoke the constructor, getting us a SafeRegistryHandle
            var safeHandle = safeRegistryHandleCtorInfo.Invoke( new Object[] { hKey, ownsHandle } );

            // Get the type of a RegistryKey
            var registryKeyType = typeof( RegistryKey );

            // Get the array of types matching the args of the ctor we want.
            // .NET 4: The RegistryKey constructor (private) has changed in .NET4, so we need to adjust for this.
            //         As it uses a type not in lower versions of .NET, we can't explicitly reference it as we
            //         need to compile in lower versions.
            var registryKeyConstructorTypes = Environment.Version.Major == 4 
                ? new Type[] { safeRegistryHandleType, typeof( bool ), Type.GetType( "Microsoft.Win32.RegistryView" ) } // .NET 4 constructor version
                : new Type[] { safeRegistryHandleType, typeof( bool ) };                                                // .NET 3.5 and below

            // Get the constructor info for our object
            var registryKeyCtorInfo = registryKeyType.GetConstructor( constructorFlags, null, registryKeyConstructorTypes, null );
            
            // Invoke the constructor, getting us a RegistryKey
            if ( Environment.Version.Major == 4 )
            {
                // .NET 4: Need to create the right enum type without explicit reference to it so that we
                //         can compile in .NET 3.5.
                var rvType = Type.GetType( "Microsoft.Win32.RegistryView" );
                var enumVal = Enum.Parse( rvType, "Default", true );
                switch ( options )
                {
                    case RegWow64Options.KEY_WOW64_32KEY:
                        enumVal = Enum.Parse( rvType, "Registry32", true );
                        break;

                    case RegWow64Options.KEY_WOW64_64KEY:
                        enumVal = Enum.Parse( rvType, "Registry64", true );
                        break;
                }

                return (RegistryKey)registryKeyCtorInfo.Invoke( new Object[] { safeHandle, writable, enumVal } );
            }
            return (RegistryKey)registryKeyCtorInfo.Invoke( new Object[] { safeHandle, writable } );
        }
    }
}
