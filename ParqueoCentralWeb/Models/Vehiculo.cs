using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParqueoCentralWeb.Models
{
    /// <summary>
    /// Representa un vehículo registrado en el sistema (HU-01, HU-02, HU-03).
    /// </summary>
    public class Vehiculo
    {
        [Key]
        public int IdVehiculo { get; set; }

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(15, ErrorMessage = "La placa no puede tener más de 15 caracteres.")]
        [Display(Name = "Placa")]
        [Index("IX_Vehiculo_Placa", IsUnique = true)]
        public string Placa { get; set; }

        [Required(ErrorMessage = "El tipo de vehículo es obligatorio.")]
        [Display(Name = "Tipo de vehículo")]
        public TipoVehiculo TipoVehiculo { get; set; }

        [Required(ErrorMessage = "El nombre del propietario o conductor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El propietario no puede tener más de 100 caracteres.")]
        [Display(Name = "Propietario / Conductor")]
        public string Propietario { get; set; }

        [StringLength(30, ErrorMessage = "El contacto no puede tener más de 30 caracteres.")]
        [Display(Name = "Contacto (opcional)")]
        public string Contacto { get; set; }

        public virtual ICollection<MovimientoEstacionamiento> Movimientos { get; set; }
    }
}
