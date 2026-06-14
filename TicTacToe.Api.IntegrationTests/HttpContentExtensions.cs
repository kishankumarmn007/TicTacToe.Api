using System.Text.Json;

namespace TicTacToe.Api.IntegrationTests
{
    public static class HttpContentExtensions
    {
        public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
