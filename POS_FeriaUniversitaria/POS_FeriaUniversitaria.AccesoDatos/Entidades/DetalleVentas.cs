namespace POS_FeriaUniversitaria.AccesoDatos.Entidades
{
    /// <summary>
    /// Detalle de cada producto vendido en una venta.
    /// </summary>
    public class DetalleVenta
    {
        public int DetalleId { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
