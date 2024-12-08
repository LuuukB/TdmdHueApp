using Moq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TdmdHueApp.Domain.Model;
using TdmdHueApp.Domain.Services;

namespace UnitTestProject
{
    public class TestsHueApp
    {

        private Lamp lamp = new(1, true, 20, 200, 2000);
        private Lamp lamp2 = new(2, false, 10, 100, 1000);
        private Lamp lamp3 = new(1, true, 20, 200, 2000);
        private Lamp lamp4 = new(1, false, 20, 200, 2000);
        private Lamp lamp5 = new(1, true, 10, 200, 2000);
        private Lamp lamp6 = new(1, true, 20, 100, 2000);
        private Lamp lamp7 = new(1, true, 20, 200, 1000);

        [Fact]
        public void CreateLampTest()
        {

            Assert.Equal(1, lamp.LampId);
            Assert.True(lamp.IsOn);
            Assert.Equal(20, lamp.Brightness);
            Assert.Equal(200, lamp.Saturation);
            Assert.Equal(2000, lamp.Hue);
        }       

        [Fact] 
        public void ExtractUsernameTest() {

            var mockPreferences = new Mock<IPreferences>();
            var testJson = " [ { \"success\": { \"username\": \"1028d66426293e821ecfd9ef1a0731df\" } } ]";

            var extractUsername = new ExtractUsername(mockPreferences.Object);

            extractUsername.setUsername(testJson);
            Debug.WriteLine(extractUsername.ToString());
            mockPreferences.Verify(
            p => p.Set("username", "1028d66426293e821ecfd9ef1a0731df",null),
            Times.Once,
            "Username was not correctly stored in preferences."
        );
        }

        [Fact]
        public void setSelectedLampTest() {

            IPreferences preferences = new Mock<IPreferences>().Object;
            IBridgeConnectorHueLights bridgeConnectorHueLights = new Mock<IBridgeConnectorHueLights>().Object;

            ViewModel viewModel = new ViewModel(preferences, bridgeConnectorHueLights);

            viewModel.SetSelectedLamp(lamp);
            Assert.Equal(lamp, viewModel.SelectedLamp);

            viewModel.SetSelectedLamp(lamp2);
            Assert.NotEqual(lamp, viewModel.SelectedLamp);
            Assert.Equal(lamp2,viewModel.SelectedLamp);
        }

        [Fact]
        public async Task SendApiLinkErrorTest() {

            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.SendApiLinkAsync())
                .ReturnsAsync("error: connection failed");

            ViewModel viewModel = new(new Mock<IPreferences>().Object,mockBridgeConnector.Object);

            await viewModel.SendApiLink();

            Assert.Equal("unable to make Connection error: connection failed", viewModel.StatusApp);
            Assert.True(viewModel.IsEmulatorButtonEnabled);
            Assert.False(viewModel.IsBridgeButtonEnabled);

        }

        [Fact]
        public async Task namSendApiLinkSuccesTeste() {
            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.SendApiLinkAsync())
                .ReturnsAsync("success: connection established");

            ViewModel viewModel = new (new Mock<IPreferences>().Object,mockBridgeConnector.Object);

            await viewModel.SendApiLink();

