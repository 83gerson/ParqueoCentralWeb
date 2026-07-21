using System;
using ParqueoCentralWeb.Models;

namespace ParqueoCentralWeb.Helpers
{
    /// <summary>
    /// Calcula el monto a cobrar por el tiempo de permanencia de un vehículo (HU-09).
    /// La tarifa varía según el tipo de vehículo y se cobra por hora o fracción.
    /// </summary>
    public static class TarifaHelper
    {
        public static decimal ObtenerTarifaPorHora(TipoVehiculo tipo)
        {
            switch (tipo)
            {
                case TipoVehiculo.Automovil:
                    return 500m;
                case TipoVehiculo.Motocicleta:
                    return 300m;
                case TipoVehiculo.Buseta:
                    return 800m;
                default:
                    return 500m;
            }
        }

        public static int CalcularHoras(DateTime entrada, DateTime salida)
        {
            var minutos = (salida - entrada).TotalMinutes;
            if (minutos <= 0)
            {
                return 1;
            }

            var horas = (int)Math.Ceiling(minutos / 60.0);
            return horas < 1 ? 1 : horas;
        }

        public static decimal CalcularMonto(DateTime entrada, DateTime salida, TipoVehiculo tipo)
        {
            var horas = CalcularHoras(entrada, salida);
            var tarifa = ObtenerTarifaPorHora(tipo);
            return horas * tarifa;
        }
    }
}
