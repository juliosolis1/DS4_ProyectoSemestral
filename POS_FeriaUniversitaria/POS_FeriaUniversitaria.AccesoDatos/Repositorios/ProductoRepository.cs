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

using POS_FeriaUniversitaria.AccesoDatos.Entidades;
using POS_FeriaUniversitaria.AccesoDatos.Infraestructura;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace POS_FeriaUniversitaria.AccesoDatos.Repositorios
{
    /* Repositorio de Productos(ADO.NET).
       - Responsable de ejecutar consultas SQL sobre la tabla Productos.
       - Implementa CRUD + funciones de historial (eliminación lógica, purga y restauración).
       - Usa consultas parametrizadas para evitar inyección SQL y garantizar tipos correctos. */
    public class ProductoRepository : IProductoRepository
    {
        // Consulta el inventario principal: solo productos activos (Activo = 1).
        public List<Producto> Listar()
        {
            var lista = new List<Producto>();

            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            // Se utiliza 'using' para asegurar el cierre/liberación de la conexión y comandos aunque ocurra un error.
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Activo = 1", cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        // Mapeo de cada fila (SqlDataReader) hacia la entidad Producto (capa de Entidades).
                        lista.Add(new Producto
                        {
                            ProductoId = (int)dr["ProductoId"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            PrecioVenta = (decimal)dr["PrecioVenta"],
                            Stock = (int)dr["Stock"],
                            // Si ImagenPortada viene NULL en BD, se guarda como null en C# para evitar errores.
                            ImagenPortada = dr["ImagenPortada"] == DBNull.Value
                            ? null
                            : dr["ImagenPortada"].ToString(),
                            Activo = (bool)dr["Activo"],
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            // FechaEliminacion es nullable: si no existe en BD, se asigna null.
                            FechaEliminacion = dr["FechaEliminacion"] == DBNull.Value
                                ? (DateTime?)null
                                : (DateTime)dr["FechaEliminacion"]
                        });
                    }
                }
            }

            return lista;
        }

        // Busca un producto por su clave primaria. Retorna null si no se encuentra.
        public Producto ObtenerPorId(int id)
        {
            Producto producto = null;

            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            // Consulta parametrizada (@id) para evitar inyección SQL y problemas de formato.
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

        // Inserta un nuevo producto. Descripcion e ImagenPortada pueden ser opcionales (NULL en BD).
        public void Agregar(Producto producto)
        {
            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
            @"INSERT INTO Productos (Nombre, Descripcion, PrecioVenta, Stock, Activo, ImagenPortada)
              VALUES (@Nombre, @Descripcion, @PrecioVenta, @Stock, @Activo, @ImagenPortada)", cn))
            {
                cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                // Si un campo opcional viene null en C#, se envía DBNull.Value a SQL Server.
                cmd.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                cmd.Parameters.AddWithValue("@Stock", producto.Stock);
                cmd.Parameters.AddWithValue("@Activo", producto.Activo);
                cmd.Parameters.AddWithValue("@ImagenPortada", (object)producto.ImagenPortada ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }

        }

        // Actualiza los datos de un producto existente identificado por ProductoId.
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

        // Eliminación lógica: no borra el registro, solo lo oculta del inventario y guarda la fecha.
        public void Eliminar(int id)
        {
            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
                // GETDATE() registra en BD la fecha/hora exacta en la que se hizo la eliminación.
                @"UPDATE Productos 
          SET Activo = 0, FechaEliminacion = GETDATE() 
          WHERE ProductoId = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /* Devuelve el historial de productos:
         - Todos los productos activos.
         - Los productos inactivos cuya FechaEliminacion es menor o igual a 30 días.
         Además, limpia automáticamente los inactivos más antiguos de 30 días. */

        // Pantalla de Historial: devuelve activos + inactivos recientes y purga inactivos antiguos (>30 días).
        public List<Producto> ListarHistorial()
        {
            var lista = new List<Producto>();

            using (SqlConnection cn = ConexionBD.ObtenerConexion())
            {
                cn.Open();

                // 1) Limpiar productos inactivos con más de 30 días en el historial
                // Purga automática: evita que el historial crezca indefinidamente eliminando inactivos muy antiguos.
                using (SqlCommand cmdPurge = new SqlCommand(
                    @"DELETE FROM Productos
              WHERE Activo = 0
                AND FechaEliminacion IS NOT NULL
                AND FechaEliminacion < DATEADD(DAY, -30, GETDATE());", cn))
                {
                    cmdPurge.ExecuteNonQuery();
                }

                // 2) Obtener productos activos + inactivos dentro de los 30 días
                // Consulta combinada: muestra activos y también inactivos dentro del rango permitido (últimos 30 días).
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

        /* Eliminación física: borra la fila de la tabla Productos.
           Se usa desde la pantalla de Historial, cuando el usuario decide
           que ya no quiere recuperar el producto. */
        // Eliminación definitiva: borra físicamente el producto (acción irreversible).
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

        /* Restaura un producto al inventario:
           - Activo = 1
           - FechaEliminacion = NULL
           De esta forma vuelve a aparecer en el listado principal de productos. */

        // Restaura un producto eliminado lógicamente para que vuelva a aparecer en el inventario.
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
