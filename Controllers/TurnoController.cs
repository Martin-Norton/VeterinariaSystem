using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VeterinariaSystem.Controllers
{
    public class TurnoController : Controller
    {
        private readonly IRepositorioTurno repoTurno;
        private readonly IRepositorioMascota repoMascota;

        public TurnoController(IRepositorioTurno repoTurno, IRepositorioMascota repoMascota)
        {
            this.repoTurno = repoTurno;
            this.repoMascota = repoMascota;
        }

        public IActionResult Index(int pagina = 1)
        {
            int tamaño = 5;
            var turnos = repoTurno.ObtenerPaginadas(pagina, tamaño);
            int total = repoTurno.ObtenerCantidad();
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View("Index", turnos);
        }

        // GET: /Turno/Detalles/5
        public IActionResult Detalles(int id)
        {
            var turno = repoTurno.ObtenerPorId(id);
            if (turno == null) return NotFound();
            return View("Detalles", turno);
        }

        // GET: /Turno/Crear
        public IActionResult Crear()
        {
            //con Login se llena el viewbag con las mascotas del dueño logueado
            //ViewBag.Mascotas = repoMascota.ObtenerPorDueno(Id_Dueno);
            ViewBag.Mascotas = new SelectList(repoMascota.ObtenerTodos(), "Id", "Nombre");

            return View("Crear");
        }

        // POST: /Turno/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Turno turno)
        {
            ModelState.Remove("Mascota");
            if (!ModelState.IsValid)
            {
                //con Login
                //ViewBag.Mascotas = repoMascota.ObtenerPorDueno(Id_Dueno);
                ViewBag.Mascotas = new SelectList(repoMascota.ObtenerTodos(), "Id", "Nombre");
                return View("Crear", turno);
            }

            if (repoTurno.ExisteTurnoEnHorario(turno.Fecha, turno.Hora))
            {
                TempData["Error"] = "Turno ya ocupado, seleccione otro porfavor!";
                ModelState.AddModelError("", "Ya existe un turno en ese horario.");
            }
            else if (repoTurno.ExisteTurnoParaMascotaEnDia(turno.Id_Mascota.Value, turno.Fecha))
            {
                ModelState.AddModelError("", "La mascota ya tiene un turno ese día.");
                TempData["Error"] = "Ya existe un turno para esa mascota ese día.";

            }
            else
            {
                repoTurno.Alta(turno);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Mascotas = new SelectList(repoMascota.ObtenerTodos(), "Id", "Nombre");
            return View("Crear", turno);
        }

        // GET: /Turno/Editar/5
        public IActionResult Editar(int id)
        {
            var turno = repoTurno.ObtenerPorId(id);
            if (turno == null) return NotFound();

            var mascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value);
            ViewBag.NombreMascota = mascota?.Nombre;
            ViewBag.NombreMascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value)?.Nombre;

            return View("Editar", turno);
        }

        // POST: /Turno/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Turno turno)
        {
            if (id != turno.Id) return NotFound();
            ModelState.Remove("Mascota");
            if (!ModelState.IsValid)
            {
                var mascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value);
                ViewBag.NombreMascota = mascota?.Nombre;
                ViewBag.NombreMascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value)?.Nombre;
                return View("Editar", turno);
            }

            var turnoExistente = repoTurno.ObtenerPorId(id);
            if (turnoExistente == null) return NotFound();

            if (turno.Fecha != turnoExistente.Fecha || turno.Hora != turnoExistente.Hora)
            {
                if (repoTurno.ExisteTurnoEnHorario(turno.Fecha, turno.Hora))
                    ModelState.AddModelError("", "Ya existe un turno en ese horario.");
            }

            if (turno.Fecha != turnoExistente.Fecha || turno.Id_Mascota != turnoExistente.Id_Mascota)
            {
                if (repoTurno.ExisteTurnoParaMascotaEnDia(turno.Id_Mascota.Value, turno.Fecha))
                    ModelState.AddModelError("", "La mascota ya tiene un turno ese día.");
            }

            ModelState.Remove("Mascota");
            if (!ModelState.IsValid)
            {
                var mascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value);
                ViewBag.NombreMascota = mascota?.Nombre;
                ViewBag.NombreMascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value)?.Nombre;
                return View("Editar", turno);
            }

            repoTurno.Modificacion(turno);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Turno/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var turno = repoTurno.ObtenerPorId(id);
            if (turno == null) return NotFound();
            return View("Eliminar", turno);
        }

        // POST: /Turno/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            repoTurno.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    //Zona Busquedas
    //     // GET: /Turno/BuscarPorFecha
    //     public IActionResult BuscarPorFecha(DateTime? fecha)
    //     {
    //         if (fecha == null)
    //         {
    //             ViewBag.Mensaje = "Debe seleccionar una fecha.";
    //             return View("BuscarPorFecha", new List<Turno>());
    //         }

    //         var lista = repoTurno.ObtenerPorFecha(fecha.Value);
    //         ViewBag.Mensaje = $"Mostrando turnos del día {fecha.Value:dd/MM/yyyy}";
    //         return View("BuscarPorFecha", lista);
    //     }
    //     // GET: /Turno/BuscarPorMascota
    //     public IActionResult BuscarPorMascota(int? idMascota)
    //     {
    //         var mascotas = repoMascota.ObtenerTodos();
    //         ViewBag.Mascotas = mascotas.Select(m => new SelectListItem
    //         {
    //             Value = m.Id.ToString(),
    //             Text = m.Nombre
    //         }).ToList();


    //         if (idMascota == null)
    //             return View("BuscarPorMascota", new List<Turno>());

    //         var turno = repoTurno.ObtenerPorMascotaYFecha(idMascota.Value, DateTime.Today);
    //         var lista = new List<Turno>();
    //         if (turno != null)
    //         {
    //             lista.Add(turno);
    //             ViewBag.Mensaje = "Turno encontrado para hoy.";
    //         }
    //         else
    //         {
    //             ViewBag.Mensaje = "No hay turnos para esta mascota hoy.";
    //         }

    //         return View("BuscarPorMascota", lista);
    //     }

    //    // GET: /Turno/ObtenerPorMascota
    //     public IActionResult ObtenerPorMascota(int? idMascota)
    //     {
    //         var mascotas = repoMascota.ObtenerTodos();
    //         ViewBag.Mascotas = mascotas.Select(m => new SelectListItem
    //         {
    //             Value = m.Id.ToString(),
    //             Text = m.Nombre
    //         }).ToList();

    //         if (idMascota == null)
    //             return View("TodosLosTurnosPorMascota", new List<Turno>());

    //         var lista = repoTurno.ObtenerPorMascota(idMascota.Value);

    //         if (lista != null && lista.Any())
    //         {
    //             ViewBag.Mensaje = "Turnos encontrados.";
    //         }
    //         else
    //         {
    //             ViewBag.Mensaje = "No hay turnos para esta mascota.";
    //             lista = new List<Turno>();
    //         }

    //         return View("TodosLosTurnosPorMascota", lista);
    //     }
        // GET: /Turno/BuscarPorFecha
        public IActionResult BuscarPorFecha(DateTime? fecha, int pagina = 1)
        {
            int tamaño = 5;

            if (fecha == null)
            {
                ViewBag.Mensaje = "Debe seleccionar una fecha.";
                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = 0;
                return View("BuscarPorFecha", new List<Turno>());
            }

            var turnos = repoTurno.ObtenerPorFechaPaginado(fecha.Value, pagina, tamaño); // Necesitas este método en tu repo
            int total = repoTurno.ObtenerCantidadPorFecha(fecha.Value); // Necesitas este método en tu repo

            ViewBag.Mensaje = $"Mostrando turnos del día {fecha.Value:dd/MM/yyyy}";
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View("BuscarPorFecha", turnos);
        }

        // GET: /Turno/BuscarPorMascota
        public IActionResult BuscarPorMascota(int? idMascota, int pagina = 1)
        {
            int tamaño = 5;
            var mascotas = repoMascota.ObtenerTodos();
            ViewBag.Mascotas = mascotas.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre
            }).ToList();

            if (idMascota == null)
            {
                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = 0;
                return View("BuscarPorMascota", new List<Turno>());
            }

            var turnos = repoTurno.ObtenerPorMascotaPaginado(idMascota.Value, pagina, tamaño); // Necesitas este método en tu repo
            int total = repoTurno.ObtenerCantidadPorMascota(idMascota.Value); // Necesitas este método en tu repo

            if (turnos != null && turnos.Any())
            {
                ViewBag.Mensaje = $"Mostrando turnos para la mascota con ID {idMascota.Value}.";
            }
            else
            {
                ViewBag.Mensaje = $"No hay turnos para la mascota con ID {idMascota.Value}.";
            }

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View("BuscarPorMascota", turnos);
        }

        // GET: /Turno/ObtenerPorMascota
        public IActionResult ObtenerPorMascota(int? idMascota, int pagina = 1)
        {
            int tamaño = 5;
            var mascotas = repoMascota.ObtenerTodos();
            ViewBag.Mascotas = mascotas.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre
            }).ToList();

            if (idMascota == null)
            {
                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = 0;
                return View("TodosLosTurnosPorMascota", new List<Turno>());
            }

            var turnos = repoTurno.ObtenerPorMascotaPaginado(idMascota.Value, pagina, tamaño); // Necesitas este método en tu repo
            int total = repoTurno.ObtenerCantidadPorMascota(idMascota.Value); // Necesitas este método en tu repo

            if (turnos != null && turnos.Any())
            {
                ViewBag.Mensaje = $"Mostrando todos los turnos para la mascota con ID {idMascota.Value}.";
            }
            else
            {
                ViewBag.Mensaje = $"No hay turnos para la mascota con ID {idMascota.Value}.";
                turnos = new List<Turno>();
            }

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View("TodosLosTurnosPorMascota", turnos);
        }
    //Fin Zona Busquedas

    }
}
