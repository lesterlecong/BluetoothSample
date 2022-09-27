using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BluetoothSample.Infrastructure;
using Shiny;
using Shiny.BluetoothLE;

namespace BluetoothSample
{
    public class BleClientDelegate : BleDelegate
    {
        readonly SqliteConnection conn;
        public BleClientDelegate(SqliteConnection conn)
        {
            this.conn = conn;
        }

        public override Task OnAdapterStateChanged(AccessState state)
        {
            return this.conn.InsertAsync(new ShinyEvent
            {
                Text = "BLE Adapter Status",
                Detail = $"New Status: {state}",
            });
        }

        public override Task OnConnected(IPeripheral peripheral)
        {
            return this.conn.InsertAsync(new ShinyEvent
            {
                Text = "Peripheral Connected",
                Detail = peripheral.Name,
                Timestamp = DateTime.Now
            });
        }
    }
}
