namespace Lykke.Bil2.BaseTests
{
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
