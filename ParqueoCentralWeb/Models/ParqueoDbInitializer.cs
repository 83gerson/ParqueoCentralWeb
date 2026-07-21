using System.Data.Entity;

namespace ParqueoCentralWeb.Models
{
    /// <summary>
    /// Crea la base de datos automáticamente (si no existe) la primera vez que se
    /// ejecuta la aplicación y pone los datos de ejemplo para facilitar las pruebas.
    /// </summary>
    public class ParqueoDbInitializer : CreateDatabaseIfNotExists<ParqueoContext>
    {
        protected override void Seed(ParqueoContext context)
        {
            context.Espacios.Add(new EspacioEstacionamiento { CodigoEspacio = "A-01", TipoEspacio = TipoEspacio.Automovil, Estado = EstadoEspacio.Disponible, Activo = true });
            context.Espacios.Add(new EspacioEstacionamiento { CodigoEspacio = "A-02", TipoEspacio = TipoEspacio.Automovil, Estado = EstadoEspacio.Disponible, Activo = true });
            context.Espacios.Add(new EspacioEstacionamiento { CodigoEspacio = "A-03", TipoEspacio = TipoEspacio.Automovil, Estado = EstadoEspacio.Disponible, Activo = true });
            context.Espacios.Add(new EspacioEstacionamiento { CodigoEspacio = "M-01", TipoEspacio = TipoEspacio.Motocicleta, Estado = EstadoEspacio.Disponible, Activo = true });
            context.Espacios.Add(new EspacioEstacionamiento { CodigoEspacio = "M-02", TipoEspacio = TipoEspacio.Motocicleta, Estado = EstadoEspacio.Disponible, Activo = true });
            context.Espacios.Add(new EspacioEstacionamiento { CodigoEspacio = "P-01", TipoEspacio = TipoEspacio.Preferencial, Estado = EstadoEspacio.Disponible, Activo = true });
            context.Espacios.Add(new EspacioEstacionamiento { CodigoEspacio = "B-01", TipoEspacio = TipoEspacio.Otro, Estado = EstadoEspacio.Disponible, Activo = true });

            context.Vehiculos.Add(new Vehiculo { Placa = "SJO-1234", TipoVehiculo = TipoVehiculo.Automovil, Propietario = "Juan Pérez", Contacto = "8888-0000" });
            context.Vehiculos.Add(new Vehiculo { Placa = "CL-456789", TipoVehiculo = TipoVehiculo.Motocicleta, Propietario = "María Rodríguez", Contacto = "8888-1111" });

            context.SaveChanges();

            base.Seed(context);
        }
    }
}
