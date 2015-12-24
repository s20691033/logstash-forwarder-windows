using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace shipper.Processors.Output
{
    public class RedisProcessor : IOutputProcessor
    {
        private string _name;
        private string _host;
        private int _db;
        private string _key;
        private bool _status = false;
        private ConnectionMultiplexer _redis;
        private IDatabase _redisdb;

        public void SetName(string name)
        {
            _name = name;
        }
        public string GetName()
        {
            return _name;
        }

        public RedisProcessor()
        {
            System.Console.WriteLine("constructor has been called!!!");
        }

        public void SetMetadata(string metadata)
        {
            
            System.Console.WriteLine("redis!!!");
            try
            {
                string[] md = metadata.Split('|');
                if (md[0].Length > 0)
                {
                    _host = md[0];
                }
                if (md[1].Length > 0 )
                {
                    _db = Int32.Parse(md[1]);
                }
                if (md[2].Length > 0)
                {
                    _key = md[2];
                }
                _redis = ConnectionMultiplexer.Connect(_host);
                _redisdb = _redis.GetDatabase(_db);
                _status = true;
            }
            catch(Exception ex)
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
                   
                    if (!_redis.IsConnected)
                    {
                        System.Console.WriteLine("reconnecting now...");
                        _redis.Close(false);
                        _redis = ConnectionMultiplexer.Connect(_host);
                        _redisdb = _redis.GetDatabase(_db);
                    }
                    //var ret = _redisdb.ListRightPush(_key, data, When.Always, CommandFlags.FireAndForget);
                    var ret = _redisdb.ListRightPush(_key, data);
                    //System.Console.WriteLine("returned {0}, {1}",ret,data);
                }
                catch(System.TimeoutException ex)
                {
                    System.Console.WriteLine("timeout exception caught{0}",ex.ToString());
                    _redis.Close();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
