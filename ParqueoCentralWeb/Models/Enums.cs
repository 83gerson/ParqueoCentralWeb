namespace ParqueoCentralWeb.Models
{
    /// <summary>
    /// Tipos de vehículo que pueden registrarse en el estacionamiento.
    /// </summary>
    public enum TipoVehiculo
    {
        Automovil,
        Motocicleta,
        Buseta,
        Otro
    }

    /// <summary>
    /// Clasificación de los espacios de estacionamiento.
    /// </summary>
    public enum TipoEspacio
    {
        Automovil,
        Motocicleta,
        Preferencial,
        Otro
    }

    /// <summary>
    /// Estado operativo de un espacio de estacionamiento.
    /// </summary>
    public enum EstadoEspacio
    {
        Disponible,
        Ocupado
    }

    /// <summary>
    /// Estado de un movimiento (entrada/salida) de un vehículo.
    /// </summary>
    public enum EstadoMovimiento
    {
        Activo,
        Finalizado
    }
}
