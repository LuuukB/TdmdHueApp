using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TdmdHueApp.Domain.Model
{
    public interface IBridgeConnectorHueLights
    {
        Task<string> SendApiLinkAsync();
        void SetConnectionType(ConnectionType connection);

        Task<string> GetAllLightIDsAsync();
        Task<string> TurnLightOnOffAsync(string lightID, bool isOn);

        Task<string> SetLighColorAsync(string lightId, int hue, int saturation, int brightness, bool isOn);

    }
}
