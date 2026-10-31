using MesaAyuda.Models;

namespace MesaAyuda.Services;

/// <summary>
/// Resultado de intentar registrar un ticket: o bien el ticket creado,
/// o bien la lista de correcciones que debe hacer el usuario.
/// </summary>
public class ResultadoRegistro
{
    public Ticket? Ticket { get; }

    public IReadOnlyList<string> Errores { get; }

    public bool Exitoso => Ticket is not null;

    private ResultadoRegistro(Ticket? ticket, IReadOnlyList<string> errores)
    {
        Ticket = ticket;
        Errores = errores;
    }

    public static ResultadoRegistro Correcto(Ticket ticket) => new(ticket, Array.Empty<string>());

    public static ResultadoRegistro Fallido(IReadOnlyList<string> errores) => new(null, errores);
}
