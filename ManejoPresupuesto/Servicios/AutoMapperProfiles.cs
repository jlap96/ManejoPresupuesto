using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
            /*Configuramos el autoMapper ReverseMap porque vamos a llevar de TransaccionActualizacionViewModel
             a Transaccion, y despues de transaccion a TransaccionActualizacionViewModel*/
            CreateMap<TransaccionActualizacionViewModel, Transaccion>().ReverseMap();   
        }
    }
}
