using System.Configuration;

namespace shipper.Configuration
{
    public class InputProcessorElement :ConfigurationElement
    {
        [ConfigurationProperty("type",IsRequired=true)]
        public string type
        {
            get { return (string)base["type"]; }
        }

        [ConfigurationProperty("output", IsRequired = true)]
        public string output
        {
            get { return (string)base["output"]; }
        }

        [ConfigurationProperty("metadata", IsRequired = false)]
        public string metadata
        {
            get { return (string)base["metadata"]; }
        }
    }
}
