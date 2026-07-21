using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParqueoCentralWeb.Models
{
    /// <summary>
    /// Representa una entrada/salida de un vehículo del estacionamiento (HU-06 a HU-11).
    /// </summary>
    public class MovimientoEstacionamiento
    {
        [Key]
        public int IdMovimiento { get; set; }

        [Required]
        public int IdVehiculo { get; set; }

        [ForeignKey("IdVehiculo")]
        public virtual Vehiculo Vehiculo { get; set; }

        [Required]
        public int IdEspacio { get; set; }

        [ForeignKey("IdEspacio")]
        public virtual EspacioEstacionamiento Espacio { get; set; }

        [Display(Name = "Fecha y hora de entrada")]
        public DateTime FechaHoraEntrada { get; set; }

        [Display(Name = "Fecha y hora de salida")]
        public DateTime? FechaHoraSalida { get; set; }

        [Display(Name = "Estado del movimiento")]
        public EstadoMovimiento EstadoMovimiento { get; set; }

        [Display(Name = "Monto cobrado (₡)")]
        [Column(TypeName = "decimal")]
        public decimal? MontoCobrado { get; set; }

        [StringLength(50)]
        [Display(Name = "Usuario que registró")]
        public string UsuarioRegistro { get; set; }
    }
}
