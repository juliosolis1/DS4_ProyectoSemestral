using System;
using System.Collections.Generic;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;

namespace POS_FeriaUniversitaria.Servicios.Interfaces
{
    /// <summary>
    /// Capa de orquestación/negocio para ventas.
    /// </summary>
    public interface IVentaService
    {
        int RegistrarVenta(Venta venta, List<DetalleVenta> detalles);
        List<Venta> VentasDelDia(DateTime fecha);
    }
}
