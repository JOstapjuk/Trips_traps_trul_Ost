using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;

namespace Trips_traps_trul_Ost
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
