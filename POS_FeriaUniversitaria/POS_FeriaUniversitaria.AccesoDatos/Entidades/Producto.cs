using System;
using System.ComponentModel.DataAnnotations;

namespace POS_FeriaUniversitaria.AccesoDatos.Entidades
{
    /// <summary>
    /// Representa un producto del inventario de la feria universitaria.
    /// Esta clase se usa tanto en la capa de acceso a datos como en MVC.
    /// </summary>
    public class Producto
    {
        [Key] // No es obligatorio, pero documenta que es la PK
        public int ProductoId { get; set; }   // PK

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; }    // Nombre visible en la venta

        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor que cero.")]
        [Display(Name = "Precio de venta")]
        public decimal PrecioVenta { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        [Display(Name = "Stock disponible")]
        public int Stock { get; set; }        // Cantidad disponible

        [StringLength(260)]
        [Display(Name = "Imagen del producto")]
        public string ImagenPortada { get; set; }  // Ej: "~/Content/ImagenesProductos/abc123.jpg"

        [Display(Name = "Activo")]
        public bool Activo { get; set; }      // Para "desactivar" productos sin borrarlos

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Fecha de eliminación")]
        public DateTime? FechaEliminacion { get; set; }
    }
}
