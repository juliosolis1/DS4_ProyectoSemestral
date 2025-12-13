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

namespace POS_FeriaUniversitaria.AccesoDatos.Repositorios
{
    /* Contrato (interfaz) para el acceso a datos de Productos.
       - Define las operaciones que la capa de Servicios puede solicitar a la capa de AccesoDatos.
       - Permite desacoplar la lógica de negocio de la implementación (ADO.NET / SQL Server).
       - Centraliza las acciones de inventario: listar, buscar, crear, actualizar, eliminar y restaurar. */

    public interface IProductoRepository
    {
        // Lista los productos activos (inventario principal).
        List<Producto> Listar();

        // Devuelve el historial: activos + inactivos recientes (para pantalla de Historial).
        List<Producto> ListarHistorial();

        // Obtiene un producto por su ID (si no existe, la implementación puede devolver null).
        Producto ObtenerPorId(int id);

        // Inserta un producto nuevo en la base de datos.
        void Agregar(Producto producto);

        // Actualiza la información de un producto existente.
        void Actualizar(Producto producto);

        // Eliminación lógica: marca como inactivo y registra la fecha de eliminación.
        void Eliminar(int id);

        // Eliminación física: borra definitivamente el registro en la tabla (solo desde historial).
        void EliminarDefinitivo(int id);

        /* Restaura un producto previamente eliminado (eliminación lógica):
           - Activo = 1 (vuelve a mostrarse en inventario)
           - FechaEliminacion = NULL (se limpia la marca de eliminación) */
        void Restaurar(int id);
    }
}
