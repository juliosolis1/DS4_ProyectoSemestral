using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using POS_FeriaUniversitaria.AccesoDatos.Infraestructura;

namespace POS_FeriaUniversitaria.Web.Controllers.Api
{
    /// <summary>
    /// API REST para exponer los reportes.
    /// Este controlador corresponde a la capa "API Rest" del diagrama.
    /// Aquí implementamos el reporte diario con detalle de ventas.
    /// </summary>
    [RoutePrefix("api/reportes")]
    public class ReportesApiController : ApiController
    {
        #region DTOs (clases internas para dar forma al JSON de salida)

        /// <summary>
        /// Representa un producto dentro del detalle de una venta.
        /// Estas propiedades se serializan a JSON y las consume la vista con JavaScript.
        /// </summary>
        private class DetalleVentaDto
        {
            public int DetalleId { get; set; }
            public int ProductoId { get; set; }
            public string NombreProducto { get; set; }
            public int Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Subtotal { get; set; }
            public string ImagenPortada { get; set; }
        }

        /// <summary>
        /// Representa una venta del reporte diario con su lista de detalles.
        /// </summary>
        private class VentaDto
        {
            public int VentaId { get; set; }
            public DateTime Fecha { get; set; }
            public decimal Total { get; set; }
            public string Observaciones { get; set; }
            public List<DetalleVentaDto> Detalles { get; set; } = new List<DetalleVentaDto>();
        }

        #endregion

        /// <summary>
        /// GET api/reportes/diario?fecha=2025-12-07
        /// 
        /// Devuelve el cierre diario:
        /// - Fecha de cierre
        /// - Total de ventas del día
        /// - Cantidad de transacciones
        /// - Lista de ventas del día con sus productos.
        /// </summary>
        [HttpGet]
        [Route("diario")]
        public IHttpActionResult GetReporteDiario(string fecha)
        {
            // 1. Validar y normalizar la fecha recibida
            //    Si viene vacía o inválida, tomamos la fecha de hoy.
            DateTime fechaConsulta;
            if (!DateTime.TryParse(fecha, out fechaConsulta))
            {
                fechaConsulta = DateTime.Today;
            }

            // Diccionario auxiliar para ir agrupando los detalles por VentaId
            var ventasPorId = new Dictionary<int, VentaDto>();

            using (var cn = ConexionBD.ObtenerConexion())
            {
                cn.Open();

                // 2. Consulta única que trae:
                //    - Cabecera de la venta (VentaId, Fecha, Total, Observaciones)
                //    - Detalle de la venta (producto, cantidad, precios)
                //    - Nombre del producto desde la tabla Productos
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
                INNER JOIN DetalleVentas d   ON v.VentaId = d.VentaId
                INNER JOIN Productos     p   ON d.ProductoId = p.ProductoId
                WHERE CONVERT(date, v.Fecha) = @Fecha
                ORDER BY v.VentaId, d.DetalleId;", cn))
                {
                    cmd.Parameters.AddWithValue("@Fecha", fechaConsulta.Date);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var ventaId = dr.GetInt32(0);

                            // 3. Si la venta aún no existe en el diccionario, la creamos
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

                            // 4. Creamos el detalle asociado a esta venta
                            var detalle = new DetalleVentaDto
                            {
                                DetalleId = dr.GetInt32(4),
                                ProductoId = dr.GetInt32(5),
                                NombreProducto = dr.GetString(6),
                                Cantidad = dr.GetInt32(7),
                                PrecioUnitario = dr.GetDecimal(8),
                                Subtotal = dr.GetDecimal(9),
                                ImagenPortada = dr.IsDBNull(10)
                                ? null
                                : Url.Content(dr.GetString(10))

                            };

                            // 5. Lo agregamos a la lista de detalles de la venta
                            ventaDto.Detalles.Add(detalle);
                        }
                    }
                }
            }

            // 6. Convertimos el diccionario a una lista ordenada por Id de venta
            var listaVentas = ventasPorId.Values
                                         .OrderBy(v => v.VentaId)
                                         .ToList();

            // 7. Calculamos el resumen diario:
            decimal totalVentas = listaVentas.Sum(v => v.Total);
            int cantidadTransacciones = listaVentas.Count;

            // 8. Objeto anónimo que se serializa automáticamente a JSON
            var resultado = new
            {
                fechaCierre = fechaConsulta.ToString("yyyy-MM-dd"),
                totalVentas = totalVentas,
                cantidadTransacciones = cantidadTransacciones,
                ventas = listaVentas
            };

            return Ok(resultado);
        }
    }
}
