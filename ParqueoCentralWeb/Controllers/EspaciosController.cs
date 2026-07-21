using System.Linq;
using System.Web.Mvc;
using ParqueoCentralWeb.Filters;
using ParqueoCentralWeb.Models;

namespace ParqueoCentralWeb.Controllers
{
    /// <summary>
    /// CRUD de espacios de estacionamiento y consulta de disponibilidad (HU-04, HU-05, HU-07).
    /// </summary>
    [SesionRequerida]
    public class EspaciosController : Controller
    {
        private readonly ParqueoContext db = new ParqueoContext();

        // GET: Espacios
        public ActionResult Index()
        {
            return View(db.Espacios.OrderBy(e => e.CodigoEspacio).ToList());
        }

        // GET: Espacios/Estado (AJAX, actualiza la disponibilidad sin recargar la página - HU-05 criterio 4)
        public ActionResult Estado()
        {
            return PartialView("_TablaEspacios", db.Espacios.OrderBy(e => e.CodigoEspacio).ToList());
        }

        // GET: Espacios/Disponibles?tipo=Automovil (AJAX, usado al registrar una entrada)
        public JsonResult Disponibles(string tipo)
        {
            var query = db.Espacios.Where(e => e.Activo && e.Estado == EstadoEspacio.Disponible);

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                TipoEspacio tipoEnum;
                if (System.Enum.TryParse(tipo, true, out tipoEnum))
                {
                    query = query.Where(e => e.TipoEspacio == tipoEnum);
                }
            }

            var lista = query.OrderBy(e => e.CodigoEspacio)
                .Select(e => new { e.IdEspacio, e.CodigoEspacio, Tipo = e.TipoEspacio.ToString() })
                .ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // GET: Espacios/Create
        public ActionResult Create()
        {
            return View(new EspacioEstacionamiento { Activo = true, Estado = EstadoEspacio.Disponible });
        }

        // POST: Espacios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EspacioEstacionamiento espacio)
        {
            if (!string.IsNullOrWhiteSpace(espacio.CodigoEspacio) && CodigoExiste(espacio.CodigoEspacio, null))
            {
                ModelState.AddModelError("CodigoEspacio", "Ya existe un espacio registrado con ese código.");
            }

            if (ModelState.IsValid)
            {
                espacio.CodigoEspacio = espacio.CodigoEspacio.Trim().ToUpper();
                espacio.Estado = EstadoEspacio.Disponible;
                db.Espacios.Add(espacio);
                db.SaveChanges();

                TempData["Mensaje"] = "Espacio de estacionamiento registrado correctamente.";
                return RedirectToAction("Index");
            }

            return View(espacio);
        }

        // GET: Espacios/Edit/5
        public ActionResult Edit(int id)
        {
            var espacio = db.Espacios.Find(id);
            if (espacio == null)
            {
                TempData["Error"] = "El espacio solicitado no existe.";
                return RedirectToAction("Index");
            }

            return View(espacio);
        }

        // POST: Espacios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EspacioEstacionamiento cambios)
        {
            var espacio = db.Espacios.Find(id);
            if (espacio == null)
            {
                TempData["Error"] = "El espacio solicitado no existe.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                cambios.IdEspacio = id;
                cambios.CodigoEspacio = espacio.CodigoEspacio;
                return View(cambios);
            }

            if (espacio.Estado == EstadoEspacio.Ocupado && !cambios.Activo)
            {
                ModelState.AddModelError("", "No se puede desactivar un espacio que está actualmente ocupado.");
                cambios.IdEspacio = id;
                cambios.CodigoEspacio = espacio.CodigoEspacio;
                return View(cambios);
            }

            espacio.TipoEspacio = cambios.TipoEspacio;
            espacio.Activo = cambios.Activo;
            db.SaveChanges();

            TempData["Mensaje"] = "Espacio actualizado correctamente.";
            return RedirectToAction("Index");
        }

        private bool CodigoExiste(string codigo, int? idExcluir)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return false;
            }

            var normalizado = codigo.Trim().ToUpper();
            var query = db.Espacios.Where(e => e.CodigoEspacio.ToUpper() == normalizado);

            if (idExcluir.HasValue)
            {
                query = query.Where(e => e.IdEspacio != idExcluir.Value);
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
