using System.Collections.Generic;
using System.Linq;

namespace POS_FeriaUniversitaria.Web.Models
{
    /// <summary>
    /// Carrito completo (lista + total).
    /// </summary>
    public class CarritoViewModel
    {
        public List<CarritoItemViewModel> Items { get; set; } = new List<CarritoItemViewModel>();

        public decimal Total => Items.Sum(i => i.Subtotal);
    }
}
