using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using shipper.DataHub;
using System.Net;
using System.Net.Sockets;


namespace shipper.Processors.Input
{
    class UdpProcessor :IInputProcessor
    {
        private readonly IDataHub _datahub;
        private string _metadata;
        private string _dest;
        public volatile bool _done;
        private UdpClient listener;


        private readonly bool _debug = false;

        public UdpProcessor(IDataHub datahub, string debug)
        {
            _datahub = datahub;
            _done = false;
            if (debug.ToUpper().Equals("TRUE"))
            {
                _debug = true;
            }
        }

        public void SetMetadata(string metadata,string dest)
        {
            _metadata = metadata;
            _dest = dest;
        }

        public void Stop()
        {
            _done = true;
            try
            {
                listener.Close();
            }
            catch (Exception ex)
            {
                //
            }
        }


        public void Load()
        {
            (new Thread(() => {
                try
                {
                    listener = new UdpClient(Int32.Parse(_metadata));
                    listener.Client.ReceiveBufferSize = Int32.MaxValue;
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, Int32.Parse(_metadata));
                    while (!_done)
                    {
                        
                        byte[] bytes = listener.Receive(ref groupEP);
                        //Console.WriteLine("Received broadcast from {0} :\n {1}\n",groupEP.ToString(),Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                        lock (_datahub)
                        {
                            _datahub.ProcessData(Encoding.ASCII.GetString(bytes, 0, bytes.Length), _dest);
                            if (_debug)
                            {
                                System.Console.WriteLine("received-from-udp {0}", Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                            }
                        }
                    }
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    Console.WriteLine("terminating thread...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            })).Start();

        }

    }
}
