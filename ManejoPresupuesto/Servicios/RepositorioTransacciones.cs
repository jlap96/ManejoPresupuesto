using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace ManejoPresupuesto.Servicios
{
    //Creamos la interface de la clase. Esto nos permitirá estar seguros que, las clases que la implemente,
    //van a cumplir las tareas que se definen en la interfaz
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Crear(Transaccion transaccion);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly string connectionString;
        //Creamos el constructor
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Creamos el método para crear una transacción 
        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            //Para realizar el insert vamos a utilizar un procedimiento almacenado, pues el insert de Transacciones es más complejo
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
              new
              {
                  transaccion.UsuarioId,
                  transaccion.FechaTransaccion,
                  transaccion.Monto,
                  transaccion.CategoriaId,
                  transaccion.CuentaId,
                  transaccion.Nota
              },
              commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior,
            int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        //Creamos un método para obtener la transacció por Id 

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT Transacciones.*, cat.TipoOperacionId
                FROM Transacciones
                INNER JOIN Categorias cat
                ON cat.Id = Transacciones.CategoriaId  
                WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId",
                new { id, usuarioId });
        }

    }
}
