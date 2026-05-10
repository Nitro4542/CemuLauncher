using System.IO;
using System.Text;
using CemuLauncher.Models;
using YamlDotNet.Serialization;

namespace CemuLauncher.Services;

public sealed class ConfigService(IDeserializer deserializer, ISerializer serializer) {
    private Config? _cached;

    public async Task<Config> GetAsync() {
        return _cached ??= await LoadAsync(Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "CemuLauncher", "config.yml"));
    }

    private async Task<Config> LoadAsync(string path) {
        if (!File.Exists(path)) {
            var config = new Config();
            var yaml = serializer.Serialize(config);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllBytesAsync(path, Encoding.UTF8.GetBytes(yaml));

            return config;
        }

        try {
            var content = await File.ReadAllTextAsync(path);
            return deserializer.Deserialize<Config>(content);
        } catch {
            return new Config();
        }
    }
}
