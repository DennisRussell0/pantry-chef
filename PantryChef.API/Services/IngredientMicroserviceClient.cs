using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class IngredientMicroserviceClient
{
    private readonly HttpClient _httpClient;

    public IngredientMicroserviceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(string coreIngredient, double similarity)> GetCoreIngredientWithScoreAsync(string ingredient)
    {
        var payload = new { ingredient };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://localhost:5005/parse-ingredient", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var core = doc.RootElement.GetProperty("core_ingredient").GetString() ?? "";
        var sim = doc.RootElement.GetProperty("similarity").GetDouble();
        return (core, sim);
    }
}