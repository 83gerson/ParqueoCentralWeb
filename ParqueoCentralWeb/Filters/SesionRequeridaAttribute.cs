using System.Web.Mvc;

namespace ParqueoCentralWeb.Filters
{
    /// <summary>
    /// Filtro de acción que pide que exista un operador identificado en la sesión
    /// (HU-12) antes de permitir el acceso a un controlador o acción. Si no hay
    /// datos de sesión, redirige a la pantalla de identificación del operador.
    /// </summary>
    public class SesionRequeridaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sesion = filterContext.HttpContext.Session;

            if (sesion != null && sesion["NombreOperador"] == null)
            {
                filterContext.Result = new RedirectResult("~/Sesion/Iniciar");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
