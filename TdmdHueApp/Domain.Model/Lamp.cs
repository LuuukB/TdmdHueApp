using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TdmdHueApp.Domain.Model
{
    public class Lamp
    {
        public int? LampId { get; set; }

        public bool IsOn { get; set; }
        public int Brightness { get; set; }
        public int Saturation { get; set; }
        public int Hue { get; set; }

        public Lamp(int id, bool ison, int bri, int sat, int hue)
        {
            LampId = id;
            IsOn = ison;
            Brightness = bri;
            Saturation = sat;
            Hue = hue;
        }
        public override bool Equals(object obj)
        {
            if (obj is Lamp otherLamp)
            {
                return LampId == otherLamp.LampId &&
                       IsOn == otherLamp.IsOn &&
                       Brightness == otherLamp.Brightness &&
                       Saturation == otherLamp.Saturation &&
                       Hue == otherLamp.Hue;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LampId, IsOn, Brightness, Saturation, Hue);
        }
    }
}
