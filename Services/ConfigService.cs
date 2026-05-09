using System.IO;
using CemuLauncher.Models;
using YamlDotNet.Serialization;

namespace CemuLauncher.Services;

public sealed class ConfigService(IDeserializer deserializer) {
    private Config? _cached;

    public async Task<Config> GetAsync() {
        return _cached ??= await LoadAsync(Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "CemuLauncher", "config.yml"));
    }

    private async Task<Config> LoadAsync(string path) {
        if (!File.Exists(path))
            return new Config();

        try {
            var content = await File.ReadAllTextAsync(path);
            return deserializer.Deserialize<Config>(content);
        } catch {
            return new Config();
        }
    }
}
