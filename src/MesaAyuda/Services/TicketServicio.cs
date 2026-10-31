using MesaAyuda.Models;

namespace MesaAyuda.Services;

/// <summary>
/// Casos de uso de la mesa de ayuda. Conserva los tickets en memoria
/// mientras la aplicación está en ejecución (requisito 8).
/// </summary>
public class TicketServicio
{
    private readonly List<Ticket> _tickets = new();
    private int _siguienteId = 1;

    public int Total => _tickets.Count;

    public int Abiertos => _tickets.Count(t => t.Estado == EstadoTicket.Abierto);

    public int Cerrados => _tickets.Count(t => t.Estado == EstadoTicket.Cerrado);

    /// <summary>
    /// Registra un ticket nuevo en estado abierto. La prioridad llega como texto
    /// (Baja, Media o Alta); cualquier otro valor se rechaza con un mensaje claro.
    /// </summary>
    public ResultadoRegistro Registrar(string? titulo, string? descripcion, string? prioridad)
    {
        var errores = new List<string>();

        var prioridadValida = TryParsePrioridad(prioridad, out var prioridadParseada);
        if (!prioridadValida)
        {
            // Se valida título y descripción con una prioridad válida de relleno
            // para no repetir el mensaje de prioridad dos veces.
            errores.AddRange(Ticket.Validar(titulo, descripcion, PrioridadTicket.Baja));
            errores.Add("La prioridad no es válida. Use Baja, Media o Alta.");
            return ResultadoRegistro.Fallido(errores);
        }

        errores.AddRange(Ticket.Validar(titulo, descripcion, prioridadParseada));
        if (errores.Count > 0)
        {
            return ResultadoRegistro.Fallido(errores);
        }

        var ticket = new Ticket(_siguienteId, titulo, descripcion, prioridadParseada);
        _siguienteId++;
        _tickets.Add(ticket);
        return ResultadoRegistro.Correcto(ticket);
    }

    /// <summary>Devuelve los tickets según el filtro, sin modificar el almacén.</summary>
    public IReadOnlyList<Ticket> Listar(FiltroEstado filtro = FiltroEstado.Todos)
    {
        return filtro switch
        {
            FiltroEstado.Abiertos => _tickets.Where(t => t.Estado == EstadoTicket.Abierto).ToList(),
            FiltroEstado.Cerrados => _tickets.Where(t => t.Estado == EstadoTicket.Cerrado).ToList(),
            _ => _tickets.ToList()
        };
    }

    /// <summary>Busca un ticket por identificador; devuelve null si no existe.</summary>
    public Ticket? ObtenerPorId(int id) => _tickets.FirstOrDefault(t => t.Id == id);

    /// <summary>
    /// Cierra el ticket indicado. Devuelve un mensaje de error si no existe
    /// o ya estaba cerrado, y null si el cambio se realizó.
    /// </summary>
    public string? Cerrar(int id)
    {
        var ticket = ObtenerPorId(id);
        if (ticket is null)
        {
            return $"No existe un ticket con identificador {id}.";
        }

        if (!ticket.PuedeCerrarse)
        {
            return $"El ticket #{id} ya está cerrado.";
        }

        ticket.Cerrar();
        return null;
    }

    private static bool TryParsePrioridad(string? texto, out PrioridadTicket prioridad)
    {
        switch (texto?.Trim().ToLowerInvariant())
        {
            case "baja":
                prioridad = PrioridadTicket.Baja;
                return true;
            case "media":
                prioridad = PrioridadTicket.Media;
                return true;
            case "alta":
                prioridad = PrioridadTicket.Alta;
                return true;
            default:
                prioridad = default;
                return false;
        }
    }
}
