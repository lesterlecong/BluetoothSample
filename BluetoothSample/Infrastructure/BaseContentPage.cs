using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BluetoothSample.Infrastructure
{
    public class BaseContentPage : ContentPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.TryFireOnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.TryFireOnDisappearing();
        }
    }
}
