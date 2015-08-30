using System;
using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config.Win
{
    class RegistryConfigValue : ConfigValue
    {
        readonly RegistryConfigKey parentKey;

        public RegistryConfigValue( RegistryConfigKey parentKey, string name, object value, bool isReadOnly = false ) : base( name, isReadOnly )
        {
            this.parentKey = parentKey;
            // make sure value is set
            if ( GetValue() == null )
            {
                SetValue( value );
            }
        }

        protected override object GetValue()
        {
            return parentKey.GetValue( Name );
            /*
            if ( regKey == null ) return null;

            var obj = regKey.GetValue( Name );
            if ( obj == null )
            {
                Trace.WriteLineIf( RRTrace.regAccessTraceSwitch.TraceInfo,
                                       string.Format( "Get value: {0}\\{1} is null, so returning default: {2}", regKey.Name, Name, value ),
                                       RRTrace.regAccessTraceCategory );
                return value;
            }

            Trace.WriteLineIf( RRTrace.regAccessTraceSwitch.TraceInfo, string.Format( "Get value: {0}\\{1} -> {2}", regKey.Name, Name, obj ), 
                               RRTrace.regAccessTraceCategory );
            return obj;
             */
        }

        protected override void SetValue( object newValue )
        {
            parentKey.SetValue( Name, newValue );
            /*
            regKey.SetValue( Name, newValue );
            Trace.WriteLineIf( RRTrace.regAccessTraceSwitch.TraceInfo, string.Format( "Set value: {0}\\{1} = {2}", regKey.Name, Name, newValue ),
                               RRTrace.regAccessTraceCategory );
             */
        }

        public override bool BooleanValue
        {
            get
            {
                return Value.Equals( "True" );
            }
        }

        public override int IntValue
        {
            get
            {
                return Convert.ToInt32( Value );
            }
        }

        public override string StringValue
        {
            get
            {
                return Value.ToString();
            }
        }

        public override void Delete()
        {
            parentKey.DeleteValue( Name );
        }
    }
}
