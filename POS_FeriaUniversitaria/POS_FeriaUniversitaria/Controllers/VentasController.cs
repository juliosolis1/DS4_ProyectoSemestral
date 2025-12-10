using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using POS_FeriaUniversitaria.Servicios.Interfaces;
using POS_FeriaUniversitaria.Servicios.Implementaciones;
using POS_FeriaUniversitaria.Web.Models;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;

namespace POS_FeriaUniversitaria.Web.Controllers
{
    /// <summary>
    /// Flujo de "Nueva Venta / Carrito".
    /// Usa sesión para guardar el carrito.
    /// </summary>
    public class VentasController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IVentaService _ventaService;

        private const string SESSION_CART = "CARRITO_VENTA";

        public VentasController()
        {
            _productoService = new ProductoService();
            _ventaService = new VentaService();
        }

        // GET: Ventas/Nueva
        public ActionResult Nueva()
        {
            var vm = ObtenerCarritoDeSesion();
            ViewBag.Productos = _productoService.ObtenerTodos()
                                 .Where(p => p.Activo && p.Stock > 0)
                                 .OrderBy(p => p.Nombre)
                                 .ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarItem(int productoId, int cantidad = 1)
        {
            if (cantidad <= 0) cantidad = 1;

            var producto = _productoService.ObtenerPorId(productoId);
            if (producto == null || !producto.Activo || producto.Stock <= 0)
            {
                TempData["Error"] = "Producto inválido o sin stock.";
                return RedirectToAction("Nueva");
            }

            var carrito = ObtenerCarritoDeSesion();
            var existente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (existente == null)
            {
                carrito.Items.Add(new CarritoItemViewModel
                {
                    ProductoId = producto.ProductoId,
                    ImagenPortada = producto.ImagenPortada,
                    Nombre = producto.Nombre,
                    PrecioUnitario = producto.PrecioVenta,
                    Cantidad = Math.Min(cantidad, producto.Stock)
                });
            }
            else
            {
                // Suma cantidad sin superar stock disponible
                var nuevaCantidad = existente.Cantidad + cantidad;
                existente.Cantidad = Math.Min(nuevaCantidad, producto.Stock);
            }

            GuardarCarritoEnSesion(carrito);
            return RedirectToAction("Nueva");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarCantidad(int productoId, int cantidad)
        {
            var carrito = ObtenerCarritoDeSesion();
            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                var p = _productoService.ObtenerPorId(productoId);
                if (p != null)
                {
                    if (cantidad <= 0) carrito.Items.Remove(item);
                    else item.Cantidad = Math.Min(cantidad, p.Stock);
                }
            }
            GuardarCarritoEnSesion(carrito);
            return RedirectToAction("Nueva");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuitarItem(int productoId)
        {
            var carrito = ObtenerCarritoDeSesion();
            carrito.Items.RemoveAll(i => i.ProductoId == productoId);
            GuardarCarritoEnSesion(carrito);
            return RedirectToAction("Nueva");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarVenta(string observaciones)
        {
            var carrito = ObtenerCarritoDeSesion();
            if (carrito.Items.Count == 0)
            {
                TempData["Error"] = "El carrito está vacío.";
                return RedirectToAction("Nueva");
            }

            // Mapear a entidades de dominio para la capa de servicios
            var detalles = carrito.Items.Select(i => new DetalleVenta
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            }).ToList();

            var venta = new Venta
            {
                Fecha = DateTime.Now,
                Total = carrito.Total,
                Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
            };

            try
            {
                int ventaId = _ventaService.RegistrarVenta(venta, detalles);
                // limpiar carrito luego de venta
                Session.Remove(SESSION_CART);
                TempData["Ok"] = $"Venta #{ventaId} registrada correctamente.";
                return RedirectToAction("Nueva");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No fue posible registrar la venta: " + ex.Message;
                return RedirectToAction("Nueva");
            }
        }

        // =================== helpers de sesión ===================
        private CarritoViewModel ObtenerCarritoDeSesion()
        {
            var carrito = Session[SESSION_CART] as CarritoViewModel;
            if (carrito == null)
            {
                carrito = new CarritoViewModel();
                Session[SESSION_CART] = carrito;
            }
            return carrito;
        }

        private void GuardarCarritoEnSesion(CarritoViewModel carrito)
            => Session[SESSION_CART] = carrito;
    }
}
