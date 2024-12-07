using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using TdmdHueApp.Domain.Model;
using TdmdHueApp.infrastucture;


namespace TdmdHueApp.Domain.Services
{
    public partial class ViewModel : ObservableObject
    {
        private IBridgeConnectorHueLights BridgeConnector;
       
        public ViewModel(IPreferences preferences, IBridgeConnectorHueLights bridgeConnectorHueLights) 
        {
            BridgeConnector = bridgeConnectorHueLights;
            lamps = new ObservableCollection<Lamp>();
        }
        [ObservableProperty]
        private Lamp _selectedLamp;
        [ObservableProperty]
        private string _lampId;
        [ObservableProperty]
        private bool _isLightOn;
        [ObservableProperty]
        private int _hue;
        [ObservableProperty]
        private int _saturation;
        [ObservableProperty]
        private int _brightness;
        [ObservableProperty]
        private string _infoLamp;
        [ObservableProperty]
        public ObservableCollection<Lamp> lamps;

        [RelayCommand]
        public void SetSelectedLamp(Lamp lamp) {
            SelectedLamp = lamp;
        }

        [RelayCommand]
        public async Task SendApiLink() {
            await BridgeConnector.SendApiLinkAsync();
        }
        [RelayCommand]
        public async Task GetLights() {
            var result = await BridgeConnector.GetAllLightIDsAsync();
            if (result.StartsWith("Fout")) { 
                return;
            }
            Debug.WriteLine("in lghts");
            JsonDocument jsondoc = JsonDocument.Parse(result);
            var root = jsondoc.RootElement;
            Debug.WriteLine("voor array");
            Debug.WriteLine("geparsed");

            foreach (var lampElement in root.EnumerateObject())
            {
                Debug.WriteLine("in ");
                var name = lampElement.Name;
                Debug.WriteLine($"{name}");
                var info = lampElement.Value;
                var baseProperty = info.GetProperty("state");
                var LampId = int.Parse(name);
                var Saturation = baseProperty.GetProperty("sat").GetInt32();
                var IsOn = baseProperty.GetProperty("on").GetBoolean();
                var Brightness = baseProperty.GetProperty("bri").GetInt32();
                var Hue = baseProperty.GetProperty("hue").GetInt32();
                var lamp = new Lamp(LampId, IsOn, Brightness, Saturation, Hue);
                Debug.WriteLine("Lampieess");
                if (!lamps.Contains(lamp))
                {
                    Lamps.Add(lamp);
                }
            }
            
        }
        [RelayCommand]
        public async Task TurnLightOnOffAsync() {
            await BridgeConnector.TurnLightOnOffAsync(LampId, IsLightOn);
        }
        [RelayCommand]
        public async Task SetLightColor()
        {
            int hue = Hue >= 0 && Hue <= 65535 ? Hue : 0;
            int saturation = Saturation >= 0 && Saturation <= 255 ? Saturation : 0;
            int brightness = Brightness >= 0 && Brightness <= 255 ? Brightness : 0;

            if (SelectedLamp == null)
                return;

            await BridgeConnector.SetLighColorAsync(SelectedLamp.LampId.ToString(), SelectedLamp.Hue, SelectedLamp.Saturation, SelectedLamp.Brightness, SelectedLamp.IsOn);
        }

        [RelayCommand]
        public async Task GetSpecificLightInfo()
        {
            var lightInfo = await BridgeConnector.GetLightInfoSpecificAsync(LampId);
            if (lightInfo.StartsWith("Fout")) 
            {
                return;
            } 
            InfoLamp = lightInfo ?? "No info available";
        }

        public static Color GetColorFromHSV(int hue, int saturation, int brightness)
        {
            float h = hue / 65535f;
            float s = saturation / 255f;
            float v = brightness / 255f;

            float r = v;
            float g = v;
            float b = v;

            if (s != 0)
            {
                float h2 = h * 6;
                int i = (int)Math.Floor(h2);
                float f = h2 - i;
                float p = v * (1 - s);
                float q = v * (1 - f * s);
                float t = v * (1 - (1 - f) * s);

                switch (i)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    case 5: r = v; g = p; b = q; break;
                }
            }

            return Color.FromRgb(r, g, b);
        }
        
        public Color CircleColor => GetColorFromHSV((int)Hue, (int)Saturation, (int)Brightness);
    }
}
