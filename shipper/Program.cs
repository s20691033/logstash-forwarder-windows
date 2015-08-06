using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Ninject;
using shipper.IoC;
using shipper.Main;

namespace shipper
{
    class Program
    {
        static void Main()
        {
            HostFactory.Run(x => {
                x.Service<IMain>(s => {
                    var kernel = new StandardKernel(new BindingModule());
                    s.ConstructUsing(name => kernel.Get<IMain>());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.SetDescription("test");
                x.SetDisplayName("shipper");
                x.SetServiceName("shipper");
            });
        }
    }
}
