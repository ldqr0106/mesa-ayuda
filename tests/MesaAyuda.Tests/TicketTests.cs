using MesaAyuda.Models;

namespace MesaAyuda.Tests;

public class TicketTests
{
    [Fact]
    public void NuevoTicket_QuedaAbierto()
    {
        var ticket = new Ticket(1, "Título", "Descripción", PrioridadTicket.Media);

        Assert.Equal(EstadoTicket.Abierto, ticket.Estado);
        Assert.Equal(1, ticket.Id);
        Assert.Null(ticket.FechaCierre);
    }

    [Theory]
    [InlineData(null, "Descripción")]
    [InlineData("", "Descripción")]
    [InlineData("   ", "Descripción")]
    [InlineData("Título", null)]
    [InlineData("Título", "")]
    [InlineData("Título", "   ")]
    public void Constructor_RechazaTituloODescripcionVacios(string? titulo, string? descripcion)
    {
        Assert.Throws<ArgumentException>(() => new Ticket(1, titulo, descripcion, PrioridadTicket.Baja));
    }

    [Fact]
    public void Constructor_RechazaPrioridadNoPermitida()
    {
        Assert.Throws<ArgumentException>(() => new Ticket(1, "Título", "Descripción", (PrioridadTicket)99));
    }

    [Fact]
    public void Validar_DevuelveTodosLosErroresACorregir()
    {
        var errores = Ticket.Validar("", "", (PrioridadTicket)99);

        Assert.Equal(3, errores.Count);
    }

    [Fact]
    public void Validar_RechazaTituloDemasiadoLargo()
    {
        var errores = Ticket.Validar(new string('a', Ticket.LongitudMaximaTitulo + 1), "Descripción", PrioridadTicket.Alta);

        Assert.Single(errores);
    }

    [Fact]
    public void Cerrar_CambiaElEstadoYRegistraLaFecha()
    {
        var ticket = new Ticket(1, "Título", "Descripción", PrioridadTicket.Alta);

        ticket.Cerrar();

        Assert.Equal(EstadoTicket.Cerrado, ticket.Estado);
        Assert.NotNull(ticket.FechaCierre);
        Assert.False(ticket.PuedeCerrarse);
    }

    [Fact]
    public void Cerrar_UnTicketCerrado_LanzaExcepcion()
    {
        var ticket = new Ticket(1, "Título", "Descripción", PrioridadTicket.Alta);
        ticket.Cerrar();

        Assert.Throws<InvalidOperationException>(() => ticket.Cerrar());
    }
}
