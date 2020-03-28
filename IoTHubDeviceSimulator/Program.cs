using IoTHubDeviceSimulator.ViewModels;
using IoTHubDeviceSimulator.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IoTHubDeviceSimulator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var application = new Application();

            var viewModel = new CanvasViewModel();

            var view = new CanvasWindow();
            view.DataContext = viewModel;

            application.Run(view);
        }
    }
}
