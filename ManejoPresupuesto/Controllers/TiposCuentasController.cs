using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController: Controller
    {
        private readonly string connectionString;
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        //Utilizamos el servicio IRepositorioTipoCuentas y ServicioUsuarios
        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServicioUsuarios servicioUsuarios)
        {
            //Creamos e inicializamos el servicio
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        //Creamos una accion para el listado de tipos cuentas
        //Usamos Index para aquella vista que va mostrar un listado de elementos
        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
        }
        public IActionResult Crear()
        {

            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            /*Validamos que el modelo no venga vacio, aunque existan validaciones de Front, debemos estar seguros
            que los campos no vienen vacios en el Back*/
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();


            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            //Si ya existe el nombre que se desea ingresar

            //En RepositorioTiposCuentas se retorna en el metodo Existe un valor 1 para cuando ya existe el registro
            if (yaExisteTipoCuenta)
            {
                //Agregamos el error
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");

                //Retornamos la vista de tipoCuenta
                return View(tipoCuenta );
            }
            //Si no existe el tipo de cuenta, se crea el registro
            //Accedemos al repositorio
            await repositorioTiposCuentas.Crear(tipoCuenta);

            //Redireccionamos al usuario a la vista Index de TiposCuentas
            return RedirectToAction("Index");
        }

        //Esta será la página que permitira cargar el registro por su Id
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

           //Validamos si el usuario tiene permiso de cargar el registro
           if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

           return View(tipoCuenta);
        }

        //Creamos la accion de editar

        [HttpPost]
        public async Task<ActionResult>Editar(TipoCuenta tipoCuenta)
        {
            //Obtenemos el Id del usuario a través del servicio creado
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            //Validamos que el tipo cuenta exista
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if(tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //Si el tipo de cuenta existe, se ejecuta el metodo actualizar 
            await repositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }

        //Creamos la acción que nos permitira borrar un registro 

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        //Creamos el HttpPost que será el que realizará el borrado

        [HttpPost]

        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }

        //Creamos una nueva acción para realizar una nueva validacion con JavaScript utilizando Remote
        [HttpGet]
        //Esta acción debera enlazarse a una propiedad de TipoCuenta
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);

            //Validamos si el tipo de cuenta ya existe
            if (yaExisteTipoCuenta)
            {
                //Devolvemos un Json en caso de existir
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        //Acción que se ejecutara cuando el usuario arrastre las filas de la tabla TipoCuenta
        [HttpPost]

        public async Task <IActionResult> Ordenar([FromBody] int[] ids)
        {
            return Ok();
        }
    }

}
