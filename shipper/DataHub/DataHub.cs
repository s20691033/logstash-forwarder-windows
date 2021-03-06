﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using shipper.Processors.Output;
using shipper.Configuration;

namespace shipper.DataHub
{
    public class DataHub : IDataHub
    {
        private List<IOutputProcessor> _outputprocessors = new List<IOutputProcessor>();
        private readonly Func<string, IOutputProcessor> _outputprocessor;
        private readonly bool _debug = false;

        public DataHub(Func<string, IOutputProcessor> outputprocessor,string debug)
        {
            if (debug.ToUpper().Equals("TRUE"))
            {
                _debug = true;
            }

            
            _outputprocessor = outputprocessor;
            var config = ConfigurationManager.GetSection("Processors");
            if (config != null)
            {
                foreach (OutputProccessorElement opt in ((ProcessorsSection)config).outputProcessors)
                {
                    var op = _outputprocessor(opt.type);
                    op.SetMetadata(opt.metadata);
                    op.SetName(opt.name);
                    _outputprocessors.Add(op);
                }
            }
        }

        public void ProcessData(string data,string dest)
        {
            if (_debug)
            {
                System.Console.WriteLine("received-from-datahub {0}", data);
            }


            foreach (IOutputProcessor op in _outputprocessors)
            {
                if (op.GetName().Equals(dest))
                {
                    op.ProcessData(data);
                }
            }
        }
    }
}
