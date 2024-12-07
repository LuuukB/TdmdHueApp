using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TdmdHueApp.Domain.Model;

namespace TdmdHueApp.infrastucture
{
    public class BridgeConnector : IBridgeConnectorHueLights
    {
        private static readonly HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost/api/") };//als je met emulator wil connencten gebruik deze anders https://192.168.1.179/api
        private IPreferences _preferences;
       
        public BridgeConnector(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public async Task SendApiLinkAsync() 
        {
            Debug.WriteLine("Send LINK");
            try
            {
                var response = await _httpClient.PostAsJsonAsync("", new
                {
                    devicetype = "my_hue_app#iphone peter"
                });
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(json);

                    ExtractUserName(json);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Foutdetails: {errorContent}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine(httpEx.Message);
            }
            catch (TimeoutException timeoutEx)
            {
                Debug.WriteLine($"Time-out fout: {timeoutEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }



        }

        public void ExtractUserName(string json)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(json);
            var rootArray = jsonDocument.RootElement;
            var rootObject = rootArray[0];
            var successElement = rootObject.GetProperty("success");
            var usernameProperty = successElement.GetProperty("username");
            var userName = usernameProperty.GetString();
            //UserName = usernameProperty.GetString();
            Debug.WriteLine(userName);
            _preferences.Set("username",userName);
        }


        public async Task<string> GetAllLightIDsAsync()
        {
            Debug.WriteLine("GETTING LIGHTSSS");
            try
            {
                var response = await _httpClient.GetAsync($"{_preferences.Get("username", string.Empty)}/lights");
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                return json;
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine(httpEx.Message);
                return "Fout : NetwerkFout";
            }
            catch (Exception ex)
            {
                return "Fout : Onverwachte fout bij ophalen van licht-ID's";
            }

        }

        public async Task TurnLightOnOffAsync(string lightID, bool isOn)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"{_preferences.Get("username", string.Empty)}/lights/{lightID}/state",
                    new { on = isOn }
                    );
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(json);
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"Netwerkfout bij het inschakelen/uitschakelen van licht {lightID}: {httpEx.Message}");
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Er is een onverwachte fout opgetreden bij het On/Off zetten van licht {lightID}: {ex.Message}");
            }
         
        }

        public async Task SetLighColorAsync(string lightId, int hueOrigen, int saturation, int brightness, bool isOn)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                  $"{_preferences.Get("username", string.Empty)}/lights/{lightId}/state",
                  new
                  {
                      on = isOn,
                      sat = saturation,
                      bri = brightness,
                      hue = hueOrigen,

                  }
                  );
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(json);
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"Netwerkfout bij het instellen van kleur voor licht {lightId}: {httpEx.Message}");
            }
            catch (Exception ex)
            { 
                Debug.WriteLine($"Er is een onverwachte fout opgetreden bij het instellen van kleur voor licht {lightId}: {ex.Message}");
            }
        }

        public async Task<string> GetLightInfoSpecificAsync(string lightId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                  $"{_preferences.Get("username", string.Empty)}/lights/{lightId}");

                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(json);
                return json;
            }
            catch (HttpRequestException httpEx)
            {
                return "Fout : Netwerkfout bij ophalen van lichtinformatie";
            }
            catch (Exception ex)
            {
                return "Fout : Onverwachte fout bij ophalen van lichtinformatie";
            }

        }
    }
}
