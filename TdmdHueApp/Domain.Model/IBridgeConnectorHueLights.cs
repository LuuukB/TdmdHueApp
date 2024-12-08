using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdmdHueApp.Domain.Services;

namespace TdmdHueApp.Domain.Model
{
    public interface IBridgeConnectorHueLights
    {
        Task SendApiLinkAsync();
        void SetConnectionType(ConnectionType connection);

        Task<string> GetAllLightIDsAsync();

        Task TurnLightOnOffAsync(string lightID, bool isOn);

        Task SetLighColorAsync(string lightId, int hue, int saturation, int brightness, bool isOn);

        Task<string> GetLightInfoSpecificAsync(string lightId);

    }
}
