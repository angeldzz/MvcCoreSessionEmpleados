using Microsoft.AspNetCore.Mvc;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> SessionEmpleadosV4(int? idempleado)
        {
            List<int> idsEmpleados = null;
            if (idempleado != null)
            {
                //Almacenamos lo minimo
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
