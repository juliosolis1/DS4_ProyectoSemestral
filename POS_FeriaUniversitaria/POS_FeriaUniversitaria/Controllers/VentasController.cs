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
    /* Controller: VentasController
    - Maneja el flujo de "Nueva Venta" (carrito de compras) del Mini POS.
    - Utiliza Session para almacenar temporalmente el carrito mientras el usuario agrega/quita ítems.
    - Consulta productos disponibles desde IProductoService.
    - Confirma la venta mediante IVentaService (crea cabecera + detalles y descuenta stock).
    - Usa TempData para mostrar mensajes de éxito/error tras redirecciones. */

    public class VentasController : Controller
    {
        // Servicio de productos: consulta inventario (listado, búsqueda por ID, etc.).
        private readonly IProductoService _productoService;
        // Servicio de ventas: registra ventas y delega la transacción a la capa de datos.
        private readonly IVentaService _ventaService;

        // Clave fija de Session donde se almacena el carrito de la venta actual.
        private const string SESSION_CART = "CARRITO_VENTA";

        /* Constructor del controller.
           En un proyecto más avanzado se usaría Inyección de Dependencias (DI).
           Aquí se instancian los servicios directamente para simplificar el proyecto semestral. */

        public VentasController()
        {
            _productoService = new ProductoService();
            _ventaService = new VentaService();
        }

        // Muestra la vista "Nueva" con el carrito actual y la lista de productos disponibles.
        // GET: Ventas/Nueva
        public ActionResult Nueva()
        {
            // Recupera el carrito desde sesión (si no existe, lo crea).
            var vm = ObtenerCarritoDeSesion();
            // Carga productos activos y con stock (>0) para poblar el combo/listado en la vista.
            ViewBag.Productos = _productoService.ObtenerTodos()
                                 .Where(p => p.Activo && p.Stock > 0)
                                 .OrderBy(p => p.Nombre)
                                 .ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        /* Agrega un producto al carrito en Session.
           - Valida cantidad mínima.
           - Verifica que el producto exista, esté activo y tenga stock.
           - Si ya existe en el carrito, suma cantidades sin superar el stock disponible. */
        public ActionResult AgregarItem(int productoId, int cantidad = 1)
        {
            // Normaliza la cantidad: si llega un valor inválido, se ajusta a 1.
            if (cantidad <= 0) cantidad = 1;

            // Se consulta el producto desde la BD vía capa de servicios para validar disponibilidad.
            var producto = _productoService.ObtenerPorId(productoId);
            if (producto == null || !producto.Activo || producto.Stock <= 0)
            {
                TempData["Error"] = "Producto inválido o sin stock.";
                return RedirectToAction("Nueva");
            }

            // Obtiene el carrito actual (en sesión) para agregar o actualizar el ítem.
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
                // Si el producto ya estaba en el carrito, se incrementa la cantidad sin exceder el stock.
                var nuevaCantidad = existente.Cantidad + cantidad;
                existente.Cantidad = Math.Min(nuevaCantidad, producto.Stock);
            }

            // Persistir cambios del carrito en Session antes de recargar la pantalla.
            GuardarCarritoEnSesion(carrito);
            return RedirectToAction("Nueva");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        /* Actualiza la cantidad de un ítem en el carrito:
           - Si la cantidad <= 0, se elimina el ítem.
           - Si la cantidad es válida, se limita al stock disponible del producto. */
        public ActionResult ActualizarCantidad(int productoId, int cantidad)
        {
            var carrito = ObtenerCarritoDeSesion();
            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                // Se vuelve a consultar el producto para respetar el stock actual.
                var p = _productoService.ObtenerPorId(productoId);
                if (p != null)
                {
                    // Regla de UX: cantidad 0 o negativa equivale a quitar el producto del carrito.
                    if (cantidad <= 0) carrito.Items.Remove(item);
                    else item.Cantidad = Math.Min(cantidad, p.Stock);
                }
            }
            GuardarCarritoEnSesion(carrito);
            return RedirectToAction("Nueva");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        // Elimina completamente un producto del carrito (todas sus ocurrencias) por su ProductoId.
        public ActionResult QuitarItem(int productoId)
        {
            var carrito = ObtenerCarritoDeSesion();
            // RemoveAll garantiza que no quede ningún ítem con ese ProductoId dentro del carrito.
            carrito.Items.RemoveAll(i => i.ProductoId == productoId);
            GuardarCarritoEnSesion(carrito);
            return RedirectToAction("Nueva");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        /* Confirma la venta:
           - Valida que el carrito tenga ítems.
           - Mapea el carrito (ViewModel) a entidades de dominio (Venta y DetalleVenta).
           - Llama a la capa de servicios para registrar la venta (transacción en repositorio).
           - Si todo sale bien: limpia Session y muestra mensaje de éxito. */
        public ActionResult ConfirmarVenta(string observaciones)
        {
            var carrito = ObtenerCarritoDeSesion();
            // Validación de negocio: no se puede registrar una venta sin productos.
            if (carrito.Items.Count == 0)
            {
                TempData["Error"] = "El carrito está vacío.";
                return RedirectToAction("Nueva");
            }

            // Validación de negocio: no se puede registrar una venta sin productos.
            var detalles = carrito.Items.Select(i => new DetalleVenta
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            }).ToList();

            // Se construye la cabecera de la venta (fecha, total y observaciones opcionales).
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                Total = carrito.Total,
                Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
            };

            try
            {
                int ventaId = _ventaService.RegistrarVenta(venta, detalles);
                // Limpia el carrito de Session para iniciar una nueva venta desde cero.
                Session.Remove(SESSION_CART);
                TempData["Ok"] = $"Venta #{ventaId} registrada correctamente.";
                return RedirectToAction("Nueva");
            }
            catch (Exception ex)
            {
                // Captura errores del proceso (validaciones/BD) y los muestra al usuario.
                TempData["Error"] = "No fue posible registrar la venta: " + ex.Message;
                return RedirectToAction("Nueva");
            }
        }

        // =================== Helpers de Session (Carrito) ===================

        /* Obtiene el carrito guardado en Session.
           Si no existe todavía, crea uno nuevo y lo almacena para las siguientes acciones. */
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

        // Guarda/reemplaza el carrito actual en Session (persistencia temporal por usuario).
        private void GuardarCarritoEnSesion(CarritoViewModel carrito)
            => Session[SESSION_CART] = carrito;
    }
}
