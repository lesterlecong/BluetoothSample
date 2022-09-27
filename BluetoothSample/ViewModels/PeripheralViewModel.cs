using BluetoothSample.Infrastructure;
using BluetoothSample.Views;
using Shiny;
using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace BluetoothSample.ViewModels
{
    public class PeripheralViewModel : BaseViewModel
    {
        public PeripheralViewModel(IPeripheral peripheral)
        {
            this.Title = peripheral.Name;

            this.Load = this.LoadingCommand(async () =>
            {
               this.Services = (await peripheral.GetServicesAsync())
                    .Select(x => new ServiceViewModel(x)) 
                    .ToList();

                this.RaisePropertyChanged(nameof(this.Services));
            });

            this.WhenAnyProperty(x => x.SelectedService)
                .Where(x => x != null)
                .SubOnMainThread(async x =>
                {
                    this.SelectedService = null;
                    await this.Navigation.PushAsync(new ServicePage
                    {
                        BindingContext = x
                    });
                });
        }

        public string Title { get; set; }

        public ICommand Load { get; }

        public List<ServiceViewModel> Services { get; private set; }

        ServiceViewModel selected;

        public ServiceViewModel SelectedService
        {
            get => this.selected;
            set
            {
                this.selected = value;
                this.RaisePropertyChanged();
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            this.Load.Execute(null);
        }
    }
}
