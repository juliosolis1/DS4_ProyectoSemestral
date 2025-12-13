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

namespace POS_FeriaUniversitaria.Servicios.Interfaces
{
    /* Contrato de la Capa de Servicios para Productos.
       - Define las operaciones disponibles para la capa Web (Controllers/Vistas).
       - Permite desacoplar la lógica de negocio de la implementación concreta del servicio. */
    public interface IProductoService
    {
        // Devuelve el inventario principal: productos activos.
        List<Producto> ObtenerTodos();

        // Devuelve el historial: activos + inactivos recientes (según reglas del repositorio).
        List<Producto> ObtenerHistorial();

        // Busca un producto por su ID para ver detalle/editar/eliminar.
        Producto ObtenerPorId(int id);

        // Crea un producto nuevo en el inventario.
        void Crear(Producto producto);

        // Edita un producto existente.
        void Editar(Producto producto);

        // Eliminación lógica: lo oculta del inventario sin borrar el registro.
        void Eliminar(int id);

        // Eliminación física: borra definitivamente el producto (irreversible).
        void EliminarDefinitivo(int id);

        // Restaura un producto eliminado lógicamente para que vuelva al inventario.
        void Restaurar(int id);
    }
}
