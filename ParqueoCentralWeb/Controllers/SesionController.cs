using System;
using System.Web.Mvc;

namespace ParqueoCentralWeb.Controllers
{
    /// <summary>
    /// Controla la identificación del operador que utiliza el sistema y el
    /// mantenimiento de la información temporal en la sesión (HU-12).
    /// </summary>
    public class SesionController : Controller
    {
        // GET: Sesion/Iniciar
        public ActionResult Iniciar()
        {
            if (Session["NombreOperador"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Sesion/Iniciar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Iniciar(string nombreOperador)
        {
            if (string.IsNullOrWhiteSpace(nombreOperador))
            {
                ModelState.AddModelError("", "Debe ingresar su nombre para continuar.");
                return View();
            }

            Session["NombreOperador"] = nombreOperador.Trim();
            Session["HoraInicioSesion"] = DateTime.Now;

            TempData["Mensaje"] = "Bienvenido(a), " + nombreOperador.Trim() + ".";
            return RedirectToAction("Index", "Home");
        }

        // GET: Sesion/Salir
        public ActionResult Salir()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Iniciar");
        }
    }
}
