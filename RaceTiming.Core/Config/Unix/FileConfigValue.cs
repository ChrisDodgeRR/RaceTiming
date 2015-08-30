using System;
using System.Runtime.Serialization;
using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config.Unix
{
    [DataContract]
    class FileConfigValue : ConfigValue
    {
        [DataMember]
        object currentValue;

        public FileConfigValue( string name, object value, bool isReadOnly = false ) : base( name, isReadOnly )
        {
            currentValue = value;
        }

        protected override object GetValue()
        {
            return currentValue;
        }

        protected override void SetValue( object newValue )
        {
            currentValue = newValue;
            Configuration.Provider.SaveChanges();
        }

        public override bool BooleanValue
        {
            get
            {
                if ( currentValue is string )
                {
                    return ((string)currentValue).Equals( "true", StringComparison.CurrentCultureIgnoreCase );
                }
                return (bool)currentValue;
            }
        }

        public override int IntValue
        {
            get
            {
                return (int)currentValue;
            }
        }

        public override string StringValue
        {
            get
            {
                return (string)currentValue;
            }
        }

        public bool IsDeleted { get; private set; }

        public override void Delete()
        {
            IsDeleted = true;
            Configuration.Provider.SaveChanges();
        }
    }
}
