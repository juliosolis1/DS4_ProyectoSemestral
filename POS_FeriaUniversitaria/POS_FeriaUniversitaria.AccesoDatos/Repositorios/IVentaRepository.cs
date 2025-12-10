using System;
using System.Collections.Generic;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;

namespace POS_FeriaUniversitaria.AccesoDatos.Repositorios
{
    /// <summary>
    /// Contrato de persistencia para registrar ventas y consultar reportes.
    /// </summary>
    public interface IVentaRepository
    {
        /// <summary>
        /// Inserta la cabecera de venta y sus detalles en una transacción.
        /// También descuenta stock de Productos.
        /// Devuelve el Id autogenerado de la venta.
        /// </summary>
        int CrearVenta(Venta venta, List<DetalleVenta> detalles);

        /// <summary>
        /// Ventas (cabecera) de una fecha concreta (para reporte diario).
        /// </summary>
        List<Venta> ListarPorFecha(DateTime fecha);
    }
}
