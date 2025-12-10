using System;

namespace POS_FeriaUniversitaria.AccesoDatos.Entidades
{
    /// <summary>
    /// Resumen diario de ventas (simulación de cierre de caja).
    /// </summary>
    public class CierreCaja
    {
        public int CierreId { get; set; }
        public DateTime FechaCierre { get; set; }
        public decimal TotalVentas { get; set; }
        public int CantidadTransacciones { get; set; }
    }
}
