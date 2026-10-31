using MesaAyuda.Models;
using MesaAyuda.Services;

namespace MesaAyuda.UI;

/// <summary>
/// Interfaz de consola de la mesa de ayuda. Recibe la entrada y la salida
/// por parámetro para poder probar el flujo completo sin teclado real.
/// </summary>
public class MenuConsola
{
    private readonly TicketServicio _servicio;
    private readonly TextReader _entrada;
    private readonly TextWriter _salida;
    private FiltroEstado _filtro = FiltroEstado.Todos;

    public MenuConsola(TicketServicio servicio, TextReader entrada, TextWriter salida)
    {
        _servicio = servicio;
        _entrada = entrada;
        _salida = salida;
    }

    public void Ejecutar()
    {
        var continuar = true;
        while (continuar)
        {
            MostrarMenu();
            var opcion = Leer("Opción: ");
            if (opcion is null)
            {
                break; // fin de la entrada
            }

            _salida.WriteLine();
            switch (opcion.Trim())
            {
                case "1": RegistrarTicket(); break;
                case "2": ListarTickets(); break;
                case "3": ConsultarTicket(); break;
                case "4": CerrarTicket(); break;
                case "5": CambiarFiltro(); break;
                case "6": CargarDatosDeMuestra(); break;
                case "0": continuar = false; break;
                default: _salida.WriteLine("Opción no válida. Elija un número del menú."); break;
            }
        }

        _salida.WriteLine("Hasta luego.");
    }

    private void MostrarMenu()
    {
        _salida.WriteLine();
        _salida.WriteLine("=== MESA DE AYUDA ===");
        _salida.WriteLine($"Total: {_servicio.Total} | Abiertos: {_servicio.Abiertos} | Cerrados: {_servicio.Cerrados}");
        _salida.WriteLine($"Filtro de la lista: {_filtro}");
        _salida.WriteLine("1. Registrar ticket");
        _salida.WriteLine("2. Listar tickets");
        _salida.WriteLine("3. Consultar ticket");
        _salida.WriteLine("4. Cerrar ticket");
        _salida.WriteLine("5. Cambiar filtro (todos / abiertos / cerrados)");
        _salida.WriteLine("6. Cargar datos de muestra");
        _salida.WriteLine("0. Salir");
    }

    private void RegistrarTicket()
    {
        _salida.WriteLine("--- Registrar ticket ---");
        var titulo = Leer("Título: ");
        var descripcion = Leer("Descripción: ");
        var prioridad = Leer("Prioridad (Baja, Media, Alta): ");

        var resultado = _servicio.Registrar(titulo, descripcion, prioridad);
        if (resultado.Exitoso)
        {
            _salida.WriteLine($"Ticket #{resultado.Ticket!.Id} registrado correctamente (estado: {resultado.Ticket.Estado}).");
            return;
        }

        _salida.WriteLine("No se pudo registrar el ticket. Corrija lo siguiente:");
        foreach (var error in resultado.Errores)
        {
            _salida.WriteLine($" - {error}");
        }
    }

    private void ListarTickets()
    {
        var tickets = _servicio.Listar(_filtro);
        _salida.WriteLine($"--- Tickets ({_filtro}) ---");

        if (tickets.Count == 0)
        {
            _salida.WriteLine("No hay tickets para mostrar con este filtro.");
            return;
        }

        _salida.WriteLine($"{"ID",-5}{"Título",-42}Estado");
        foreach (var t in tickets)
        {
            _salida.WriteLine($"{t.Id,-5}{Recortar(t.Titulo, 40),-42}{t.Estado}");
        }
    }

    private void ConsultarTicket()
    {
        var ticket = PedirTicket("Identificador del ticket a consultar: ");
        if (ticket is null)
        {
            return;
        }

        _salida.WriteLine($"--- Ticket #{ticket.Id} ---");
        _salida.WriteLine($"Título:      {ticket.Titulo}");
        _salida.WriteLine($"Descripción: {ticket.Descripcion}");
        _salida.WriteLine($"Prioridad:   {ticket.Prioridad}");
        _salida.WriteLine($"Estado:      {ticket.Estado}");
        _salida.WriteLine($"Creado:      {ticket.FechaCreacion:dd/MM/yyyy HH:mm}");
        if (ticket.FechaCierre is not null)
        {
            _salida.WriteLine($"Cerrado:     {ticket.FechaCierre:dd/MM/yyyy HH:mm}");
        }
    }

    private void CerrarTicket()
    {
        var ticket = PedirTicket("Identificador del ticket a cerrar: ");
        if (ticket is null)
        {
            return;
        }

        var error = _servicio.Cerrar(ticket.Id);
        _salida.WriteLine(error ?? $"Ticket #{ticket.Id} cerrado correctamente.");
    }

    private void CambiarFiltro()
    {
        var texto = Leer("Filtro (todos, abiertos, cerrados): ")?.Trim().ToLowerInvariant();
        switch (texto)
        {
            case "todos": _filtro = FiltroEstado.Todos; break;
            case "abiertos": _filtro = FiltroEstado.Abiertos; break;
            case "cerrados": _filtro = FiltroEstado.Cerrados; break;
            default:
                _salida.WriteLine("Filtro no válido. Escriba: todos, abiertos o cerrados.");
                return;
        }

        _salida.WriteLine($"Filtro cambiado a: {_filtro}. Los tickets registrados no se modificaron.");
    }

    private void CargarDatosDeMuestra()
    {
        // Datos genéricos: no identifican personas ni solicitudes reales.
        var muestras = new (string Titulo, string Descripcion, string Prioridad)[]
        {
            ("Impresora sin conexión", "La impresora del laboratorio no responde desde la red.", "Media"),
            ("Solicitud de acceso al aula virtual", "Se necesita acceso al curso de práctica.", "Baja"),
            ("Equipo no enciende", "El equipo de la sala 2 no arranca al presionar el botón.", "Alta")
        };

        foreach (var m in muestras)
        {
            _servicio.Registrar(m.Titulo, m.Descripcion, m.Prioridad);
        }

        _salida.WriteLine($"Se cargaron {muestras.Length} tickets de muestra.");
    }

    private Ticket? PedirTicket(string mensaje)
    {
        var texto = Leer(mensaje);
        if (!int.TryParse(texto?.Trim(), out var id))
        {
            _salida.WriteLine("Identificador no válido. Escriba un número entero.");
            return null;
        }

        var ticket = _servicio.ObtenerPorId(id);
        if (ticket is null)
        {
            _salida.WriteLine($"No existe un ticket con identificador {id}.");
        }

        return ticket;
    }

    private string? Leer(string mensaje)
    {
        _salida.Write(mensaje);
        return _entrada.ReadLine();
    }

    private static string Recortar(string texto, int maximo) =>
        texto.Length <= maximo ? texto : texto[..(maximo - 3)] + "...";
}
