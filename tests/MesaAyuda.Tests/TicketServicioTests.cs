using MesaAyuda.Models;
using MesaAyuda.Services;

namespace MesaAyuda.Tests;

public class TicketServicioTests
{
    private static TicketServicio ServicioConTres()
    {
        var servicio = new TicketServicio();
        servicio.Registrar("Uno", "Descripción uno", "Baja");
        servicio.Registrar("Dos", "Descripción dos", "Media");
        servicio.Registrar("Tres", "Descripción tres", "Alta");
        return servicio;
    }

    [Fact]
    public void Registrar_AsignaIdentificadoresDistintos()
    {
        var servicio = ServicioConTres();

        var ids = servicio.Listar().Select(t => t.Id).ToList();

        Assert.Equal(new[] { 1, 2, 3 }, ids);
    }

    [Fact]
    public void Registrar_ConDatosInvalidos_NoGuardaYIndicaQueCorregir()
    {
        var servicio = new TicketServicio();

        var resultado = servicio.Registrar("", " ", "Urgente");

        Assert.False(resultado.Exitoso);
        Assert.Equal(3, resultado.Errores.Count);
        Assert.Equal(0, servicio.Total);
    }

    [Fact]
    public void Registrar_NoAceptaPrioridadNumerica()
    {
        var servicio = new TicketServicio();

        var resultado = servicio.Registrar("Título", "Descripción", "1");

        Assert.False(resultado.Exitoso);
    }

    [Fact]
    public void Registrar_AceptaPrioridadSinImportarMayusculas()
    {
        var servicio = new TicketServicio();

        var resultado = servicio.Registrar("Título", "Descripción", " ALTA ");

        Assert.True(resultado.Exitoso);
        Assert.Equal(PrioridadTicket.Alta, resultado.Ticket!.Prioridad);
    }

    [Fact]
    public void Cerrar_ActualizaSoloElTicketIndicado()
    {
        var servicio = ServicioConTres();

        var error = servicio.Cerrar(2);

        Assert.Null(error);
        Assert.Equal(EstadoTicket.Abierto, servicio.ObtenerPorId(1)!.Estado);
        Assert.Equal(EstadoTicket.Cerrado, servicio.ObtenerPorId(2)!.Estado);
        Assert.Equal(EstadoTicket.Abierto, servicio.ObtenerPorId(3)!.Estado);
    }

    [Fact]
    public void Cerrar_UnTicketYaCerrado_DevuelveError()
    {
        var servicio = ServicioConTres();
        servicio.Cerrar(1);

        var error = servicio.Cerrar(1);

        Assert.NotNull(error);
        Assert.Equal(1, servicio.Cerrados);
    }

    [Fact]
    public void Cerrar_UnTicketInexistente_DevuelveError()
    {
        var servicio = ServicioConTres();

        Assert.NotNull(servicio.Cerrar(99));
    }

    [Fact]
    public void Listar_FiltraPorEstadoSinModificarLosTickets()
    {
        var servicio = ServicioConTres();
        servicio.Cerrar(2);

        Assert.Equal(3, servicio.Listar(FiltroEstado.Todos).Count);
        Assert.Equal(new[] { 1, 3 }, servicio.Listar(FiltroEstado.Abiertos).Select(t => t.Id));
        Assert.Equal(new[] { 2 }, servicio.Listar(FiltroEstado.Cerrados).Select(t => t.Id));
        Assert.Equal(3, servicio.Total);
    }

    [Fact]
    public void Contadores_SeActualizanAlRegistrarYCerrar()
    {
        var servicio = new TicketServicio();
        Assert.Equal((0, 0, 0), (servicio.Total, servicio.Abiertos, servicio.Cerrados));

        servicio.Registrar("Uno", "Descripción", "Baja");
        servicio.Registrar("Dos", "Descripción", "Baja");
        Assert.Equal((2, 2, 0), (servicio.Total, servicio.Abiertos, servicio.Cerrados));

        servicio.Cerrar(1);
        Assert.Equal((2, 1, 1), (servicio.Total, servicio.Abiertos, servicio.Cerrados));
    }

    [Fact]
    public void ObtenerPorId_ConIdInexistente_DevuelveNull()
    {
        Assert.Null(ServicioConTres().ObtenerPorId(42));
    }
}
