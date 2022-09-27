#nullable enable
using BluetoothSample.Infrastructure;
using BluetoothSample.Views;
using Shiny;
using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BluetoothSample.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        readonly IBleManager? bleManager;
        IDisposable? scanSub;

        public ScanViewModel()
        {
            bleManager = ShinyHost.Resolve<IBleManager>();
            this.IsScanning = bleManager?.IsScanning ?? false;
            this.CanControlAdapterState = bleManager?.CanControlAdapterState() ?? false;

            this.WhenAnyProperty(x => x.SelectedPeripheral)
                .Skip(1)
                .Where(x => x != null)
                .Subscribe(async x =>
                {
                    this.SelectedPeripheral = null;
                    this.StopScan();
                    await this.Navigation.PushAsync(new PeripheralPage
                    {
                        BindingContext = new PeripheralViewModel(x.Peripheral)
                    });
                });

            this.TestCommand = new Command(() =>
            {
                Console.WriteLine("[lester-debug] Press me more!!!");
            });

            this.NavToTest = this.NavigateCommand<TestPage>();

            this.ToggleAdapterState = new Command(
                async () =>
                {
                    Console.WriteLine("[lester-debug] Toggle AdapterState");
                    if (bleManager == null)
                    {
                        await this.Alert("Platform Not Supported");
                    }
                    else
                    {
                        var status = await bleManager.RequestAccess();
                        if (status == AccessState.Available)
                        {
                            await bleManager.TrySetAdapterState(false);
                        }
                        else
                        {
                            await bleManager.TrySetAdapterState(true);
                        }
                    }
                }
            );

            this.ScanToggle = new Command(
                async () =>
                {

                    if (bleManager == null)
                    {
                        await this.Alert("Platform Not Supported");
                        return;
                    }

                    if (this.IsScanning)
                    {
                        this.StopScan();
                    }
                    else
                    {
                        this.Peripherals.Clear();
                        this.IsScanning = true;

                        this.scanSub = bleManager
                            .Scan()
                            .Buffer(TimeSpan.FromSeconds(1))
                            .Where(x => x?.Any() ?? false)
                            .SubOnMainThread(
                                results =>
                                {
                                    var list = new List<PeripheralItemViewModel>();
                                    foreach (var result in results)
                                    {
                                        
                                        Console.WriteLine("[lester-debug] result:{0}", result.Peripheral.Name);
                                       
                                        var peripheral = this.Peripherals.FirstOrDefault(x => x.Uuid == result.Peripheral.Uuid);

                                        

                                        if (peripheral == null)
                                        {
                                            peripheral = list.FirstOrDefault(x => x.Uuid == result.Peripheral.Uuid);

                                            if(peripheral != null)
                                            {
                                                Console.WriteLine("[lester-debug] Peripheral {0} already exists", peripheral.Uuid);
                                                peripheral.Update(result);
                                            }
                                            else
                                            {
                                                peripheral = new PeripheralItemViewModel(result.Peripheral);

                                                Console.WriteLine("[lester-debug] Adding: {0}", result.Peripheral.Uuid);
                                                
                                                peripheral.Update(result);
                                                Console.WriteLine("[lester-debug] Service count: {0}", peripheral.ServiceCount);
                                                //if(peripheral.Connectable.ToLower() == "true")
                                                //{
                                                    list.Add(peripheral);
                                                //}
                                                
                                                
                                                    
                                            }
                                        }
                                        
                                    }

                                    if (list.Any())
                                    {
                                        foreach (var item in list)
                                            this.Peripherals.Add(item);
                                    }
                                },
                                ex => this.Alert(ex.ToString(), "ERROR")
                            );
                    }
                }

            );
        }

        public ICommand TestCommand { get; }
        public ICommand NavToTest { get; }
        public ICommand ScanToggle { get; }
        public ICommand ToggleAdapterState { get; }
        public bool CanControlAdapterState { get; }
        public ObservableCollection<PeripheralItemViewModel> Peripherals { get; } = new ObservableCollection<PeripheralItemViewModel>();
        private List<string> peripheralUUID { get; } = new List<string>();

        PeripheralItemViewModel? selected;

        public PeripheralItemViewModel? SelectedPeripheral
        {
            get => this.selected;
            set
            {
                this.selected = value;
                this.RaisePropertyChanged();
            }
        }
       

        bool scanning;
        public bool IsScanning
        {
            get => this.scanning;
            private set => this.Set(ref this.scanning, value);
        }

        void StopScan()
        {
            this.scanSub?.Dispose();
            this.scanSub = null;
            this.IsScanning = false;
        }
    }
}
