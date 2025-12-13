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
using POS_FeriaUniversitaria.AccesoDatos.Entidades;
using POS_FeriaUniversitaria.AccesoDatos.Infraestructura;

namespace POS_FeriaUniversitaria.AccesoDatos.Repositorios
{
    /* Repositorio de Ventas (ADO.NET).
       - Registra ventas (cabecera + detalles) en la BD.
       - Actualiza inventario descontando stock por cada producto vendido.
       - Usa transacciones para garantizar consistencia (todo se guarda o nada se guarda). */

    public class VentaRepository : IVentaRepository
    {
        // Crea una venta completa: inserta la cabecera, inserta los detalles y descuenta stock (todo en una transacción).
        public int CrearVenta(Venta venta, List<DetalleVenta> detalles)
        {
            using (var cn = ConexionBD.ObtenerConexion())
            {
                cn.Open();
                // Transacción: asegura que si falla un insert/update, se revierte todo para no dejar datos incompletos.
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) Insert cabecera Venta
                        int nuevaVentaId;
                        // Inserta la cabecera de la venta y obtiene el ID generado con SCOPE_IDENTITY().
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
                        // Por cada producto vendido: se guarda el detalle y se descuenta el stock del inventario.
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

                            // Descuento de stock: mantiene actualizado el inventario según las cantidades vendidas.
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

                        // Commit: confirma permanentemente los cambios (cabecera, detalles y stock).
                        tx.Commit();
                        return nuevaVentaId;
                    }
                    catch
                    {
                        // Rollback: revierte todos los cambios si ocurre cualquier error durante el proceso.
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // Lista ventas por día (ignora la hora), útil para el Reporte Diario y cierre de caja.
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
