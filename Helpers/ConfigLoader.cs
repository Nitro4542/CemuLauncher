using System.IO;
using CemuLauncher.Models;
using YamlDotNet.Serialization;

namespace CemuLauncher.Helpers;

public sealed class ConfigLoader(IDeserializer _deserializer) {
    public async Task<Config> LoadConfigAsync(string path = "config.yml") {
        if (!File.Exists(path))
            return new Config();

        try {
            var content = await File.ReadAllTextAsync(path);
            return _deserializer.Deserialize<Config>(content);
        } catch {
            return new Config();
        }
    }
}
