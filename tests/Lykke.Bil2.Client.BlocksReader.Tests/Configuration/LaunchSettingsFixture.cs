using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
{
    //https://stackoverflow.com/questions/43927955/should-getenvironmentvariable-work-in-xunit-test
    public class LaunchSettingsFixture : IDisposable
    {
        public LaunchSettingsFixture()
        {
            try
            {
                using (var file = File.OpenText("Properties\\launchSettings.json"))
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var vhost = Environment.GetEnvironmentVariable("ENV_INFO");
            var host = Environment.GetEnvironmentVariable("RabbitHost");
            var port = Environment.GetEnvironmentVariable("RabbitPort");
            var username = Environment.GetEnvironmentVariable("RabbitUsername");
            var password = Environment.GetEnvironmentVariable("RabbitPassword");

            RabbitMqTestSettings = new RabbitMqTestSettings()
            {
                Host = host,
                Password = password,
                Port = port,
                Username = username,
                Vhost = vhost
            };
        }

        public RabbitMqTestSettings RabbitMqTestSettings { get; private set; }

        public void Dispose()
        {
            // ... clean up
        }
    }

    public class RabbitMqTestSettings
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Host { get; set; }

        public string Port { get; set; }

        public string Vhost { get; set; }

        public string GetConnectionString()
        {
            return $"amqp://{Username}:{Password}@{Host}:{Port}";
        }
    }
}
