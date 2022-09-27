using System;
using BluetoothSample.Infrastructure;
using Shiny.BluetoothLE;

namespace BluetoothSample.ViewModels
{
    public class PeripheralItemViewModel : BaseViewModel
    {
        public PeripheralItemViewModel(IPeripheral peripheral)
            => this.Peripheral = peripheral;

        public override bool Equals(object obj)
            => this.Peripheral.Equals(obj);
      
        public IPeripheral Peripheral { get; }

        public string Uuid => this.Peripheral.Uuid;

        public string Name { get; private set; }
        public bool IsConnected { get; private set; }
        public int Rssi { get; private set; }
        public string Connectable { get; private set; }
        public int ServiceCount { get; private set; }
    
        public string LocalName { get; private set; }
        public int TxPower { get; private set; }

        public void Update(ScanResult result)
        {

            Console.WriteLine("PeripheralItemViewModel::Update: Name: '{0}'", this.Peripheral.Name);
            if(string.IsNullOrEmpty(this.Peripheral.Name) || string.IsNullOrWhiteSpace(this.Peripheral.Name))
            {
                //Console.WriteLine("Empty name!!!");
                this.Name = "<Unknown>";
            }
            else
            {
                this.Name = this.Peripheral.Name;
            }
            
            this.Rssi = result.Rssi;

            var ad = result.AdvertisementData;
            this.ServiceCount = ad.ServiceUuids?.Length ?? 0;
            this.Connectable = ad?.IsConnectable?.ToString() ?? "Unknown";
            this.LocalName = ad.LocalName;
            this.TxPower = ad.TxPower ?? 0;
            this.RaisePropertyChanged(String.Empty);
        }
    }
}
