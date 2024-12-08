using Moq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TdmdHueApp.Domain.Model;

namespace UnitTestProject
{
    public class UnitTest1
    {
        
        [Fact]
        public void CreateLampTest()
        {
            Lamp lamp1 = new(1, true, 20, 200, 10000);

            Assert.Equal(1, lamp1.LampId);
            Assert.True(lamp1.IsOn);
            Assert.Equal(20, lamp1.Brightness);
            Assert.Equal(200, lamp1.Saturation);
            Assert.Equal(10000, lamp1.Hue);
        }

       

        [Fact] public void Test2() {



            var mockPreferences = new Mock<IPreferences>();
            var testJson = " [ {  \"success\": {\r\n               \"username\": \"1028d66426293e821ecfd9ef1a0731df\"\r\n             }\r\n           }\r\n ]";

            var extractUsername = new ExtractUsername(mockPreferences.Object);

            extractUsername.setUsername(testJson);
            Debug.WriteLine(extractUsername.ToString());
            mockPreferences.Verify(
            p => p.Set("username", "1028d66426293e821ecfd9ef1a0731df",null),
            Times.Once,
            "Username was not correctly stored in preferences."
        );
        }
        
    }
}