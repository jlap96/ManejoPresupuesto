using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Validaciones
{
    public class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Validamos que si es nulo el campo, se acepte como nulo
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            
            {
                return ValidationResult.Success;
            }

            //Obtenemos la primer letra
            var primeraLetra = value.ToString()[0].ToString();

            //Validamos que si la primer letra no es mayuscula, se muestre el error
            if(primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primer letra debe ser mayúscula");
            }

            //Si no se cumple el if anterior, la validación es exitosa
            return ValidationResult.Success;
        }
    }
}
