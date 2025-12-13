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
    /* Controlador únicamente para las vistas de reportes.
       Los datos los consumirá desde la API REST usando JS. */
    public class ReportesController : Controller
    {
        public ActionResult Diario()
        {
            // La vista tendrá un datepicker y un botón para llamar a la API.
            return View();
        }
    }
}
