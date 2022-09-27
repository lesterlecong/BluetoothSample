using BluetoothSample.Infrastructure;
using Shiny;
using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BluetoothSample.ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        readonly IBleManager manager;

        public TestViewModel()
        {
            this.manager = ShinyHost.Resolve<IBleManager>();
            this.Tests = new List<CommandItem>
            { this.Test1()
            };
        }

        public List<CommandItem> Tests { get; }

        CommandItem Test1()
        {
            var cmd = new CommandItem();
            cmd.Text = "Test1";
            cmd.PrimaryCommand = new Command(async () =>
            {

            });

            return cmd;
        }
    }
}
