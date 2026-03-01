using System.Collections.Generic;
using System.Threading.Tasks;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public interface IClimaService
{
    Task<string> ObtenerClimaEntregasAsync(IEnumerable<Entrega> entregas);
}
