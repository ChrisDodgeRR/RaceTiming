using RedRat.Config;

namespace RedRat.RaceTiming.Core.Config
{
    public class Options
    {
        // Registry key for Race Timing application options. Stored in 'Software\RedRat'.
        public const string raceTimeingConfigLocationName = "RaceTiming";

        protected const string reopenLastFileConfigName = "ReopenLastFile";
        protected ConfigValue reopenLastFileConfigValue;

        protected const string lastFilenameConfigName = "LastFile";
        protected ConfigValue lastFilenameConfigValue;

        public Options()
        {
            reopenLastFileConfigValue = Configuration.Provider.GetOrCreateValue(raceTimeingConfigLocationName, reopenLastFileConfigName, "False", false);
            lastFilenameConfigValue = Configuration.Provider.GetOrCreateValue(raceTimeingConfigLocationName, lastFilenameConfigName, "", false);
        }

        public bool ReopenLastFile
        {
            get { return reopenLastFileConfigValue.BooleanValue; }
            set { reopenLastFileConfigValue.Value = value; }
        }

        public string LastFile
        {
            get { return lastFilenameConfigValue.StringValue; }
            set { lastFilenameConfigValue.Value = value; }
        }
    }
}
