using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipper.DataHub
{
    public interface IDataHub
    {
        void ProcessData(string data,string dest);
    }
}
