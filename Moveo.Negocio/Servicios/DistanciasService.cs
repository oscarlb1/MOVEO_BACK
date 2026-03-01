using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class DistanciasService : IDistanciasService
{
    private readonly HttpClient _httpClient;

    public DistanciasService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> ObtenerMatrizDistanciasAsync(IEnumerable<Entrega> entregas)
    {
        var validEntregas = entregas.Where(e => e.Cliente?.Latitud.HasValue == true && e.Cliente?.Longitud.HasValue == true).ToList();

        if (validEntregas.Count < 2)
        {
            return "No hay suficientes puntos válidos para calcular distancias.";
        }

        var coordinates = string.Join(";", validEntregas.Select(e =>
            $"{e.Cliente!.Longitud!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{e.Cliente!.Latitud!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}"));

        try
        {
            var url = $"http://router.project-osrm.org/table/v1/driving/{coordinates}?annotations=distance,duration";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
            return $"Error obteniendo distancias de OSRM: {response.ReasonPhrase}";
        }
        catch (Exception ex)
        {
            return $"Excepción al conectar con OSRM: {ex.Message}";
        }
    }
}
