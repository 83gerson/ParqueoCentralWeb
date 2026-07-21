using System.Linq;
using System.Web.Mvc;
using ParqueoCentralWeb.Filters;
using ParqueoCentralWeb.Models;

namespace ParqueoCentralWeb.Controllers
{
    [SesionRequerida]
    public class HomeController : Controller
    {
        private readonly ParqueoContext db = new ParqueoContext();

        // GET: Home
        public ActionResult Index()
        {
            ViewBag.NombreOperador = Session["NombreOperador"];
            ViewBag.TotalVehiculos = db.Vehiculos.Count();
            ViewBag.EspaciosDisponibles = db.Espacios.Count(e => e.Activo && e.Estado == EstadoEspacio.Disponible);
            ViewBag.EspaciosOcupados = db.Espacios.Count(e => e.Activo && e.Estado == EstadoEspacio.Ocupado);
            ViewBag.MovimientosActivos = db.Movimientos.Count(m => m.EstadoMovimiento == EstadoMovimiento.Activo);

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
