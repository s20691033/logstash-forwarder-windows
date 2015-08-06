using System.Configuration;

namespace shipper.Configuration
{
    public class OutputProccessorElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string type
        {
            get { return (string)base["type"]; }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("metadata", IsRequired = false)]
        public string metadata
        {
            get { return (string)base["metadata"]; }
        }
    }
}
