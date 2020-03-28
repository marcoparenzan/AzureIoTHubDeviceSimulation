using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Input;

namespace IoTHubDeviceSimulator.ViewModels
{
    public class CanvasViewModel : INotifyPropertyChanged
    {
        public CanvasViewModel()
        {
            Play = new RelayCommand(async arg =>
            {
                await PlayImpl(this.Strokes);
            });
            Strokes = new StrokeCollection();

            this.deviceClient = DeviceClient.CreateFromConnectionString("");
        }

        private async Task PlayImpl(StrokeCollection strokes)
        {
            var stroke = strokes.First();
            var xmin = stroke.StylusPoints.Min(xx => xx.X); // t=0
            var xmax = stroke.StylusPoints.Max(xx => xx.X); // t=24
            var ymin = stroke.StylusPoints.Min(xx => xx.Y); // temp=0
            var ymax = stroke.StylusPoints.Max(xx => xx.Y); // temp = 400

            foreach (var sp in stroke.StylusPoints.OrderBy(xx => xx.X))
            {
                var tsec = ((sp.X - xmin) * 24 / (xmax - xmin))*3600;
                var data = DateTime.Today.AddDays(-1).AddSeconds(tsec);
                var temp = (ymax - sp.Y) * 400 / (ymax - ymin);

                var msg = new
                {
                    Date = data,
                    Temp = temp,
                    DeviceId = "device1"
                };
                var json = JsonSerializer.Serialize(msg);
                var bytes = Encoding.UTF8.GetBytes(json);
                var message = new Message(bytes);

                await deviceClient.SendEventAsync(message);

                await Task.Delay(1000);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = default)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand Play { get; set; }

        public StrokeCollection Strokes { get; set; }

        private DeviceClient deviceClient;
    }
}
