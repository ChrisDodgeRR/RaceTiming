
using System.Runtime.Serialization;
using RedRat.RaceTiming.Core.Config.Unix;

namespace RedRat.Config
{
    [DataContract]
    [KnownType(typeof(FileConfigValue))]
    public abstract class ConfigValue
    {
        public ConfigValue( string name, bool isReadOnly )
        {
            Name = name;
            IsReadOnly = isReadOnly;
        }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Returns the current value of this key
        /// </summary>
        protected abstract object GetValue();

        /// <summary>
        /// Stores the value of this key
        /// </summary>
        protected abstract void SetValue( object newValue );

        [DataMember]
        public object Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                if ( !IsReadOnly ) SetValue( value );
            }
        }

        /// <summary>
        /// Value cast as boolean
        /// </summary>
        public abstract bool BooleanValue { get; }

        public abstract string StringValue { get; }

        /// <summary>
        /// Value cast as integer
        /// </summary>
        public abstract int IntValue { get; }

        public abstract void Delete();
    }
}
