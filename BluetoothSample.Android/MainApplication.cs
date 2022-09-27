using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Shiny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BluetoothSample.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
    
        public override void OnCreate()
        {
            Console.WriteLine("[lester-debug] MainApplication::OnCreate");
            base.OnCreate();
            this.ShinyOnCreate(new Startup());
        }
    }
}