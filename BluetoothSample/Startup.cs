using System;
using System.Collections.Generic;
using System.Text;
using BluetoothSample.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shiny;


namespace BluetoothSample
{
    public class Startup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services, IPlatform platform)
        {
            Console.WriteLine("[lester-debug] Startup!");
            services.AddSingleton<SqliteConnection>();
            services.UseBleClient<BleClientDelegate>();
        }
    }
}
