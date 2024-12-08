
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TdmdHueApp.Domain.Model
{
    public class ExtractUsername
    {
        private IPreferences _preferences;
        public ExtractUsername(IPreferences preferences) { _preferences = preferences; }

        public void setUsername(string json) {
            JsonDocument jsonDocument = JsonDocument.Parse(json);
            var rootArray = jsonDocument.RootElement;
            var rootObject = rootArray[0];
            var userName = rootObject.GetProperty("success").GetProperty("username").GetString();

            Debug.WriteLine(userName);
            _preferences.Set("username", userName);
        }
    }
}
