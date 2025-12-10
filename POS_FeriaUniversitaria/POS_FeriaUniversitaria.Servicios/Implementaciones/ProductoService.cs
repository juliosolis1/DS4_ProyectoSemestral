using System.Collections.Generic;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;
using POS_FeriaUniversitaria.AccesoDatos.Repositorios;
using POS_FeriaUniversitaria.Servicios.Interfaces;

namespace POS_FeriaUniversitaria.Servicios.Implementaciones
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repo;

        public ProductoService()
        {
            // En un proyecto más avanzado usaríamos inyección de dependencias.
            _repo = new ProductoRepository();
        }

        public List<Producto> ObtenerTodos()
        {
            return _repo.Listar();
        }

        /// <summary>
        /// Obtiene el historial de productos desde el repositorio.
        /// </summary>
        public List<Producto> ObtenerHistorial()
        {
            return _repo.ListarHistorial();
        }

        public Producto ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }

        public void Crear(Producto producto)
        {
            // Aquí podrías validar datos antes de guardar
            _repo.Agregar(producto);
        }

        public void Editar(Producto producto)
        {
            _repo.Actualizar(producto);
        }

        public void Eliminar(int id)
        {
            _repo.Eliminar(id);
        }

        /// <summary>
        /// Pasa la orden de eliminación definitiva al repositorio.
        /// </summary>
        public void EliminarDefinitivo(int id)
        {
            _repo.EliminarDefinitivo(id);
        }

        public void Restaurar(int id)
        {
            _repo.Restaurar(id);
        }

    }
}
