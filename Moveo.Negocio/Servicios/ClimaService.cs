using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class ClimaService : IClimaService
{
    private readonly HttpClient _httpClient;

    public ClimaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> ObtenerClimaEntregasAsync(IEnumerable<Entrega> entregas)
    {
        var validEntregas = entregas.Where(e => e.Cliente?.Latitud.HasValue == true && e.Cliente?.Longitud.HasValue == true).ToList();

        if (!validEntregas.Any())
        {
            return "No hay coordenadas válidas para consultar el clima.";
        }

        var result = new List<string>();

        foreach (var entrega in validEntregas)
        {
            try
            {
                var lat = entrega.Cliente!.Latitud!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var lon = entrega.Cliente!.Longitud!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

                var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);

                    if (doc.RootElement.TryGetProperty("current_weather", out var currentWeather))
                    {
                        var temp = currentWeather.GetProperty("temperature").GetDouble();
                        var weathercode = currentWeather.GetProperty("weathercode").GetInt32();

                        string condition = GetWeatherConditionString(weathercode);
                        result.Add($"Entrega {entrega.Id}: {temp}°C, {condition}");
                    }
                    else
                    {
                        result.Add($"Entrega {entrega.Id}: Datos de clima no disponibles");
                    }
                }
                else
                {
                    result.Add($"Entrega {entrega.Id}: Error API Open-Meteo");
                }
            }
            catch (Exception)
            {
                result.Add($"Entrega {entrega.Id}: Excepción al consultar el clima");
            }
        }

        return string.Join(" | ", result);
    }

    // Basado en los códigos WMO de Open-Meteo https://open-meteo.com/en/docs
    private string GetWeatherConditionString(int code)
    {
        if (code == 0) return "Despejado";
        if (code >= 1 && code <= 3) return "Nublado";
        if (code >= 51 && code <= 67) return "Lluvia";
        if (code >= 71 && code <= 77) return "Nieve";
        if (code >= 95 && code <= 99) return "Tormenta";
        return "Desconocido";
    }
}
