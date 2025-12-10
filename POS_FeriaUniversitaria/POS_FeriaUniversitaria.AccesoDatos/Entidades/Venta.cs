using System;
using System.Collections.Generic;

namespace POS_FeriaUniversitaria.AccesoDatos.Entidades
{
    /// <summary>
    /// Representa una venta realizada en la feria.
    /// </summary>
    public class Venta
    {
        public int VentaId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Observaciones { get; set; }

        // Lista de detalles de la venta
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
