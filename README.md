# Mesa de ayuda

Primera versión funcional de un sistema de mesa de ayuda en C# (proyecto de consola, .NET 8).
Permite registrar tickets, listarlos, consultar su detalle y cambiar su estado, con los datos en memoria.

Documentación del proyecto:

- [hito-0.md](hito-0.md): propuesta inicial.
- [hito-1.md](hito-1.md): cambios respecto del Hito 0, decisiones de diseño y correcciones.

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) o superior.
- Git (para clonar el repositorio).

Verifique la instalación con `dotnet --version`.

## Pasos de ejecución

```bash
git clone <URL-del-repositorio>
cd <carpeta-del-repositorio>
dotnet run --project src/MesaAyuda
```

Para ejecutar las pruebas unitarias:

```bash
dotnet test
```

## Funciones disponibles

Al iniciar, la aplicación muestra un menú con los contadores actualizados (total, abiertos y cerrados) y el filtro activo.

| Opción | Función |
|--------|---------|
| 1 | **Registrar ticket**: título, descripción y prioridad (Baja, Media o Alta). Recibe un identificador único y queda en estado *Abierto*. |
| 2 | **Listar tickets**: muestra identificador, título y estado según el filtro activo. |
| 3 | **Consultar ticket**: muestra todos los datos del ticket cuyo identificador se indique. |
| 4 | **Cerrar ticket**: cambia el estado de *Abierto* a *Cerrado*. Un ticket cerrado no puede cerrarse otra vez. |
| 5 | **Cambiar filtro**: todos, abiertos o cerrados. No modifica los tickets registrados. |
| 6 | **Cargar datos de muestra**: agrega tres tickets genéricos para la demostración. |
| 0 | Salir. |

### Validaciones

El registro se rechaza, indicando qué corregir, si:

- el título está vacío o supera 100 caracteres,
- la descripción está vacía,
- la prioridad no es Baja, Media o Alta.

El cierre se rechaza si el ticket no existe o ya está cerrado.

## Estructura del repositorio

```
src/MesaAyuda/
  Models/      Ticket, EstadoTicket, PrioridadTicket
  Services/    TicketServicio (datos en memoria), FiltroEstado, ResultadoRegistro
  UI/          MenuConsola
  Program.cs   punto de entrada
tests/MesaAyuda.Tests/   pruebas unitarias (xUnit)
```

## Limitaciones conocidas

- Los tickets se guardan solo en memoria: al cerrar la aplicación se pierden.
- El único cambio de estado disponible es *Abierto → Cerrado*; no se puede reabrir un ticket.
- No hay edición ni eliminación de tickets.
- No hay usuarios, autenticación ni asignación de responsables.
- La interfaz es de consola; no hay versión web (Blazor) en esta iteración.
- El identificador es un número consecutivo que se reinicia en cada ejecución.
