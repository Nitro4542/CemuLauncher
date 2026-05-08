using CemuLauncher.Models;
using System.IO;
using YamlDotNet.Serialization;

namespace CemuLauncher.Helpers
{
    public sealed class ConfigLoader(IDeserializer deserializer)
    {
        public async Task<Config> LoadConfigAsync(string path = "config.yml")
        {
            if (!File.Exists(path))
                return new Config();

            try
            {
                var content = await File.ReadAllTextAsync(path);
                return deserializer.Deserialize<Config>(content);
            }
            catch
            {
                return new Config();
            }
        }
    }
}
