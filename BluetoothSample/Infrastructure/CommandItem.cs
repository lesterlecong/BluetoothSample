#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BluetoothSample.Infrastructure
{
    public class CommandItem
    {
        public string? Text { get; set; }
        public string? Detail { get; set; }
        public string? ImageUri { get; set; }
        public object? Data { get; set; }

        public ICommand? PrimaryCommand { get; set; }
        public ICommand? SecondaryCommand { get; set; }
    }
}
