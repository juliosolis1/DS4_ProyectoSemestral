using System.Collections.Generic;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;

namespace POS_FeriaUniversitaria.Servicios.Interfaces
{
    public interface IProductoService
    {
        List<Producto> ObtenerTodos();
        List<Producto> ObtenerHistorial();

        Producto ObtenerPorId(int id);
        void Crear(Producto producto);
        void Editar(Producto producto);

        void Eliminar(int id);
        void EliminarDefinitivo(int id);
        void Restaurar(int id);
    }
}
