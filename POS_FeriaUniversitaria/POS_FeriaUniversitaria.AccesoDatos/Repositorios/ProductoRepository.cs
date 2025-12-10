using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;
using POS_FeriaUniversitaria.AccesoDatos.Infraestructura;

namespace POS_FeriaUniversitaria.AccesoDatos.Repositorios
{
    /// <summary>
    /// Implementa el CRUD de productos usando ADO.NET "puro".
    /// La idea es que tú puedas reconocer el patrón de los laboratorios.
    /// </summary>
    public class ProductoRepository : IProductoRepository
    {
        public List<Producto> Listar()
        {
            var lista = new List<Producto>();

            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Activo = 1", cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Producto
                        {
                            ProductoId = (int)dr["ProductoId"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            PrecioVenta = (decimal)dr["PrecioVenta"],
                            Stock = (int)dr["Stock"],
                            ImagenPortada = dr["ImagenPortada"] == DBNull.Value
                            ? null
                            : dr["ImagenPortada"].ToString(),
                            Activo = (bool)dr["Activo"],
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaEliminacion = dr["FechaEliminacion"] == DBNull.Value
                                ? (DateTime?)null
                                : (DateTime)dr["FechaEliminacion"]
                        });
                    }
                }
            }

            return lista;
        }

        public Producto ObtenerPorId(int id)
        {
            Producto producto = null;

            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE ProductoId = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        producto = new Producto
                        {
                            ProductoId = (int)dr["ProductoId"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            PrecioVenta = (decimal)dr["PrecioVenta"],
                            Stock = (int)dr["Stock"],
                            ImagenPortada = dr["ImagenPortada"] == DBNull.Value
                             ? null
                            : dr["ImagenPortada"].ToString(),
                            Activo = (bool)dr["Activo"],
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaEliminacion = dr["FechaEliminacion"] == DBNull.Value
                                ? (DateTime?)null
                                : (DateTime)dr["FechaEliminacion"]
                        };
                    }
                }
            }

            return producto;
        }


        public void Agregar(Producto producto)
        {
            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
            @"INSERT INTO Productos (Nombre, Descripcion, PrecioVenta, Stock, Activo, ImagenPortada)
              VALUES (@Nombre, @Descripcion, @PrecioVenta, @Stock, @Activo, @ImagenPortada)", cn))
            {
                cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                cmd.Parameters.AddWithValue("@Stock", producto.Stock);
                cmd.Parameters.AddWithValue("@Activo", producto.Activo);
                cmd.Parameters.AddWithValue("@ImagenPortada", (object)producto.ImagenPortada ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }

        }

        public void Actualizar(Producto producto)
        {
            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
            @"UPDATE Productos
              SET Nombre = @Nombre,
                  Descripcion = @Descripcion,
                  PrecioVenta = @PrecioVenta,
                  Stock = @Stock,
                  Activo = @Activo,
                  ImagenPortada = @ImagenPortada
              WHERE ProductoId = @ProductoId", cn))
            {
                cmd.Parameters.AddWithValue("@ProductoId", producto.ProductoId);
                cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                cmd.Parameters.AddWithValue("@Stock", producto.Stock);
                cmd.Parameters.AddWithValue("@Activo", producto.Activo);
                cmd.Parameters.AddWithValue("@ImagenPortada", (object)producto.ImagenPortada ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }

        }

        public void Eliminar(int id)
        {
            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
                @"UPDATE Productos 
          SET Activo = 0, FechaEliminacion = GETDATE() 
          WHERE ProductoId = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// Devuelve el historial de productos:
        /// - Todos los productos activos.
        /// - Los productos inactivos cuya FechaEliminacion es menor o igual a 30 días.
        /// Además, limpia automáticamente los inactivos más antiguos de 30 días.
        public List<Producto> ListarHistorial()
        {
            var lista = new List<Producto>();

            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            {
                cn.Open();

                // 1) Limpiar productos inactivos con más de 30 días en el historial
                using (SqlCommand cmdPurge = new SqlCommand(
                    @"DELETE FROM Productos
              WHERE Activo = 0
                AND FechaEliminacion IS NOT NULL
                AND FechaEliminacion < DATEADD(DAY, -30, GETDATE());", cn))
                {
                    cmdPurge.ExecuteNonQuery();
                }

                // 2) Obtener productos activos + inactivos dentro de los 30 días
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT * 
              FROM Productos
              WHERE Activo = 1
                 OR (Activo = 0 
                     AND FechaEliminacion IS NOT NULL
                     AND FechaEliminacion >= DATEADD(DAY, -30, GETDATE()))
              ORDER BY Activo DESC, Nombre;", cn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Producto
                        {
                            ProductoId = (int)dr["ProductoId"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            PrecioVenta = (decimal)dr["PrecioVenta"],
                            Stock = (int)dr["Stock"],
                            ImagenPortada = dr["ImagenPortada"] == DBNull.Value
                            ? null
                            : dr["ImagenPortada"].ToString(),
                            Activo = (bool)dr["Activo"],
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaEliminacion = dr["FechaEliminacion"] == DBNull.Value
                                ? (DateTime?)null
                                : (DateTime)dr["FechaEliminacion"]
                        });
                    }
                }
            }

            return lista;
        }

        /// Eliminación física: borra la fila de la tabla Productos.
        /// Se usa desde la pantalla de Historial, cuando el usuario decide
        /// que ya no quiere recuperar el producto.
        public void EliminarDefinitivo(int id)
        {
            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
                @"DELETE FROM Productos WHERE ProductoId = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Restaura un producto al inventario:
        /// - Activo = 1
        /// - FechaEliminacion = NULL
        /// De esta forma vuelve a aparecer en el listado principal de productos.
        /// </summary>
        public void Restaurar(int id)
        {
            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
                @"UPDATE Productos
          SET Activo = 1,
              FechaEliminacion = NULL
          WHERE ProductoId = @id;", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
