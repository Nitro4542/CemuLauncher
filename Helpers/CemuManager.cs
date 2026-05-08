using System.Net.Http;
using System.Text.Json;

namespace CemuLauncher.Helpers;

public static class CemuManager {
    private const string LatestCommitUrl = "https://api.github.com/repos/cemu-project/Cemu/commits/main";

    public static async Task<string?> GetLatestCommitAsync(HttpClient httpClient) {
        try {
            using var response = await httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Get, LatestCommitUrl));

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            if (doc.RootElement.TryGetProperty("sha", out var shaElement))
                return shaElement.GetString();

            return null;
        } catch {
            return null;
        }
    }
}
