using System.Configuration;

namespace shipper.Configuration
{
    [ConfigurationCollection(typeof(InputProcessorElement), AddItemName = "InputProcessor")]
    public class InputProcessorsCollection : ConfigurationElementCollection
    {
        
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InputProcessorElement)element);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new InputProcessorElement();
        }

        public InputProcessorElement this[int idx]
        {
            get { return (InputProcessorElement)BaseGet(idx); }
        }
    }
}
