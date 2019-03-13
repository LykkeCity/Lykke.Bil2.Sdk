using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lykke.Bil2.BaseTests
{
    /// <summary>
    /// Reads and initializes environment variables specified in the launchsettings.json if any.
    /// Used for local run only.
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/43927955/should-getenvironmentvariable-work-in-xunit-test
    /// </remarks>
    public static class LaunchSettingsReader
    {
        public static void Read()
        {
            try
            {
                var path = Path.Combine("Properties", "launchSettings.json");
                
                using (var file = File.OpenText(path))
                {
                    var reader = new JsonTextReader(file);
                    var jObject = JObject.Load(reader);

                    var variables = jObject
                        .GetValue("profiles")
                        .SelectMany(profiles => profiles.Children())
                        .SelectMany(profile => profile.Children<JProperty>())
                        .Where(prop => prop.Name == "environmentVariables")
                        .SelectMany(prop => prop.Value.Children<JProperty>())
                        .ToList();

                    foreach (var variable in variables)
                    {
                        Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (FileNotFoundException)
            {
            }
        }
    }
}
