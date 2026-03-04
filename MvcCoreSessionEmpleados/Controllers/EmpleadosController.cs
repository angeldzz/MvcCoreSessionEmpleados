using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        private IMemoryCache memoryCache;
        public EmpleadosController(
            RepositoryEmpleados repo, 
            IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
        }
        public IActionResult EmpleadosFavoritos()
        {
            if (this.memoryCache.Get("FAVORITOS") == null)
            {
                ViewData["MENSAJE"] = "No existen empleados favoritos";
                return View();
            }
            else
            {
                List<Empleado> favoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                return View(favoritos);
            }
        }
        public async Task<IActionResult> SessionEmpleadosV5
            (int? idempleado, 
            int? idfavorito)
        {
            if (idfavorito != null)
            {
                //COMO ESTOY ALMACENANDO EN CACHE VAMOS A GUARDAR
                //DIRECATMENTE LOS OBETOS EN LUGAR DE LOS IDS
                List<Empleado> empleadosFavoritos;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    //NO EXISTE NADA EN CACHE
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    //RECUPERAMOS EL CACHE
                    empleadosFavoritos =
                        this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }
                //BUSCAMOS AL EMPLEADO PARA GUARDARLO
                Empleado empleado = await this.repo.FindEmpleadosAsync(idfavorito.Value);
                empleadosFavoritos.Add(empleado);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);
            }
            if (idempleado != null)
            {
                //ALMACENAMOS LO MINIMO...
                List<int> idsEmpleadosList;
                if (HttpContext.Session.GetObject<List<int>>
                    ("IDSEMPLEADOS") != null)
                {
                    //RECUPERAMOS LA COLECCION
                    idsEmpleadosList =
                        HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    //CREAMOS LA COLECCION
                    idsEmpleadosList = new List<int>();
                }
                //ALMACENAMOS EL ID DEL EMPLEADO
                idsEmpleadosList.Add(idempleado.Value);
                //ALMACENAMOS EN SESSION LOS DATOS
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleadosList);
                ViewData["MENSAJE"] = "Empleados almacenados: "
                    + idsEmpleadosList.Count;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosAlmacenadosV5(int? ideliminar)
        {
            //RECUPERAMOS LA COLECCION DE SESSION
            List<int> idsEmpleados =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en Session";
                return View();
            }
            else
            {
                //Preguntamos si hemos recibido algun dato para eliminar
                if (ideliminar != null)
                {
                    idsEmpleados.Remove(ideliminar.Value);
                    //si no tenemos empelados en session nuestra coleccion
                    //existe y se queda en 0
                    //elimnamos session
                    if (idsEmpleados.Count == 0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                    }
                    else
                    {
                    //actualizamos la session
                    HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    ViewData["MENSAJE"] = "Empleado eliminado: " + ideliminar.Value;
                    }
                }
                List<Empleado> empleados =
                    await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }
        public async Task<IActionResult> SessionEmpleadosV4(int? idempleado)
        {
            //RECUPERAMOS LA COLECCION
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                //creamos la coleccion
                idsEmpleados = new List<int>();
            }
            
            if (idempleado != null)
            {
                //Agregamos el id del empleado
                idsEmpleados.Add(idempleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleado almacenado: " + idempleado.Value;
            }

            List<Empleado> empleados = await this.repo.GetEmpleadosAsyncNotIn(idsEmpleados);
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosAlmacenadosV4()
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No hay empleados almacenados en session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }
        public async Task<IActionResult> SessionEmpleadosOk(int? idempleado)
        {
            if (idempleado != null)
            {
                //Almacenamos lo minimo
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    //RECUPERAMOS LA COLECCION
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    //creamos la coleccion
                    idsEmpleados = new List<int>();
                }
                //Agregamos el id del empleado
                idsEmpleados.Add(idempleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleado almacenado: " + idempleado.Value;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosAlmacenadosOk()
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No hay empleados almacenados en session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }
        public async Task<IActionResult> SessionEmpleados(int? idempleado)
        {
            if (idempleado != null)
            {
                Empleado empleado = await this.repo.FindEmpleadosAsync(idempleado.Value);
                //En session tendremos almacenado un conjunto de empleados
                List<Empleado> empleadosList;
                //Debemos preguntar si ya tenemos empleados en session
                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    empleadosList = new List<Empleado>();
                }
                //Agregamos el empleado a list;
                empleadosList.Add(empleado);
                //Almcenamos la lista en session
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = "Empleado almacenado: " + empleado.Apellido;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }
        public async Task<IActionResult> SessionSalarios(int? salario)
        {
            if (salario != null)
            {
                //Queremos almacenar la suma total de salarios
                //Que tengamos en session
                int sumaTotal = 0;
                if (HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    sumaTotal = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                sumaTotal += salario.Value;
                HttpContext.Session.SetObject("SUMASALARIAL", sumaTotal);
                ViewData["MENSAJE"] = "Salario almacenado: " + salario;
            }
            List<Empleado> empleado = await this.repo.GetEmpleadosAsync();
            return View(empleado);
        }
        public IActionResult SumaSalarial()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
