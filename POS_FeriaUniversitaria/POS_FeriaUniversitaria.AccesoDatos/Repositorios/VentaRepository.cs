using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;
using POS_FeriaUniversitaria.AccesoDatos.Infraestructura;

namespace POS_FeriaUniversitaria.AccesoDatos.Repositorios
{
    /// <summary>
    /// Implementación ADO.NET de las operaciones de Venta.
    /// </summary>
    public class VentaRepository : IVentaRepository
    {
        public int CrearVenta(Venta venta, List<DetalleVenta> detalles)
        {
            using (var cn = ConexionBD.ObtenerConexion())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) Insert cabecera Venta
                        int nuevaVentaId;
                        using (var cmd = new SqlCommand(@"
                            INSERT INTO Ventas (Fecha, Total, Observaciones)
                            VALUES (@Fecha, @Total, @Obs);
                            SELECT SCOPE_IDENTITY();", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Fecha", venta.Fecha);
                            cmd.Parameters.AddWithValue("@Total", venta.Total);
                            cmd.Parameters.AddWithValue("@Obs", (object)venta.Observaciones ?? DBNull.Value);
                            nuevaVentaId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 2) Insert detalles + 3) Descontar stock
                        foreach (var d in detalles)
                        {
                            using (var cmdDet = new SqlCommand(@"
                                INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario)
                                VALUES (@VentaId, @ProductoId, @Cantidad, @PrecioUnitario);", cn, tx))
                            {
                                cmdDet.Parameters.AddWithValue("@VentaId", nuevaVentaId);
                                cmdDet.Parameters.AddWithValue("@ProductoId", d.ProductoId);
                                cmdDet.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                                cmdDet.Parameters.AddWithValue("@PrecioUnitario", d.PrecioUnitario);
                                cmdDet.ExecuteNonQuery();
                            }

                            using (var cmdStock = new SqlCommand(@"
                                UPDATE Productos
                                SET Stock = Stock - @Cant
                                WHERE ProductoId = @ProdId;", cn, tx))
                            {
                                cmdStock.Parameters.AddWithValue("@Cant", d.Cantidad);
                                cmdStock.Parameters.AddWithValue("@ProdId", d.ProductoId);
                                cmdStock.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        return nuevaVentaId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Venta> ListarPorFecha(DateTime fecha)
        {
            var lista = new List<Venta>();
            using (var cn = ConexionBD.ObtenerConexion())
            using (var cmd = new SqlCommand(@"
                SELECT VentaId, Fecha, Total, Observaciones
                FROM Ventas
                WHERE CONVERT(date, Fecha) = @Fecha;", cn))
            {
                cmd.Parameters.AddWithValue("@Fecha", fecha.Date);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Venta
                        {
                            VentaId = dr.GetInt32(0),
                            Fecha = dr.GetDateTime(1),
                            Total = dr.GetDecimal(2),
                            Observaciones = dr.IsDBNull(3) ? null : dr.GetString(3)
                        });
                    }
                }
            }
            return lista;
        }
    }
}
