using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipper.Processors.Output
{
    public interface IOutputProcessor
    {
        void SetMetadata(string metadata);
        void ProcessData(string data);
        String GetName();
        void SetName(string name);
    }
}
