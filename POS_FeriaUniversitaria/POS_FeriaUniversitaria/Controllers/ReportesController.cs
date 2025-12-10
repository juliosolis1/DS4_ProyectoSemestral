using System.Web.Mvc;

namespace POS_FeriaUniversitaria.Web.Controllers
{
    /// <summary>
    /// Controlador únicamente para las vistas de reportes.
    /// Los datos los consumirá desde la API REST usando JS.
    /// </summary>
    public class ReportesController : Controller
    {
        public ActionResult Diario()
        {
            // La vista tendrá un datepicker y un botón para llamar a la API.
            return View();
        }
    }
}
