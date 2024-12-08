using Moq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TdmdHueApp.Domain.Model;

namespace UnitTestProject
{
    public class TestsHueApp
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

        [Fact] 
        public void EqualLampTest() {
            Lamp lamp1 = new(1, true, 20, 200, 2000);
            Lamp lamp2 = new(2, false, 10, 100, 1000);
            Lamp lamp3 = new(1, true, 20, 200, 2000);
            Lamp lamp4 = new(1, false, 20, 200, 2000);
            Lamp lamp5 = new(1, true, 10, 200, 2000);
            Lamp lamp6 = new(1, true, 20, 100, 2000);
            Lamp lamp7 = new(1, true, 20, 200, 1000);

            Assert.True(lamp1.Equals(lamp3));
            Assert.False(lamp1.Equals(lamp2));
            Assert.False(lamp1.Equals(lamp4));
            Assert.False(lamp1.Equals(lamp5));
            Assert.False(lamp1.Equals(lamp6));
            Assert.False(lamp1.Equals(lamp7));
        }

        [Fact]
        public void GetHasCodeLampTest()
        {

            var lamp = new Lamp (1, true, 100, 50, 200 );
            var lamp2 = new Lamp (1, true, 100, 50, 200 );
            var lamp3 = new Lamp (2, false, 50, 25, 100 );

            var hash1 = lamp.GetHashCode();
            var sameAsHash1 = lamp.GetHashCode();
            var hash2 = lamp2.GetHashCode();
            var hash3 = lamp3.GetHashCode();

            Assert.Equal(hash1, sameAsHash1); // 2x hetzelfde hasen zou zelfde hash moeten geven
            Assert.Equal(hash1, hash2); //verschillende objecten met zelfde inhoud zou zelfde hash moeten geven
            Assert.NotEqual(hash1, hash3); // verschillende objecten met verschillende inhoud zou verschillende hash moeten geven

        }

       

        [Fact] public void ExtractUsernameTest() {

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
        
    }
}