            Assert.Equal("Made a connection success: connection established", viewModel.StatusApp);
            Assert.False(viewModel.IsEmulatorButtonEnabled);
            Assert.False(viewModel.IsBridgeButtonEnabled);
        }

        [Fact]
        public async Task SendApiBridgeErrorTest() {
            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.SendApiLinkAsync())
                .ReturnsAsync("error: connection failed");

            ViewModel viewModel = new(new Mock<IPreferences>().Object, mockBridgeConnector.Object);

            await viewModel.SendApiBridge();

            Assert.Equal("unable to make Connection error: connection failed", viewModel.StatusApp);
            Assert.False(viewModel.IsEmulatorButtonEnabled);
            Assert.True(viewModel.IsBridgeButtonEnabled);
        }

        [Fact]
        public async Task SendApiBridgeSuccesTest() {
            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.SendApiLinkAsync())
                .ReturnsAsync("success: connected");

            ViewModel viewModel = new (new Mock<IPreferences>().Object, mockBridgeConnector.Object);

            await viewModel.SendApiBridge();

            Assert.Equal("Made a connection success: connected", viewModel.StatusApp);
            Assert.False(viewModel.IsEmulatorButtonEnabled);
            Assert.False(viewModel.IsBridgeButtonEnabled);
        }

        [Fact]
        public async Task GetLightsErrorTest() {
            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.GetAllLightIDsAsync())
                .ReturnsAsync("error: connection failed");

            ViewModel viewModel = new (new Mock<IPreferences>().Object, mockBridgeConnector.Object);

            await viewModel.GetLights();

            Assert.Equal("retry Connecting error: connection failed", viewModel.StatusApp);
            Assert.True(viewModel.IsBridgeButtonEnabled);
            Assert.True(viewModel.IsEmulatorButtonEnabled);
            Assert.Empty(viewModel.Lamps);
        }

        [Fact]
        public async Task GetLightsCorrectJsonParse() {
            var jsonResponse = @"
                 {
                     ""1"": { ""state"": { ""on"": true, ""bri"": 200, ""sat"": 150, ""hue"": 30000 } },
                     ""2"": { ""state"": { ""on"": false, ""bri"": 100, ""sat"": 100, ""hue"": 20000 } }
                 }";

            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.GetAllLightIDsAsync())
                .ReturnsAsync(jsonResponse);

            ViewModel vieModel = new ( new Mock<IPreferences>().Object, mockBridgeConnector.Object);

            await vieModel.GetLights();

            Assert.Equal(2, vieModel.Lamps.Count);

            var lamp8 = vieModel.Lamps[0];
            Assert.Equal(1, lamp8.LampId);
            Assert.True(lamp8.IsOn);
            Assert.Equal(200, lamp8.Brightness);
            Assert.Equal(150, lamp8.Saturation);
            Assert.Equal(30000, lamp8.Hue);

            var lamp9 = vieModel.Lamps[1];
            Assert.Equal(2, lamp9.LampId);
            Assert.False(lamp9.IsOn);
            Assert.Equal(100, lamp9.Brightness);
            Assert.Equal(100, lamp9.Saturation);
            Assert.Equal(20000, lamp9.Hue);
        }

        [Fact]
        public async Task GetLightsNoDubbleLamps()
        {
            var jsonResponse = @"
               {
                  ""1"": { ""state"": { ""on"": true, ""bri"": 200, ""sat"": 150, ""hue"": 30000 } },
                  ""1"": { ""state"": { ""on"": true, ""bri"": 200, ""sat"": 150, ""hue"": 30000 } }
                }";

            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.GetAllLightIDsAsync())
                .ReturnsAsync(jsonResponse);

            ViewModel viewModel = new (new Mock<IPreferences>().Object,mockBridgeConnector.Object);

            await viewModel.GetLights();

            Assert.Single(viewModel.Lamps); // Alleen één lamp toegevoegd
        }

        [Fact]
        public async Task SetLightColorCheckNullTest() {
            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            ViewModel viewModel = new (new Mock<IPreferences>().Object,mockBridgeConnector.Object)
            {
                SelectedLamp = null // Geen lamp geselecteerd
            };

            await viewModel.SetLightColor();

            mockBridgeConnector.Verify(b => b.SetLighColorAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task SetLightColorDeviceIsOffTest() {
            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.SetLighColorAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync("error: Device is set to off");

            ViewModel viewModel = new (new Mock<IPreferences>().Object, mockBridgeConnector.Object)
            {
                SelectedLamp = new Lamp(1, true, 200, 150, 30000)
            };

            await viewModel.SetLightColor();

            Assert.Equal("light is turned off", viewModel.StatusApp);
        }

        [Fact]
        public async Task SetLightColorErrorTest()
        {
            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.SetLighColorAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync("error: connection failed");

            ViewModel viewModel = new (new Mock<IPreferences>().Object,mockBridgeConnector.Object)
            {
                SelectedLamp = new Lamp(1, true, 200, 150, 30000)
            };

            await viewModel.SetLightColor();

            Assert.Equal("Retry Connecting error: connection failed", viewModel.StatusApp);
            Assert.True(viewModel.IsBridgeButtonEnabled);
            Assert.True(viewModel.IsEmulatorButtonEnabled);
        }

        [Fact]
        public async Task SetLightColorSuccesTest()
        {
            var mockBridgeConnector = new Mock<IBridgeConnectorHueLights>();
            mockBridgeConnector
                .Setup(b => b.SetLighColorAsync("1", 10, 100, 1000, false))
                .ReturnsAsync("success");

            ViewModel viewModel = new(new Mock<IPreferences>().Object, mockBridgeConnector.Object)
            {
                SelectedLamp = lamp
            };

            await viewModel.SetLightColor();

            Assert.Equal(1, viewModel.SelectedLamp.LampId);
            Assert.Equal(10, viewModel.SelectedLamp.Hue);
            Assert.Equal(100, viewModel.SelectedLamp.Saturation);
            Assert.Equal(1000, viewModel.SelectedLamp.Brightness);
            Assert.False(viewModel.SelectedLamp.IsOn);
        }



    }
}