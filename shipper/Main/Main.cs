using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using shipper.Configuration;
using shipper.Processors.Input;

namespace shipper.Main
{
    class Main :IMain
    {
        private readonly Func<string, IInputProcessor> _inputprocessor;
        private List<IInputProcessor> _inputprocessors = new List<IInputProcessor>();


        public Main(Func<string, IInputProcessor> inputprocessor)
        {
            _inputprocessor = inputprocessor;
        }

        public void Start()
        {
            var config = ConfigurationManager.GetSection("Processors");
            if (config != null)
            {
                foreach(InputProcessorElement ipt in ((ProcessorsSection)config).inputProcessors)
                {
                    var ip = _inputprocessor(ipt.type);
                    ip.SetMetadata(ipt.metadata,ipt.output);
                    _inputprocessors.Add(ip);
                    ip.Load();
                }
            }
        }

        public void Stop()
        {
            System.Console.WriteLine("exiting...");
            foreach (IInputProcessor ip in _inputprocessors)
            {
                ip.Stop();
            }
        }
    }
}
