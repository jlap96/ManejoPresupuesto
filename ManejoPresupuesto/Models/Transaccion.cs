using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ManejoPresupuesto.Models
{
    public class Transaccion
    {
        public int Id { get; set; } 
        public int UsuarioId { get; set; }
        //Agregamos el nombre que aparecera en el formulario
        [Display(Name="Fecha Transacción")]
        //Agregamos el tipo de dato indicando que sólo se mostrará la fecha y no la hora
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;
        public decimal Monto { get; set; }
        //Agregamos atributos
        [Range(1, maximum:int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]

        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage ="La nota no puede pasar de {1} caracteres")]
        public string Nota { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        [Display(Name = "Cuenta")]
        public int CuentaId { get; set; }
        [Display(Name = "Tipo Operación")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;

    }
}
