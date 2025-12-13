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

using System.Web.Mvc;

namespace POS_FeriaUniversitaria.Web.Controllers
{
    /* Controlador principal de la aplicación web.
       Maneja la navegación hacia la pantalla principal, página de información (About) y página de contacto. */
    public class HomeController : Controller
    {
        /* Pantalla principal del Mini POS.
           Desde aquí el usuario elige a qué módulo ir: Inventario (productos), Nueva Venta, Reporte Diario. */
        public ActionResult Index()
        {
            return View();
        }

        /* Página "About" (Acerca de).
           Muestra una descripción del proyecto semestral y del desarrollador (Julio Solís). */
        public ActionResult About()
        {
            // Título que se muestra en la pestaña del navegador
            ViewBag.Title = "Acerca de Mini POS - Feria Universitaria";

            // Mensaje corto que se puede usar en la vista
            ViewBag.Message = "Proyecto semestral de Desarrollo de Software IV.";

            return View();
        }

        /* Página "Contact" (Contacto).
           Muestra los datos de contacto del desarrollador para dudas, sugerencias o comentarios sobre el sistema. */
        public ActionResult Contact()
        {
            ViewBag.Title = "Contacto";
            ViewBag.Message = "Información de contacto del desarrollador del Mini POS.";

            return View();
        }
    }
}
