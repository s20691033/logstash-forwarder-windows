using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;


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

        Socket _sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);


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
            try
            {
                //_sender.Shutdown(SocketShutdown.Both);
                //_sender.Close();
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
                    _sender.Send(Encoding.ASCII.GetBytes("*3" + "\r\n"));
                    _sender.Send(Encoding.ASCII.GetBytes("$5" + "\r\n"));
                    _sender.Send(Encoding.ASCII.GetBytes("RPUSH" + "\r\n"));
                    _sender.Send(Encoding.ASCII.GetBytes("$"+_key.Length + "\r\n"));
                    _sender.Send(Encoding.ASCII.GetBytes(_key + "\r\n"));
                    _sender.Send(Encoding.ASCII.GetBytes("$" + data.Length + "\r\n"));
                    _sender.Send(Encoding.ASCII.GetBytes(data + "\r\n"));
                    //System.Console.WriteLine("returned {0}",data);
                    var bytesRec = _sender.Receive(_bytes);
                    //System.Console.WriteLine(Encoding.ASCII.GetString(_bytes, 0, bytesRec));
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

        public TCPRedisProcessor()
        {
            System.Console.WriteLine("tcpredis!!!");

        }
    }
}
