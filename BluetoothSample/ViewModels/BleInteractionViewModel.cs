using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Shiny;
using BluetoothSample.Infrastructure;
using System.Threading.Tasks;

namespace BluetoothSample.ViewModels
{
    public class BleInteractionViewModel : BaseViewModel
    {
        IPeripheral peripheral;
        public BleInteractionViewModel(IPeripheral peripheral)
        {
            this.peripheral = peripheral;

            Console.WriteLine("[lester-debug] PeripheralViewModel");
            this.Title = this.peripheral.Name;

            this.peripheral.WhenStatusChanged().Subscribe(connectionState =>
            {
                Console.WriteLine(connectionState.ToString());
            });

            this.ConnectionToggle = new Command(
                async () =>
                {
                    if(this.IsConnected)
                    {
                        this.peripheral.CancelConnection();
                        this.IsConnected = false;
                    }
                    else
                    {
                        
                        ConnectionConfig config = new ConnectionConfig()
                        {
                            AutoConnect = false
                        };

                        try
                        {
                            await this.peripheral.ConnectAsync(config);
                            this.IsConnected = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ConnectAsync ex {ex}");

                        }

                        
                    }
                }
             );

            this.SendCommand = new Command(async () =>
            {
                if (!peripheral.IsConnected())
                {
                    Console.WriteLine("Peripheral is not yet connected");
                    return;
                }
                    

                IGattCharacteristic writeGattCharacteristic;
                IGattCharacteristic readGattCharacteristic;

                try
                {
                    writeGattCharacteristic = await peripheral.GetKnownCharacteristicAsync(
                    "6e400001-b5a3-f393-e0a9-e50e24dcca9e", //service uuid
                    "6e400002-b5a3-f393-e0a9-e50e24dcca9e" //characteristic uuid
                    );

                    readGattCharacteristic = await peripheral.GetKnownCharacteristicAsync(
                    "6e400001-b5a3-f393-e0a9-e50e24dcca9e", //service uuid
                    "6e400003-b5a3-f393-e0a9-e50e24dcca9e" //characteristic uuid
                    );

                    readGattCharacteristic.Notify().Subscribe(x =>
                       {
                           if(!this.IsTerminationRecv)
                           {
                               string data = Encoding.UTF8.GetString(x.Data);
                               if(data.Contains("|"))
                               {
                                   this.GattData.Add(data);
                                   this.IsTerminationRecv = true;
                               }
                               else
                               {
                                   if(!this.GattData.Contains(data))
                                   {
                                       this.GattData.Add(data);
                                   }
                               }
                           }
                           


                           if(this.IsTerminationRecv && !this.IsRecvDataCompleted)
                           {
                               this.IsRecvDataCompleted = true;
                               this.DeviceFeedback = String.Join("", GattData);
                               Console.WriteLine("Read Data:{0}", String.Join("", GattData));
                           }
                           //Console.WriteLine("Read Gatt: {0}", Encoding.UTF8.GetString(x.Data));
                       }
                     
                        
                    );

                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"GattCharacteristic ex {ex}");
                    return;
                }

                Console.WriteLine($"[lester-debug]: WriteChar: CanWriteWithResponse: {writeGattCharacteristic.CanWriteWithResponse()}");
                Console.WriteLine($"[lester-debug]: WriteChar: CanWriteWithoutResponse: {writeGattCharacteristic.CanWriteWithoutResponse()}");
                Console.WriteLine($"[lester-debug]: WriteChar: CanRead: {writeGattCharacteristic.CanRead()}");
                Console.WriteLine($"[lester-debug]: ReadChar: CanWriteWithResponse: {readGattCharacteristic.CanWriteWithResponse()}");
                Console.WriteLine($"[lester-debug]: ReadChar: CanWriteWithoutResponse: {readGattCharacteristic.CanWriteWithoutResponse()}");
                Console.WriteLine($"[lester-debug]: ReadChar: CanRead: {readGattCharacteristic.CanRead()}");
                await Task.Delay(500);

                if(this.MessageValue.IsEmpty())
                {
                    Console.WriteLine("The Editor is empty!");
                }
                else
                {
                    try
                    {
                        await writeGattCharacteristic.WriteAsync(Encoding.ASCII.GetBytes(this.MessageValue), true);
                        this.IsTerminationRecv = false;
                        this.IsRecvDataCompleted = false;
                        this.DeviceFeedback = String.Empty;
                        this.GattData.Clear();



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Send Message ex {ex}");
                        return;
                    }
                }

            });

        }

        protected List<string> GattData { get; set; } = new List<string>();

        string deviceFeedback;
        public string DeviceFeedback { 
            get => this.deviceFeedback;
            private set => this.Set(ref this.deviceFeedback, value);
        }
        protected bool IsTerminationRecv { get; set; } = false;

        protected bool IsRecvDataCompleted { get; set; } = false;

        public string Title { get; set; }

        public string MessageValue { get; set; }

        bool connected = false;
        public bool IsConnected
        {
            get => this.connected;
            private set => this.Set(ref this.connected, value);
        }

        public ICommand ConnectionToggle { get; }
        public ICommand SendCommand { get; }
    }
}
