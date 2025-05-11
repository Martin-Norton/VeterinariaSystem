using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers
{
    public class DuenoController : Controller
    {
        private readonly IRepositorioDueno repositorio;
        private readonly IWebHostEnvironment environment;

        public DuenoController(IWebHostEnvironment environment, IRepositorioDueno repo)
        {
            this.environment = environment;
            this.repositorio = repo;
        }

        // GET: /Dueno/
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: /Dueno/Detalles/5
        public IActionResult Detalles(int id)
        {
            var dueno = repositorio.ObtenerPorId(id);
            if (dueno == null) return NotFound();
            return View(dueno);
        }

        // GET: /Dueno/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Dueno/Crear
        [HttpPost]
        public IActionResult Crear(Dueno dueno)
        {
            if (ModelState.IsValid)
            {
                var existente = repositorio.ObtenerPorDni(dueno.DNI);
                if (existente != null)
                {
                    ModelState.AddModelError("DNI", "Ya existe un dueño con ese DNI.");
                    return View(dueno);
                }
                repositorio.Alta(dueno);
                return RedirectToAction(nameof(Index));
            }
            return View(dueno);
        }

        // GET: /Dueno/Editar/5
        public IActionResult Editar(int id)
        {
            var dueno = repositorio.ObtenerPorId(id);
            if (dueno == null) return NotFound();
            return View(dueno);
        }

        // POST: /Dueno/Editar/5
        [HttpPost]
         
        public IActionResult Editar(int id, Dueno dueno)
        {
            if (id != dueno.Id) return NotFound();
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(dueno);
                return RedirectToAction(nameof(Index));
            }
            return View(dueno);
        }

        // GET: /Dueno/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var dueno = repositorio.ObtenerPorId(id);
            if (dueno == null) return NotFound();
            return View(dueno);
        }

        // POST: /Dueno/Eliminar/5
        [HttpPost]
         
        public IActionResult EliminarConfirmado(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
