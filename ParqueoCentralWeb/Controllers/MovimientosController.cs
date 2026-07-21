using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ParqueoCentralWeb.Filters;
using ParqueoCentralWeb.Helpers;
using ParqueoCentralWeb.Models;

namespace ParqueoCentralWeb.Controllers
{
    /// <summary>
    /// Registro de entradas y salidas de vehículos, cálculo de cobro y consulta
    /// del historial de movimientos (HU-06 a HU-11).
    /// </summary>
    [SesionRequerida]
    public class MovimientosController : Controller
    {
        private readonly ParqueoContext db = new ParqueoContext();

        // GET: Movimientos (historial)
        public ActionResult Index(string filtroPlaca, string filtroEstado, DateTime? filtroFecha)
        {
            ViewBag.FiltroPlaca = filtroPlaca;
            ViewBag.FiltroEstado = filtroEstado;
            ViewBag.FiltroFecha = filtroFecha.HasValue ? filtroFecha.Value.ToString("yyyy-MM-dd") : "";

            return View(ObtenerMovimientos(filtroPlaca, filtroEstado, filtroFecha));
        }

        // GET: Movimientos/Buscar (AJAX, filtra el historial sin recargar la página - HU-10 criterio 5)
        public ActionResult Buscar(string filtroPlaca, string filtroEstado, DateTime? filtroFecha)
        {
            return PartialView("_TablaMovimientos", ObtenerMovimientos(filtroPlaca, filtroEstado, filtroFecha));
        }

        private List<MovimientoEstacionamiento> ObtenerMovimientos(string filtroPlaca, string filtroEstado, DateTime? filtroFecha)
        {
            var query = db.Movimientos.Include("Vehiculo").Include("Espacio").AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroPlaca))
            {
                var filtro = filtroPlaca.Trim().ToUpper();
                query = query.Where(m => m.Vehiculo.Placa.ToUpper().Contains(filtro));
            }

            if (!string.IsNullOrWhiteSpace(filtroEstado))
            {
                EstadoMovimiento estadoEnum;
                if (Enum.TryParse(filtroEstado, true, out estadoEnum))
                {
                    query = query.Where(m => m.EstadoMovimiento == estadoEnum);
                }
            }

            if (filtroFecha.HasValue)
            {
                var fecha = filtroFecha.Value.Date;
                query = query.Where(m => DbFunctions.TruncateTime(m.FechaHoraEntrada) == fecha);
            }

