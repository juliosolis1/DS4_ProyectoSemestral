/*
Universidad Tecnológica de Panamá
Facultad de Ingeniería en Sistemas Computacionales
Licenciatura en Desarrollo y Gestión de Software

Asignatura - Desarrollo de Software IV

Proyecto Semestral - Mini POS para Feria Universitaria

Facilitador: Regis Rivera

Estudiante:
Julio Solís | 8-1011-1457

Grupo: 1GS222

Fecha de entrega: 16 de diciembre de 2025
II Semestre | II Año
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Http;
using POS_FeriaUniversitaria.AccesoDatos.Infraestructura;

namespace POS_FeriaUniversitaria.Web.Controllers.Api
{
    /* API REST para exponer los reportes.
       Este controlador corresponde a la capa "API Rest" del diagrama.
       Implementa el reporte diario con detalle de ventas.

       NOTA IMPORTANTE:
       - Se dejaron los DTO como clases PUBLICAS para evitar problemas de serialización.
       - Se normaliza la fecha con formato "yyyy-MM-dd" (compatible con input[type=date]). */
    [RoutePrefix("api/reportes")]
    public class ReportesApiController : ApiController
    {
        #region DTOs (forma del JSON de salida)

        /* Representa un producto dentro del detalle de una venta.
           Estas propiedades se serializan a JSON y las consume la vista con JavaScript. */
        public class DetalleVentaDto
        {
            public int DetalleId { get; set; }
            public int ProductoId { get; set; }
            public string NombreProducto { get; set; }
            public int Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Subtotal { get; set; }
            public string ImagenPortada { get; set; }
        }

        /* Representa una venta del reporte diario con su lista de detalles.
           Importante: Detalles se inicializa para evitar null en el front. */
        public class VentaDto
        {
            public int VentaId { get; set; }
            public DateTime Fecha { get; set; }
            public decimal Total { get; set; }
            public string Observaciones { get; set; }
            public List<DetalleVentaDto> Detalles { get; set; } = new List<DetalleVentaDto>();
        }

        #endregion

        /* GET api/reportes/ping
           Endpoint de prueba rápida para confirmar que la API está respondiendo. */
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            return Ok(new
            {
                ok = true,
                api = "ReportesApiController",
                servidor = Environment.MachineName,
                fechaServidor = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        /* GET api/reportes/diario?fecha=yyyy-MM-dd

           Devuelve el cierre diario:
           - fechaCierre
           - totalVentas
           - cantidadTransacciones
           - ventas (lista) con sus detalles de productos.

           Reglas de fecha:
           - Si NO se envía 'fecha', se consulta el día de hoy.
           - Si se envía 'fecha' y no es válida, se devuelve 400 (BadRequest).
           - Formato recomendado: yyyy-MM-dd */
        [HttpGet]
        [Route("diario")]
        public IHttpActionResult GetReporteDiario([FromUri] string fecha = null)
        {
            try
            {
                // 1) Validar y normalizar la fecha recibida
                DateTime fechaConsulta;

                if (string.IsNullOrWhiteSpace(fecha))
                {
                    fechaConsulta = DateTime.Today;
                }
                else
                {
                    /* Primero intentamos el formato más común en input type="date": yyyy-MM-dd
                       Si falla, intentamos un TryParse general como respaldo. */
                    if (!DateTime.TryParseExact(fecha.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out fechaConsulta))
                    {
                        if (!DateTime.TryParse(fecha, out fechaConsulta))
                        {
                            return Content(HttpStatusCode.BadRequest, new
                            {
                                mensaje = "Parámetro 'fecha' inválido.",
                                formatoEsperado = "yyyy-MM-dd",
                                ejemplo = "2025-12-07"
                            });
                        }
                    }
                }

                // 2) Diccionario auxiliar para agrupar detalles por VentaId
                var ventasPorId = new Dictionary<int, VentaDto>();

                using (var cn = ConexionBD.ObtenerConexion())
                {
                    cn.Open();

                    /* 3) Consulta única (cabecera + detalle + producto)
                       Se usa LEFT JOIN por seguridad, por si existiera una venta sin detalles. */
                    using (var cmd = new SqlCommand(@"
                        SELECT  v.VentaId,
                                v.Fecha,
                                v.Total,
                                v.Observaciones,
                                d.DetalleId,
                                d.ProductoId,
                                p.Nombre        AS NombreProducto,
                                d.Cantidad,
                                d.PrecioUnitario,
                                d.Subtotal,
                                p.ImagenPortada
                        FROM Ventas v
                        LEFT JOIN DetalleVentas d   ON v.VentaId = d.VentaId
                        LEFT JOIN Productos     p   ON d.ProductoId = p.ProductoId
                        WHERE CONVERT(date, v.Fecha) = @Fecha
                        ORDER BY v.VentaId, d.DetalleId;", cn))
                    {
                        // Evita AddWithValue para no generar conversiones implícitas raras en SQL Server
                        cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fechaConsulta.Date;

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var ventaId = dr.GetInt32(0);

                                // 4) Si la venta aún no existe, la creamos
                                if (!ventasPorId.TryGetValue(ventaId, out var ventaDto))
                                {
                                    ventaDto = new VentaDto
                                    {
                                        VentaId = ventaId,
                                        Fecha = dr.GetDateTime(1),
                                        Total = dr.GetDecimal(2),
                                        Observaciones = dr.IsDBNull(3) ? null : dr.GetString(3)
                                    };

                                    ventasPorId.Add(ventaId, ventaDto);
                                }

                                // 5) Si hay detalle (DetalleId no es NULL), lo agregamos a la venta
                                if (!dr.IsDBNull(4))
                                {
                                    var imagenRaw = dr.IsDBNull(10) ? null : dr.GetString(10);

                                    var detalle = new DetalleVentaDto
                                    {
                                        DetalleId = dr.GetInt32(4),
                                        ProductoId = dr.GetInt32(5),
                                        NombreProducto = dr.IsDBNull(6) ? "(Producto no encontrado)" : dr.GetString(6),
                                        Cantidad = dr.IsDBNull(7) ? 0 : dr.GetInt32(7),
                                        PrecioUnitario = dr.IsDBNull(8) ? 0m : dr.GetDecimal(8),
                                        Subtotal = dr.IsDBNull(9) ? 0m : dr.GetDecimal(9),
                                        ImagenPortada = ConvertirAUrlImagen(imagenRaw)
                                    };

                                    ventaDto.Detalles.Add(detalle);
                                }
                            }
                        }
                    }
                }

                // 6) Pasar a lista ordenada
                var listaVentas = ventasPorId.Values
                    .OrderBy(v => v.VentaId)
                    .ToList();

                // 7) Resumen diario
                decimal totalVentas = listaVentas.Sum(v => v.Total);
                int cantidadTransacciones = listaVentas.Count;

                // 8) Objeto de salida (se serializa automático a JSON)
                var resultado = new
                {
                    fechaCierre = fechaConsulta.ToString("yyyy-MM-dd"),
                    totalVentas = totalVentas,
                    cantidadTransacciones = cantidadTransacciones,
                    ventas = listaVentas
                };

                return Ok(resultado);
            }
            catch (SqlException ex)
            {
                /* Si algo falla a nivel de BD, devolvemos un error claro.
                   (En producción normalmente NO se expone el detalle del error.) */
                return Content(HttpStatusCode.InternalServerError, new
                {
                    mensaje = "Error consultando la base de datos para el reporte diario.",
                    detalle = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    mensaje = "Error inesperado generando el reporte diario.",
                    detalle = ex.Message
                });
            }
        }

        /* Convierte la ruta guardada en BD en una URL consumible por el navegador.
           Casos soportados:
           - null/empty => null
           - https://... => se devuelve igual
           - ~/Content/img.png => se resuelve con Url.Content
           - /Content/img.png  => se resuelve con Url.Content("~" + ruta)
           - Content/img.png   => se resuelve con Url.Content("~/" + ruta) */
        private string ConvertirAUrlImagen(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta))
                return null;

            // Normaliza separadores (por si en BD se guardó con backslashes)
            ruta = ruta.Trim().Replace("\\", "/");

            if (ruta.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                ruta.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return ruta;
            }

            if (ruta.StartsWith("~/"))
            {
                return Url.Content(ruta);
            }

            if (ruta.StartsWith("/"))
            {
                return Url.Content("~" + ruta);
            }

            return Url.Content("~/" + ruta.TrimStart('/'));
        }
    }
}

// Para probar la consulta del servidor en Postman: https://localhost:44373/api/reportes/ping
// Para probar la consulta del reporte en Postman: https://localhost:44373/api/reportes/diario?fecha=2025-12-08, https://localhost:44373/api/reportes/diario?fecha=2025-12-09