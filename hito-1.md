# Hito 1: primer sistema funcional de mesa de ayuda

<!-- REVISAR antes de entregar: contrastar las secciones "Cambios respecto del Hito 0" y "Correcciones" con hito-0.md y ajustarlas a lo que realmente se propuso allí. -->

## Alcance entregado

Aplicación de consola en C# (.NET 8) con el flujo completo: registrar un ticket, encontrarlo en la lista, consultar sus datos y cambiar su estado, con datos en memoria y sin modificar el código para la demostración. Cubre los ocho requisitos funcionales del Hito 1.

| Requisito | Dónde se resuelve |
|-----------|-------------------|
| 1. Registro | `TicketServicio.Registrar`; el modelo asigna estado *Abierto* y el servicio un identificador consecutivo. |
| 2. Listado | `TicketServicio.Listar` y opción 2 del menú (ID, título, estado). |
| 3. Consulta | Opción 3: se elige el ticket por identificador y se muestran todos sus datos. |
| 4. Cambio de estado | `Ticket.Cerrar` y `TicketServicio.Cerrar`; la lista y el detalle leen el mismo objeto, por lo que se actualizan al instante. |
| 5. Validación | `Ticket.Validar` (reglas) y `TicketServicio.Registrar` (prioridad como texto). |
| 6. Consulta por estado | `FiltroEstado` y opción 5. Filtrar no modifica la colección. |
| 7. Cantidades | `Total`, `Abiertos` y `Cerrados` se calculan a partir de la colección en cada consulta. |
| 8. Conservación | `List<Ticket>` dentro de `TicketServicio`, que vive mientras la aplicación está abierta. |

## Decisiones de diseño

- **El modelo protege sus reglas.** `Ticket` valida título, descripción y prioridad en el constructor y lanza `ArgumentException` si algo es inválido; así no puede existir un ticket inconsistente, sin importar quién lo cree.
- **Estado con setter privado.** `Estado` solo cambia mediante `Cerrar()`, que lanza `InvalidOperationException` si el ticket ya está cerrado. Esto garantiza la regla "un ticket cerrado no puede cerrarse otra vez" en un único lugar.
- **Identificador, título, descripción y prioridad son de solo lectura.** Solo cambian el estado y la fecha de cierre.
- **Validación con mensajes accionables.** `Ticket.Validar` devuelve la lista completa de correcciones, para que el usuario vea todos los problemas a la vez y no uno por intento.
- **Prioridad como enumeración (Baja, Media, Alta).** Se eligió prioridad en lugar de categoría porque tiene un conjunto cerrado y simple de valores. El servicio recibe el texto y rechaza cualquier valor distinto, incluidos números.
- **Separación en capas.** `Models` (reglas del ticket), `Services` (casos de uso y almacenamiento en memoria) y `UI` (consola). La interfaz recibe `TextReader` y `TextWriter`, lo que permite recorrer el flujo completo con una entrada simulada.
- **Contadores derivados.** No se guardan en variables aparte; se calculan desde la colección y por eso nunca quedan desincronizados.
- **Datos de muestra genéricos.** La opción 6 carga tickets que no identifican personas ni revelan solicitudes privadas.

## Cambios respecto del Hito 0

- El proyecto se implementó como aplicación de consola (la opción Blazor queda fuera de esta iteración).
- Se agregó la fecha de creación y de cierre al ticket, y un límite de 100 caracteres para el título.
- Se incorporó una opción de datos de muestra para facilitar la demostración.
- Se separó el código en `Models`, `Services` y `UI` y se añadió un proyecto de pruebas unitarias.

## Correcciones realizadas

- La validación de prioridad rechaza valores numéricos (por ejemplo `1`), que una conversión directa de texto a enumeración habría aceptado como válidos.
- Un identificador inexistente o no numérico en las opciones de consulta y cierre muestra un mensaje claro en lugar de interrumpir la aplicación.

## Verificación

- Pruebas unitarias del modelo y del servicio en `tests/MesaAyuda.Tests` (validación, cierre sin doble cierre, filtros, contadores, identificadores únicos).
- Recorrido manual completo de la consola: registro válido e inválido, lista, consulta de dos tickets distintos, cierre, intento de doble cierre, filtros y contadores.

## Trazabilidad

Trabajo individual. El historial de commits describe cada parte del proyecto: estructura, modelo, servicio, interfaz, pruebas y documentación.
