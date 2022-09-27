using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BluetoothSample.Infrastructure
{
    public static class Extensions
    {
        public static IDisposable SubOnMainThread<T>(this IObservable<T> observable, Action<T> onAction) =>
            observable.Subscribe(x => Device.BeginInvokeOnMainThread(() => onAction(x)));

        public static IDisposable SubOnMainThread<T>(this IObservable<T> observable, Action<T> onAction, Action<Exception> onError) =>
            observable.Subscribe(x => Device.BeginInvokeOnMainThread(() => onAction(x)), onError);

        public static IDisposable SubOnMainThread<T>(this IObservable<T> observable, Action<T> onAction, Action<Exception> onError, Action onComplete) =>
            observable.Subscribe(
                x => Device.BeginInvokeOnMainThread(() => onAction(x)),
                onError,
                onComplete
            );

        public static void TryFireOnAppearing(this Page page)
            => (page.BindingContext as BaseViewModel)?.OnAppearing();

        public static void TryFireOnDisappearing(this Page page)
            => (page.BindingContext as BaseViewModel)?.OnDisappearing();
    }
}
