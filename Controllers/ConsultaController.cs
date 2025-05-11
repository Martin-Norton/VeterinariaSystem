using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VeterinariaSystem.Models;

public class ConsultaController : Controller
{
    private readonly IRepositorioConsulta repo;
    private readonly IRepositorioTurno repoTurno;
    private readonly IRepositorioMascota repoMascota;
    private readonly IRepositorioVeterinario repoVeterinario;
    private readonly IRepositorioDueno repoDueno;

    public ConsultaController(
        IRepositorioConsulta repo,
        IRepositorioTurno repoTurno,
        IRepositorioMascota repoMascota,
        IRepositorioVeterinario repoVeterinario,
        IRepositorioDueno repoDueno
    )
    {
        this.repo = repo;
        this.repoTurno = repoTurno;
        this.repoMascota = repoMascota;
        this.repoVeterinario = repoVeterinario;
        this.repoDueno = repoDueno;
    }

    public IActionResult Crear()
    {
        var veterinarios = repoVeterinario
            .ObtenerTodos()
            .Select(v => new { Id = v.Id, NombreCompleto = v.Nombre + " " + v.Apellido })
            .ToList();
        ViewBag.Veterinarios = new SelectList(veterinarios, "Id", "NombreCompleto");

        ViewBag.Mascotas = new SelectList(repoMascota.ObtenerTodos(), "Id", "Nombre");

        return View();
    }

    [HttpPost]
    public IActionResult Crear(Consulta consulta)
    {
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"Error en el campo '{state.Key}': {error.ErrorMessage}");
            }
        }
        ModelState.Remove("Mascota");
        ModelState.Remove("Veterinario");
        if (!ModelState.IsValid)
        {
            ViewBag.Mascotas = new SelectList(repoMascota.ObtenerTodos(), "Id", "Nombre");
            var veterinarios = repoVeterinario
                .ObtenerTodos()
                .Select(v => new { Id = v.Id, NombreCompleto = v.Nombre + " " + v.Apellido })
                .ToList();
            ViewBag.Veterinarios = new SelectList(veterinarios, "Id", "NombreCompleto");
            return View(consulta);
        }

        repo.Alta(consulta);
        return RedirectToAction("Index");
    }
    public IActionResult SeleccionarTurno()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SeleccionarTurno(bool crearDesdeCero)
    {
        if (crearDesdeCero)
        {
            return RedirectToAction("Crear");
        }
        else
        {
            return RedirectToAction("SeleccionarTurnoExistente");
        }
    }
    [HttpGet]
    public IActionResult SeleccionarTurnoExistente()
    {
        ViewBag.Duenos = new SelectList(
            repoDueno.ObtenerTodos().Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Nombre + " " + d.Apellido
            }).ToList(), "Value", "Text");

        return View();
    }

    [HttpPost]
    public IActionResult SeleccionarTurnoExistente(int? Id_Dueno, int? idMascota, int? idTurno)
    {
        ViewBag.Duenos = new SelectList(
            repoDueno.ObtenerTodos().Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Nombre + " " + d.Apellido
            }).ToList(), "Value", "Text", Id_Dueno?.ToString());

        if (Id_Dueno.HasValue)
        {
            var mascotas = repoMascota.ObtenerPorDueno(Id_Dueno.Value);
            ViewBag.Mascotas = new SelectList(mascotas, "Id", "Nombre", idMascota);

            if (idMascota.HasValue)
            {
                var turnos = repoTurno.ObtenerPorMascota(idMascota.Value)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Fecha.ToString("dd/MM/yyyy HH:mm")
                    }).ToList();

                ViewBag.Turnos = new SelectList(turnos, "Value", "Text", idTurno?.ToString());
            }
        }

        return View();
    }


}
