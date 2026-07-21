using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ParqueoCentralWeb.Filters;
using ParqueoCentralWeb.Models;

namespace ParqueoCentralWeb.Controllers
{
    /// <summary>
    /// CRUD de vehículos y búsqueda de placas (HU-01, HU-02, HU-03).
    /// </summary>
    [SesionRequerida]
    public class VehiculosController : Controller
    {
        private readonly ParqueoContext db = new ParqueoContext();

        // GET: Vehiculos
        public ActionResult Index(string filtroPlaca)
        {
            ViewBag.FiltroPlaca = filtroPlaca;
            return View(ObtenerVehiculos(filtroPlaca));
        }

        // GET: Vehiculos/Buscar?filtroPlaca=... (usado por AJAX, HU-02 criterio 4)
        public ActionResult Buscar(string filtroPlaca)
        {
            return PartialView("_TablaVehiculos", ObtenerVehiculos(filtroPlaca));
        }

        private List<Vehiculo> ObtenerVehiculos(string filtroPlaca)
        {
            var query = db.Vehiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroPlaca))
            {
                var filtro = filtroPlaca.Trim().ToUpper();
                query = query.Where(v => v.Placa.ToUpper().Contains(filtro));
            }

            return query.OrderBy(v => v.Placa).ToList();
        }

        // GET: Vehiculos/Create
        public ActionResult Create()
        {
            return View(new Vehiculo());
        }

        // POST: Vehiculos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vehiculo vehiculo)
        {
            if (!string.IsNullOrWhiteSpace(vehiculo.Placa) && PlacaExiste(vehiculo.Placa, null))
            {
                ModelState.AddModelError("Placa", "Ya existe un vehículo registrado con esa placa.");
            }

            if (ModelState.IsValid)
            {
                vehiculo.Placa = vehiculo.Placa.Trim().ToUpper();
                db.Vehiculos.Add(vehiculo);
                db.SaveChanges();

                TempData["Mensaje"] = "Vehículo registrado correctamente.";
                return RedirectToAction("Index");
            }

            return View(vehiculo);
        }

        // GET: Vehiculos/Edit/5
        public ActionResult Edit(int id)
        {
            var vehiculo = db.Vehiculos.Find(id);
            if (vehiculo == null)
            {
                TempData["Error"] = "El vehículo solicitado no existe.";
                return RedirectToAction("Index");
            }

            return View(vehiculo);
        }

        // POST: Vehiculos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Vehiculo cambios)
        {
            var vehiculo = db.Vehiculos.Find(id);
            if (vehiculo == null)
            {
                TempData["Error"] = "El vehículo solicitado no existe.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                cambios.IdVehiculo = id;
                cambios.Placa = vehiculo.Placa;
                return View(cambios);
            }

            // La placa es el identificador principal y no se modifica (criterio HU-03.3).
            vehiculo.TipoVehiculo = cambios.TipoVehiculo;
            vehiculo.Propietario = cambios.Propietario;
            vehiculo.Contacto = cambios.Contacto;
            db.SaveChanges();

            TempData["Mensaje"] = "Datos del vehículo actualizados correctamente.";
            return RedirectToAction("Index");
        }

        // GET: Vehiculos/VerificarPlaca?placa=... (AJAX, validación de duplicados)
        public JsonResult VerificarPlaca(string placa, int? idVehiculo)
        {
            var existe = PlacaExiste(placa, idVehiculo);
            return Json(new { existe = existe }, JsonRequestBehavior.AllowGet);
        }

        private bool PlacaExiste(string placa, int? idVehiculoExcluir)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                return false;
            }

            var placaNormalizada = placa.Trim().ToUpper();
            var query = db.Vehiculos.Where(v => v.Placa.ToUpper() == placaNormalizada);

            if (idVehiculoExcluir.HasValue)
            {
                query = query.Where(v => v.IdVehiculo != idVehiculoExcluir.Value);
            }

            return query.Any();
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
