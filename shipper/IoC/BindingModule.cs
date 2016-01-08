using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using shipper.Main;
using shipper.Processors.Input;
using shipper.Processors.Output;
using shipper.DataHub;
using Ninject.Extensions.ContextPreservation;
using System.Configuration;



namespace shipper.IoC
{
    class BindingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMain>().To<shipper.Main.Main>().InSingletonScope();
            Bind<IDataHub>().To<DataHub.DataHub>().InSingletonScope().WithConstructorArgument("debug", ConfigurationManager.AppSettings.Get("Debug"));

            Bind<Func<string, IInputProcessor>>().ToMethod(ctx => x => ctx.ContextPreservingGet<IInputProcessor>(x));
            Bind<Func<string, IOutputProcessor>>().ToMethod(ctx => x => ctx.ContextPreservingGet<IOutputProcessor>(x));

            Bind<IInputProcessor>().To<UdpProcessor>().Named("udp").WithConstructorArgument("debug", ConfigurationManager.AppSettings.Get("Debug"));
            Bind<IOutputProcessor>().To<RedisProcessor>().InSingletonScope().Named("redis").WithConstructorArgument("debug", ConfigurationManager.AppSettings.Get("Debug"));
            Bind<IOutputProcessor>().To<TCPRedisProcessor>().InSingletonScope().Named("tcpredis").WithConstructorArgument("debug", ConfigurationManager.AppSettings.Get("Debug"));




        }
    }
}
