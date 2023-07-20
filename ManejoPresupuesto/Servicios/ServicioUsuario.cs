namespace ManejoPresupuesto.Servicios
{
    public interface IServicioUsuarios
    {
        int ObtenerUsuarioId();
    }
    public class ServicioUsuario: IServicioUsuarios
    {
        public int ObtenerUsuarioId()
        {
            return 1;
        }
    }
}
