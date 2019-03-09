using System;

namespace Lykke.Bil2.BaseTests
{
    /// <summary>
    /// Reads Rabbit MQ settings from the environment variable
    /// </summary>
    public static class RabbitMqSettingsReader
    {
        public static RabbitMqTestSettings Read()
        {           
            return new RabbitMqTestSettings
            {
                Vhost = ReadRequiredVariable("ENV_INFO"),
                Host = ReadRequiredVariable("RabbitHost"),
                Port = ReadRequiredVariable("RabbitPort"),
                Username = ReadRequiredVariable("RabbitUsername"),
                Password = ReadRequiredVariable("RabbitPassword")
            };
        }

        private static string ReadRequiredVariable(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"{name} environment variable is not specified. Use launchsettings.json to specify env variables and LaunchSettingsReader.Read to read it for local runs, or specify env variables in the build parameters in TeamCity");
            }

            return value;
        }
    }
}
