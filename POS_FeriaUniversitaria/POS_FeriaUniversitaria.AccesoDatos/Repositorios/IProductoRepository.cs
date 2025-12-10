using System.Collections.Generic;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;

namespace POS_FeriaUniversitaria.AccesoDatos.Repositorios
{
    public interface IProductoRepository
    {
        List<Producto> Listar();
        List<Producto> ListarHistorial();

        Producto ObtenerPorId(int id);
        void Agregar(Producto producto);
        void Actualizar(Producto producto);

        void Eliminar(int id);
        void EliminarDefinitivo(int id);

        /// <summary>
        /// Restaura un producto previamente eliminado:
        /// lo marca como activo y limpia la FechaEliminacion.
        /// </summary>
        void Restaurar(int id);
    }
}
