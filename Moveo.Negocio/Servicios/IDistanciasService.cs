using System.Collections.Generic;
using System.Threading.Tasks;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public interface IDistanciasService
{
    Task<string> ObtenerMatrizDistanciasAsync(IEnumerable<Entrega> entregas);
}
