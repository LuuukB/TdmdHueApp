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
        private static HttpClient _httpClient;
        private IPreferences _preferences;
        private ExtractUsername _extractUsername;
       
        public BridgeConnector(IPreferences preferences, ExtractUsername extractUsername, HttpClient httpClient)
        {
            _preferences = preferences;
            _extractUsername = extractUsername;
            _httpClient = httpClient;
        }
        public void SetConnectionType(ConnectionType connectionType)
        {
            if (_httpClient.BaseAddress == null)
            {
                if (connectionType == ConnectionType.Emulator)
                {
                    _httpClient.BaseAddress = new Uri("http://localhost/api/");
                }
                else if (connectionType == ConnectionType.HueLamp)
                {
                    _httpClient.BaseAddress = new Uri("https://192.168.1.179/api/");
                }
            }
            else
            {
                Console.WriteLine($"BaseAddress is already set to: {_httpClient.BaseAddress}");
            }
        }
        public async Task<string> SendApiLinkAsync() 
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

                    _extractUsername.setUsername(json);
                    return json;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Foutdetails: {errorContent}");
                    return errorContent;
                    
                }
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine(httpEx.Message);
                return "error";
            }
            catch (TimeoutException timeoutEx)
            {
                Debug.WriteLine($"Time-out fout: {timeoutEx.Message}");
                return "error";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "error";
            }

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
                return "error Fout : NetwerkFout";
            }
            catch (Exception ex)
            {
                return "error Fout : Onverwachte fout bij ophalen van licht-ID's";
            }

        }


        public async Task<string> SetLighColorAsync(string lightId, int hueOrigen, int saturation, int brightness, bool isOn)
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
                return json;
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"Netwerkfout bij het instellen van kleur voor licht {lightId}: {httpEx.Message}");
                return "error";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Er is een onverwachte fout opgetreden bij het instellen van kleur voor licht {lightId}: {ex.Message}");
                return "error";
            }
        }
        public async Task<string> TurnLightOnOffAsync(string lightID, bool isOn)
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
                return json;
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"Netwerkfout bij het inschakelen/uitschakelen van licht {lightID}: {httpEx.Message}");
                return "error";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Er is een onverwachte fout opgetreden bij het On/Off zetten van licht {lightID}: {ex.Message}");
                return "error";
            }

        }

    }
}
