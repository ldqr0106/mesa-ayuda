namespace MesaAyuda.Models;

/// <summary>
/// Solicitud de ayuda registrada en la mesa de ayuda.
/// El propio modelo protege sus reglas: no puede existir un ticket con datos
/// inválidos y un ticket cerrado no puede cerrarse otra vez.
/// </summary>
public class Ticket
{
    public const int LongitudMaximaTitulo = 100;

    /// <summary>Identificador asignado por el sistema. Solo se fija al crear el ticket.</summary>
    public int Id { get; }

    public string Titulo { get; }

    public string Descripcion { get; }

    public PrioridadTicket Prioridad { get; }

    /// <summary>Estado actual. Solo cambia mediante <see cref="Cerrar"/>.</summary>
    public EstadoTicket Estado { get; private set; }

    public DateTime FechaCreacion { get; }

    public DateTime? FechaCierre { get; private set; }

    /// <summary>
    /// Crea un ticket en estado abierto. Lanza <see cref="ArgumentException"/>
    /// con todos los errores si algún dato no es válido.
    /// </summary>
    public Ticket(int id, string? titulo, string? descripcion, PrioridadTicket prioridad)
    {
        var errores = Validar(titulo, descripcion, prioridad);
        if (errores.Count > 0)
        {
            throw new ArgumentException(string.Join(" ", errores));
        }

        Id = id;
        Titulo = titulo!.Trim();
        Descripcion = descripcion!.Trim();
        Prioridad = prioridad;
        Estado = EstadoTicket.Abierto;
        FechaCreacion = DateTime.Now;
    }

    /// <summary>
    /// Regla de validación del registro. Devuelve la lista de correcciones
    /// necesarias; una lista vacía significa que los datos son válidos.
    /// </summary>
    public static List<string> Validar(string? titulo, string? descripcion, PrioridadTicket prioridad)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(titulo))
        {
            errores.Add("El título no puede estar vacío.");
        }
        else if (titulo.Trim().Length > LongitudMaximaTitulo)
        {
            errores.Add($"El título no puede superar {LongitudMaximaTitulo} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            errores.Add("La descripción no puede estar vacía.");
        }

        if (!Enum.IsDefined(prioridad))
        {
            errores.Add("La prioridad no es válida. Use Baja, Media o Alta.");
        }

        return errores;
    }

    /// <summary>Indica si el ticket puede cerrarse (solo si está abierto).</summary>
    public bool PuedeCerrarse => Estado == EstadoTicket.Abierto;

    /// <summary>
    /// Regla de cambio de estado: abierto → cerrado.
    /// Lanza <see cref="InvalidOperationException"/> si ya estaba cerrado.
    /// </summary>
    public void Cerrar()
    {
        if (!PuedeCerrarse)
        {
            throw new InvalidOperationException($"El ticket #{Id} ya está cerrado.");
        }

        Estado = EstadoTicket.Cerrado;
        FechaCierre = DateTime.Now;
    }
}
