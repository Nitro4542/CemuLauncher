using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace cemu_launcher.Data
{
    public static class ConfigLoader
    {
        public static Config LoadConfig()
        {
            try
            {
                string yml = File.ReadAllText("config.yml");

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                return deserializer.Deserialize<Config>(yml);
            }
            catch
            {
                return new Config();
            }
        }
    }
}
