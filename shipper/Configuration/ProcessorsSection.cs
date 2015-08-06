using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace shipper.Configuration
{
    public class ProcessorsSection : ConfigurationSection
    {
        [ConfigurationProperty("InputProcessors", IsRequired = true)]
        public InputProcessorsCollection inputProcessors
        {
            get { return base["InputProcessors"] as InputProcessorsCollection; }
        }

        [ConfigurationProperty("OutProcessors", IsRequired = true)]
        public OutputProcessorsCollection outputProcessors
        {
            get { return base["OutProcessors"] as OutputProcessorsCollection; }
        }



    }
}
