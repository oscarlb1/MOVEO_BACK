using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public class IaOptimizationService : IIaOptimizationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string GeminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    public IaOptimizationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["API_KEY_GEMINI"] ?? Environment.GetEnvironmentVariable("API_KEY_GEMINI") ?? configuration["Gemini:ApiKey"] ?? string.Empty;
    }

    public async Task<OptimizacionIaResponseDto?> OptimizarRutaAsync(string prompt)
    {
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "TU_API_KEY_AQUI")
        {
            throw new Exception("La API Key de Gemini no está configurada.");
        }

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = prompt } }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var content = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

        var url = $"{GeminiApiUrl}?key={_apiKey}";
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al llamar a Gemini: {response.StatusCode} - {errorContent}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            return ParseGeminiResponse(jsonResponse);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al parsear la respuesta JSON de Gemini. Respuesta raw: {jsonResponse}", ex);
        }
    }

    private OptimizacionIaResponseDto? ParseGeminiResponse(string jsonResponse)
    {
        using var doc = JsonDocument.Parse(jsonResponse);
        var candidates = doc.RootElement.GetProperty("candidates");

        if (candidates.GetArrayLength() > 0)
        {
            var content = candidates[0].GetProperty("content");
            var parts = content.GetProperty("parts");

            if (parts.GetArrayLength() > 0)
            {
                var text = parts[0].GetProperty("text").GetString();

                if (!string.IsNullOrEmpty(text))
                {
                    // Limpiar markdown (a veces Gemini lo envuelve en ```json ... ``` incluso forzando application/json)
                    text = text.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                               .Replace("```", "")
                               .Trim();

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<OptimizacionIaResponseDto>(text, options);
                }
            }
        }

        return null;
    }
}
