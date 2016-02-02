using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace shipper.Processors.Output
{
    /*using tcp client to talk to redis*/
    class TCPRedisProcessor : IOutputProcessor
    {
        private string _name;
        private string _host;
        private int _db;
        private string _key;
        private bool _status = false;
        private byte[] _bytes = new byte[1024];

        private readonly bool _debug = false;

        private Socket _sender;
        private static ManualResetEvent receiveDone =
        new ManualResetEvent(false);

        public void SetName(string name)
        {
            _name = name;
        }
        public string GetName()
        {
            return _name;
        }

        private bool init()
        {
            System.Console.WriteLine("im connecting to redis now  {0}",DateTime.Now);
            try
            {
                //_sender.Shutdown(SocketShutdown.Both);
                //_sender.Close();
                _sender = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
                _sender.Connect(_host, 6379);
                _sender.Send(Encoding.ASCII.GetBytes("PING\r\n"));

                var bytesRec=_sender.Receive(_bytes);
                if (!Regex.IsMatch(Encoding.ASCII.GetString(_bytes, 0, bytesRec), @"PONG"))
                {
                    return false;
                }

                _sender.Send(Encoding.ASCII.GetBytes("SELECT " + _db + "\r\n"));
                bytesRec = _sender.Receive(_bytes);
                if (!Regex.IsMatch(Encoding.ASCII.GetString(_bytes, 0, bytesRec), @"OK"))
                {
                    return false;
                }

                _sender.Send(Encoding.ASCII.GetBytes("CLIENT SETNAME " + System.Environment.MachineName + "\r\n"));
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                return false;
            }
        }


        public void SetMetadata(string metadata)
        {
            try
            {
                string[] md = metadata.Split('|');
                if (md[0].Length > 0)
                {
                    _host = md[0];
                }
                if (md[1].Length > 0)
                {
                    _db = Int32.Parse(md[1]);
                }
                if (md[2].Length > 0)
                {
                    _key = md[2];
                }

                this.init();


                _status = true;
            }
            catch (Exception ex)
            {
                _status = false;
                System.Console.WriteLine(ex.ToString());
                System.Environment.Exit(0);
            }
        }
        public void ProcessData(string data)
        {
            if (_status && data.Length > 0)
            {
                try
                {
                    if (!_sender.Connected)
                    {
                        System.Console.WriteLine("reconnecting now...");
                        this.init();
                    }
                    else
                    {

                        if (_debug)
                        {
                            System.Console.WriteLine("received-from-tcpredis {0}", data);
                        }


                        _sender.Send(Encoding.ASCII.GetBytes("*3" + "\r\n"
                        +"$5" + "\r\n"
                        + "RPUSH" + "\r\n"
                        +"$" + _key.Length + "\r\n"
                        + _key + "\r\n"
                        + "$" + data.Length + "\r\n"
                        + data + "\r\n"
                        ));
                        
                        _sender.Receive(_bytes);

                        //System.Console.WriteLine(Encoding.ASCII.GetString(_bytes, 0, bytesRec));
                    }  
                }
                catch (System.TimeoutException ex)
                {
                    System.Console.WriteLine("timeout exception caught{0}", ex.ToString());
                    this.init();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            }
        }

        public TCPRedisProcessor(string debug)
        {
            System.Console.WriteLine("tcpredis!!!");
            if (debug.ToUpper().Equals("TRUE"))
            {
                _debug = true;
            }

        }


        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                receiveDone.Set();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 256;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }
    }
}
