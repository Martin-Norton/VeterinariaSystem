using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers
{
    public class MascotaController : Controller
    {
        private readonly IRepositorioMascota repositorio;
        private readonly IRepositorioDueno repoDueno;
        private readonly IWebHostEnvironment environment;

        public MascotaController(
            IWebHostEnvironment environment,
            IRepositorioMascota repo,
            IRepositorioDueno repoDueno
        )
        {
            this.environment = environment;
            this.repositorio = repo;
            this.repoDueno = repoDueno;
        }

        // GET: /Mascota
        public IActionResult Index()
        {
            var mascotas = repositorio.ObtenerTodos();
            return View("Index", mascotas);
        }

        // GET: /Mascota/Detalles/5
        public IActionResult Detalles(int id)
        {
            var mascota = repositorio.ObtenerPorId(id);
            if (mascota == null)
                return NotFound();

            return View("Detalles", mascota);
        }

        // GET: /Mascota/Crear
        public IActionResult Crear()
        {
            ViewBag.Duenos = repoDueno.ObtenerTodos();
            return View("Crear");
        }

        // POST: /Mascota/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Mascota mascota)
        {
            ModelState.Remove("Dueno");
            if (!ModelState.IsValid)
            {
                ViewBag.Duenos = repoDueno.ObtenerTodos();
                return View("Crear", mascota);
            }
            repositorio.Alta(mascota);
            return RedirectToAction(nameof(Index));
        }

        //GET: /Mascota/Editar/5
        public IActionResult Editar(int id)
        {
            var mascota = repositorio.ObtenerPorId(id);
            if (mascota == null)
                return NotFound();
            return View("Editar", mascota);
        }

        // POST: /Mascota/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Mascota mascota)
        {
            if (id != mascota.Id)
                return NotFound();
            ModelState.Remove("Dueno");
            if (!ModelState.IsValid)
            {
                ViewBag.Duenos = repoDueno.ObtenerTodos();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error en el campo '{state.Key}': {error.ErrorMessage}");
                    }
                }
                return View("Editar", mascota);
            }
            repositorio.Modificacion(mascota);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Mascota/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var mascota = repositorio.ObtenerPorId(id);
            if (mascota == null)
                return NotFound();

            return View("Eliminar", mascota);
        }

        // POST: /Mascota/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        //ZonaBusquedas
        [HttpGet]
        public IActionResult BuscarPorDueno()
        {
            var listaDuenos = repoDueno.ObtenerTodos();
            ViewBag.Duenos = listaDuenos;
            return View();
        }

        [HttpPost]
        public IActionResult BuscarPorDueno(int idDueno)
        {
            var listaDuenos = repoDueno.ObtenerTodos();
            ViewBag.Duenos = listaDuenos;
            var mascotas = repositorio.ObtenerPorDueno(idDueno);
            return View(mascotas);
        }
        //FinZonaBusquedas
    }
}
