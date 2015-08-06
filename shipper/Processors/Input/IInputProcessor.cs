using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipper.Processors.Input
{
    interface IInputProcessor
    {
        void SetMetadata(string metadata,string dest);
        void Load();
        void Stop();
    }
}
