using System.ComponentModel.DataAnnotations;

namespace POS_FeriaUniversitaria.Web.Models
{
    /// <summary>
    /// Ítem del carrito en sesión (con datos de producto para mostrar).
    /// </summary>
    public class CarritoItemViewModel
    {
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string Nombre { get; set; }

        [Display(Name = "Imagen")]
        public string ImagenPortada { get; set; }

        [Display(Name = "Precio")]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}
