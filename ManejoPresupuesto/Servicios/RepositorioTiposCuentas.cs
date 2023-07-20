using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        //Accedemos al connection string a traves del IConfiguration
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Creamos el método por el cual vamos a poder crear un tipo de cuenta en la base de datos
        //Lo convertimos en Asincrono agregando el async Task, donde Task representa una operación que finalizara en el futuro
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            //Al pasar como parametro TipoCuenta, va tomar los valores del Modelo TipoCuenta, como es Nombre, Usuario Id
            var id = await connection.QuerySingleAsync<int>
                                                    (@"INSERT INTO TipoCuenta (Nombre, UsuarioId, Orden)
                                                    Values (@Nombre, @UsuarioId, 0);
                                                    SELECT SCOPE_IDENTITY();", tipoCuenta);
            tipoCuenta.Id = id;
        }

        //Creamos un método para validar a nivel del controlador que no existan dos nombres iguales para tipos de cuentas
        public async Task <bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            
            //nos mostrara lo primero que encuentre o un valor por defecto con QueryFirtsOrDefault
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                                        @"SELECT 1 
                                                                        FROM TipoCuenta 
                                                                        WHERE Nombre = @nombre AND UsuarioId = @UsuarioId;",
                                                                        new {nombre, usuarioId});
            //Si el registro existe, retornara un 1. Si no existe, retornara un 0 pues con el firtsOrDefault indicamos que retornara un 0 
            return existe == 1;
        }

        //Método para mostrar la tabla de Tipos de Cuentas de nuestra base de datos.
        //Pasamos como parametro el usuarioId porque sólo queremos obtener los tiposd de cuentas del usuario que lo solicita
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            //QueryAsync permite realizar el query de SELECT a la base de datos, traera los resultados y los mapea a la clase TipoCuenta
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden 
                                                FROM TipoCuenta     
                                                WHERE UsuarioId = @UsuarioId;", new {usuarioId});
        }

        //Creamos el método para actualizar los tipos de cuentas
        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            //Execute es un query que no retorna nada
            await connection.ExecuteAsync(@"UPDATE TipoCuenta 
                                            SET Nombre = @Nombre 
                                            WHERE Id = @Id", tipoCuenta);
        }

        //Creamos un método para obtener el Tipo de cuenta por su Id
        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"
                                                                     SELECT Id, Nombre, Orden
                                                                     FROM TipoCuenta
                                                                     WHERE Id = @Id AND UsuarioId = @UsuarioId",
                                                                     new { id, usuarioId });

        }

        //Creamos el método para borrar un registro
        public async Task Borrar (int id)
        {
            using var connection = new SqlConnection(connectionString); 
            await connection.ExecuteAsync("DELETE TipoCuenta WHERE Id= @Id", new { id });
        }
    }
}
