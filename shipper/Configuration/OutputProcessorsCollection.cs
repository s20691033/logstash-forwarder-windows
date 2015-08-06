using System.Configuration;

namespace shipper.Configuration
{
    [ConfigurationCollection(typeof(OutputProccessorElement), AddItemName = "OutputProcessor")]
    public class OutputProcessorsCollection : ConfigurationElementCollection
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OutputProccessorElement)element);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new OutputProccessorElement();
        }

    }
}
