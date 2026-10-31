using System.Text;
using MesaAyuda.Services;
using MesaAyuda.UI;

Console.OutputEncoding = Encoding.UTF8;

var servicio = new TicketServicio();
var menu = new MenuConsola(servicio, Console.In, Console.Out);
menu.Ejecutar();