            return query.OrderByDescending(m => m.FechaHoraEntrada).ToList();
        }

        // GET: Movimientos/Details/5
        public ActionResult Details(int id)
        {
            var movimiento = db.Movimientos.Include("Vehiculo").Include("Espacio")
                .FirstOrDefault(m => m.IdMovimiento == id);

            if (movimiento == null)
            {
                TempData["Error"] = "El movimiento solicitado no existe.";
                return RedirectToAction("Index");
            }

            return View(movimiento);
        }

        // GET: Movimientos/Entrada
        public ActionResult Entrada()
        {
            return View();
        }

        // POST: Movimientos/Entrada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Entrada(string placa, TipoVehiculo? tipoVehiculoNuevo, string propietarioNuevo, string contactoNuevo, int idEspacio)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                ModelState.AddModelError("", "Debe indicar la placa del vehículo.");
                return View();
            }

            var placaNormalizada = placa.Trim().ToUpper();
            var vehiculo = db.Vehiculos.FirstOrDefault(v => v.Placa.ToUpper() == placaNormalizada);

            if (vehiculo == null)
            {
                if (tipoVehiculoNuevo == null || string.IsNullOrWhiteSpace(propietarioNuevo))
                {
                    ModelState.AddModelError("", "El vehículo no existe. Complete el tipo de vehículo y el propietario para registrarlo.");
                    ViewBag.PlacaIngresada = placa;
                    return View();
                }

                vehiculo = new Vehiculo
                {
                    Placa = placaNormalizada,
                    TipoVehiculo = tipoVehiculoNuevo.Value,
                    Propietario = propietarioNuevo.Trim(),
                    Contacto = string.IsNullOrWhiteSpace(contactoNuevo) ? null : contactoNuevo.Trim()
                };

                db.Vehiculos.Add(vehiculo);
                db.SaveChanges();
            }

            var tieneEntradaActiva = db.Movimientos.Any(m => m.IdVehiculo == vehiculo.IdVehiculo && m.EstadoMovimiento == EstadoMovimiento.Activo);
            if (tieneEntradaActiva)
            {
                ModelState.AddModelError("", "Este vehículo ya tiene una entrada activa registrada.");
                ViewBag.PlacaIngresada = placa;
                return View();
            }

            var espacio = db.Espacios.Find(idEspacio);
            if (espacio == null || !espacio.Activo || espacio.Estado != EstadoEspacio.Disponible)
            {
                ModelState.AddModelError("", "El espacio seleccionado ya no está disponible. Seleccione otro.");
                ViewBag.PlacaIngresada = placa;
                return View();
            }

            var movimiento = new MovimientoEstacionamiento
            {
                IdVehiculo = vehiculo.IdVehiculo,
                IdEspacio = espacio.IdEspacio,
                FechaHoraEntrada = DateTime.Now,
                EstadoMovimiento = EstadoMovimiento.Activo,
                UsuarioRegistro = Session["NombreOperador"] as string
            };

            espacio.Estado = EstadoEspacio.Ocupado;

            db.Movimientos.Add(movimiento);
            db.SaveChanges();

            TempData["Mensaje"] = "Entrada registrada correctamente para la placa " + vehiculo.Placa + ".";
            return RedirectToAction("Details", new { id = movimiento.IdMovimiento });
        }

        // GET: Movimientos/Salida?placa=... (busca la entrada activa por placa)
        public ActionResult Salida(string placa)
        {
            ViewBag.PlacaBuscada = placa;

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var placaNormalizada = placa.Trim().ToUpper();
                var movimiento = db.Movimientos.Include("Vehiculo").Include("Espacio")
                    .FirstOrDefault(m => m.Vehiculo.Placa.ToUpper() == placaNormalizada && m.EstadoMovimiento == EstadoMovimiento.Activo);

                if (movimiento == null)
                {
                    ViewBag.MensajeBusqueda = "No se encontró una entrada activa para la placa indicada.";
                }
                else
                {
                    return View(movimiento);
                }
            }

            return View((MovimientoEstacionamiento)null);
        }

        // POST: Movimientos/RegistrarSalida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarSalida(int idMovimiento)
        {
            var movimiento = db.Movimientos.Include("Vehiculo").Include("Espacio")
                .FirstOrDefault(m => m.IdMovimiento == idMovimiento);

            if (movimiento == null || movimiento.EstadoMovimiento != EstadoMovimiento.Activo)
            {
                TempData["Error"] = "El movimiento indicado no existe o ya fue finalizado.";
                return RedirectToAction("Index");
            }

            var ahora = DateTime.Now;
            movimiento.FechaHoraSalida = ahora;
            movimiento.MontoCobrado = TarifaHelper.CalcularMonto(movimiento.FechaHoraEntrada, ahora, movimiento.Vehiculo.TipoVehiculo);
            movimiento.EstadoMovimiento = EstadoMovimiento.Finalizado;

            var espacio = db.Espacios.Find(movimiento.IdEspacio);
            if (espacio != null)
            {
                espacio.Estado = EstadoEspacio.Disponible;
            }

            db.SaveChanges();

            TempData["Mensaje"] = "Salida registrada correctamente. Monto a cobrar: ₡" + movimiento.MontoCobrado.Value.ToString("N2");
            return RedirectToAction("Details", new { id = movimiento.IdMovimiento });
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
