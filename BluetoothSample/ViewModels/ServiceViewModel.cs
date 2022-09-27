using BluetoothSample.Infrastructure;
using Shiny.BluetoothLE;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Shiny;
using BluetoothSample.Views;
using System.Reactive.Linq;

namespace BluetoothSample.ViewModels
{
    public class ServiceViewModel : BaseViewModel
    {
        public ServiceViewModel(IGattService service)
        {
            this.Title = service.Uuid;

            this.Load = this.LoadingCommand(async () =>
            {
                this.Characteristics = (await service.GetCharacteristicsAsync())
                    .Select(x => new CharacteristicViewModel(x))
                    .ToList();

                this.RaisePropertyChanged(nameof(this.Characteristics));
            });

            this.WhenAnyProperty(x => x.SelectedCharacteristic)
                .Where(x => x != null)
                .SubOnMainThread(async x =>
                {
                    this.SelectedCharacteristic = null;
                    await this.Navigation.PushAsync(new CharacteristicPage
                    {
                        BindingContext = x
                    });
                });
        }

        public string Title { get; set; }
        public ICommand Load { get; }

        public List<CharacteristicViewModel> Characteristics { get; private set; }

        CharacteristicViewModel selected;

        public CharacteristicViewModel SelectedCharacteristic
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
