using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;
using POS_FeriaUniversitaria.Servicios.Implementaciones;
using POS_FeriaUniversitaria.Servicios.Interfaces;

namespace POS_FeriaUniversitaria.Web.Controllers
{
    /// <summary>
    /// Controlador MVC para administrar el inventario de productos.
    /// Corresponde a la capa "Vistas" del diagrama.
    /// </summary>
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductosController()
        {
            _productoService = new ProductoService();
        }

        // GET: Productos
        public ActionResult Index()
        {
            var productos = _productoService.ObtenerTodos();
            return View(productos);
        }

        /// <summary>
        /// Muestra el historial de productos (activos + eliminados recientes).
        /// </summary>
        public ActionResult Historial()          // NUEVO
        {
            var productos = _productoService.ObtenerHistorial();
            return View(productos);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Producto modelo, HttpPostedFileBase imagenPortada)
        {
            // Validar el archivo si el usuario subió uno
            if (imagenPortada != null && imagenPortada.ContentLength > 0)
            {
                const int maxBytes = 10 * 1024 * 1024; // 10 MB

                if (imagenPortada.ContentLength > maxBytes)
                {
                    ModelState.AddModelError("ImagenPortada", "La imagen no puede superar los 10 MB.");
                }
                else
                {
                    // Validar extensión básica (opcional, pero recomendable)
                    var extension = Path.GetExtension(imagenPortada.FileName)?.ToLower();
                    string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".gif" };

                    if (Array.IndexOf(extensionesPermitidas, extension) < 0)
                    {
                        ModelState.AddModelError("ImagenPortada", "Solo se permiten imágenes JPG, JPEG, PNG o GIF.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                // Si hay errores de validación (modelo o imagen), regresar a la vista
                return View(modelo);
            }

            // Si el modelo es válido, procesar imagen (si existe)
            if (imagenPortada != null && imagenPortada.ContentLength > 0)
            {
                string carpetaVirtual = "~/Content/ImagenesProductos";
                string carpetaFisica = Server.MapPath(carpetaVirtual);

                // Crear carpeta si no existe
                if (!Directory.Exists(carpetaFisica))
                {
                    Directory.CreateDirectory(carpetaFisica);
                }

                // Nombre único para evitar colisiones
                string extension = Path.GetExtension(imagenPortada.FileName);
                string nombreArchivo = Guid.NewGuid().ToString() + extension;
                string rutaCompleta = Path.Combine(carpetaFisica, nombreArchivo);

                imagenPortada.SaveAs(rutaCompleta);

                // Guardamos la ruta relativa en el modelo
                string rutaVirtual = VirtualPathUtility.Combine(carpetaVirtual + "/", nombreArchivo);
                modelo.ImagenPortada = rutaVirtual;
            }

            // Por defecto los productos nuevos se crean activos
            modelo.Activo = true;
            _productoService.Crear(modelo);

            return RedirectToAction("Index");
        }

        // GET: Productos/Edit/5
        public ActionResult Edit(int id)
        {
            var producto = _productoService.ObtenerPorId(id);
            if (producto == null) return HttpNotFound();
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Producto modelo, HttpPostedFileBase imagenPortada)
        {
            // Validar archivo si subió uno nuevo
            if (imagenPortada != null && imagenPortada.ContentLength > 0)
            {
                const int maxBytes = 10 * 1024 * 1024; // 10 MB

                if (imagenPortada.ContentLength > maxBytes)
                {
                    ModelState.AddModelError("ImagenPortada", "La imagen no puede superar los 10 MB.");
                }
                else
                {
                    var extension = Path.GetExtension(imagenPortada.FileName)?.ToLower();
                    string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".gif" };

                    if (Array.IndexOf(extensionesPermitidas, extension) < 0)
                    {
                        ModelState.AddModelError("ImagenPortada", "Solo se permiten imágenes JPG, JPEG, PNG o GIF.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Si el usuario sube una nueva imagen, reemplazamos la ruta
            if (imagenPortada != null && imagenPortada.ContentLength > 0)
            {
                string carpetaVirtual = "~/Content/ImagenesProductos";
                string carpetaFisica = Server.MapPath(carpetaVirtual);

                if (!Directory.Exists(carpetaFisica))
                {
                    Directory.CreateDirectory(carpetaFisica);
                }

                string extension = Path.GetExtension(imagenPortada.FileName);
                string nombreArchivo = Guid.NewGuid().ToString() + extension;
                string rutaCompleta = Path.Combine(carpetaFisica, nombreArchivo);

                imagenPortada.SaveAs(rutaCompleta);

                string rutaVirtual = VirtualPathUtility.Combine(carpetaVirtual + "/", nombreArchivo);
                modelo.ImagenPortada = rutaVirtual;
            }
            // Si NO se sube archivo, se conservará el valor actual de ImagenPortada
            // gracias al HiddenFor en la vista Edit (lo agregamos abajo).

            _productoService.Editar(modelo);
            return RedirectToAction("Index");
        }


        // GET: Productos/Delete/5
        public ActionResult Delete(int id)
        {
            var producto = _productoService.ObtenerPorId(id);
            if (producto == null) return HttpNotFound();
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Eliminación lógica: el producto pasa a histórico
            _productoService.Eliminar(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Elimina definitivamente un producto desde el historial.
        /// Esta acción solo se llama desde la vista Historial.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarDefinitivo(int id)    // NUEVO
        {
            _productoService.EliminarDefinitivo(id);
            return RedirectToAction("Historial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Restaurar(int id)   // NUEVO
        {
            _productoService.Restaurar(id);
            return RedirectToAction("Historial");
        }
    }
}
