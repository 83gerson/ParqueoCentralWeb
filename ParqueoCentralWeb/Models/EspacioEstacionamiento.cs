using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParqueoCentralWeb.Models
{
    /// <summary>
    /// Representa un espacio físico del estacionamiento (HU-04, HU-05).
    /// </summary>
    public class EspacioEstacionamiento
    {
        [Key]
        public int IdEspacio { get; set; }

        [Required(ErrorMessage = "El código de espacio es obligatorio.")]
        [StringLength(10, ErrorMessage = "El código no puede tener más de 10 caracteres.")]
        [Display(Name = "Código de espacio")]
        [Index("IX_Espacio_Codigo", IsUnique = true)]
        public string CodigoEspacio { get; set; }

        [Required(ErrorMessage = "El tipo de espacio es obligatorio.")]
        [Display(Name = "Tipo de espacio")]
        public TipoEspacio TipoEspacio { get; set; }

        [Display(Name = "Estado")]
        public EstadoEspacio Estado { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        public virtual ICollection<MovimientoEstacionamiento> Movimientos { get; set; }
    }
}
