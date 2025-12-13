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

using System.Collections.Generic;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;
using POS_FeriaUniversitaria.AccesoDatos.Repositorios;
using POS_FeriaUniversitaria.Servicios.Interfaces;

namespace POS_FeriaUniversitaria.Servicios.Implementaciones
{
    /* Capa de Servicios (ProductoService):
       - Encapsula reglas de negocio relacionadas a productos (inventario).
       - Actúa como intermediario entre Controllers (capa Web) y Repositorios (AccesoDatos).
       - Centraliza validaciones antes de enviar información a la base de datos. */

    public class ProductoService : IProductoService
    {
        // Repositorio de datos (AccesoDatos): ejecuta las operaciones CRUD en la tabla Productos.
        private readonly IProductoRepository _repo;

        public ProductoService()
        {
            /* En un escenario más avanzado se utilizaría Inyección de Dependencias (DI).
               Aquí se instancia directamente el repositorio para mantener el proyecto simple. */
            _repo = new ProductoRepository();
        }

        // Devuelve el inventario principal: lista únicamente productos activos.
        public List<Producto> ObtenerTodos()
        {
            return _repo.Listar();
        }

        // Obtiene el historial de productos desde el repositorio.
        public List<Producto> ObtenerHistorial()
        {
            // Devuelve el historial: productos activos + inactivos recientes (para pantalla Historial).
            return _repo.ListarHistorial();
        }

        // Busca un producto por su ID (si no existe, devuelve null).
        public Producto ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }

        /* Registra un nuevo producto.
           Aquí es el punto ideal para validar reglas de negocio (por ejemplo: nombre requerido, precio > 0, stock >= 0) 
           antes de enviarlo al repositorio. */

        public void Crear(Producto producto)
        {
            // Aquí se validan datos antes de guardar
            _repo.Agregar(producto);
        }

        // Actualiza la información del producto (edición desde inventario).
        public void Editar(Producto producto)
        {
            _repo.Actualizar(producto);
        }

        // Eliminación lógica: marca el producto como inactivo y registra FechaEliminacion.
        public void Eliminar(int id)
        {
            _repo.Eliminar(id);
        }

        // Pasa la orden de eliminación definitiva al repositorio.
        public void EliminarDefinitivo(int id)
        {
            _repo.EliminarDefinitivo(id);
        }

        // Restaura un producto eliminado lógicamente (Activo = 1 y FechaEliminacion = NULL).
        public void Restaurar(int id)
        {
            _repo.Restaurar(id);
        }

    }
}
