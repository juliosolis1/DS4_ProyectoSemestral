using System.Web.Mvc;

namespace POS_FeriaUniversitaria.Web.Controllers
{
    /// <summary>
    /// Controlador principal de la aplicación web.
    /// Maneja la navegación hacia la pantalla principal,
    /// página de información (About) y página de contacto.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Pantalla principal del Mini POS.
        /// Desde aquí el usuario elige a qué módulo ir:
        /// Inventario (productos), Nueva Venta, Reporte Diario.
        /// </summary>
        /// <returns>Vista Index con el menú principal.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Página "About" (Acerca de).
        /// Muestra una descripción del proyecto semestral
        /// y del desarrollador (Julio Solís).
        /// </summary>
        /// <returns>Vista About con información del proyecto.</returns>
        public ActionResult About()
        {
            // Título que se muestra en la pestaña del navegador
            ViewBag.Title = "Acerca de Mini POS - Feria Universitaria";

            // Mensaje corto que se puede usar en la vista
            ViewBag.Message = "Proyecto semestral de Desarrollo de Software IV.";

            return View();
        }

        /// <summary>
        /// Página "Contact" (Contacto).
        /// Muestra los datos de contacto del desarrollador
        /// para dudas, sugerencias o comentarios sobre el sistema.
        /// </summary>
        /// <returns>Vista Contact con la información de contacto.</returns>
        public ActionResult Contact()
        {
            ViewBag.Title = "Contacto";
            ViewBag.Message = "Información de contacto del desarrollador del Mini POS.";

            return View();
        }
    }
}
