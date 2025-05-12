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
    private readonly IWebHostEnvironment environment;

    public ConsultaController(
        IRepositorioConsulta repo,
        IRepositorioTurno repoTurno,
        IRepositorioMascota repoMascota,
        IRepositorioVeterinario repoVeterinario,
        IRepositorioDueno repoDueno,
        IWebHostEnvironment environment
    )
    {
        this.repo = repo;
        this.repoTurno = repoTurno;
        this.repoMascota = repoMascota;
        this.repoVeterinario = repoVeterinario;
        this.repoDueno = repoDueno;
        this.environment = environment;
    }

    public IActionResult Index()
    {
        var consultas = repo.ObtenerTodos();
        foreach (var consulta in consultas)
        {
            consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
            consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        }
        return View(consultas);
    }

    public IActionResult Detalles(int id)
    {
        var consulta = repo.ObtenerPorId(id);
        if (consulta == null)
        {
            return NotFound();
        }
        consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
        consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        return View(consulta);
    }

    public IActionResult Editar(int id)
    {
        var consulta = repo.ObtenerPorId(id);
        if (consulta == null)
        {
            return NotFound();
        }
        consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
        consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        return View(consulta);
    }
    [HttpPost]
    public IActionResult Editar(int id, Consulta consulta, IFormFile ArchivoNuevo)
    {
        if (id != consulta.Id)
        {
            return NotFound();
        }

        ModelState.Remove("Veterinario");
        ModelState.Remove("Mascota");
        if (ModelState.IsValid)
        {
            try
            {
                if (ArchivoNuevo != null && ArchivoNuevo.Length > 0)
                {
                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(ArchivoNuevo.FileName);
                    var ruta = Path.Combine(environment.WebRootPath, "archivos", nombreArchivo);

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        ArchivoNuevo.CopyTo(stream);
                    }

                    consulta.ArchivoAdjunto = nombreArchivo;
                }

                repo.Modificacion(consulta);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar la consulta: " + ex.Message);
            }
        }
        consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
        consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        return View(consulta);
    }

    public IActionResult Crear()
    {
        var veterinarios = repoVeterinario
            .ObtenerTodos()
            .Select(v => new { Id = v.Id, NombreCompleto = v.Nombre + " " + v.Apellido })
            .ToList();
        ViewBag.Veterinarios = new SelectList(veterinarios, "Id", "NombreCompleto");
        var duenos = repoDueno
        .ObtenerTodos()
        .Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.Nombre + " " + d.Apellido
        })
        .ToList();
        ViewBag.Duenos = new SelectList(duenos, "Value", "Text");

        ViewBag.Mascotas = new SelectList(new List<Mascota>(), "Id", "Nombre");

        return View();
    }
    public IActionResult ObtenerMascotasPorDueno(int idDueno)
    {
        var mascotas = repoMascota.ObtenerPorDueno(idDueno);  

        var resultado = mascotas.Select(m => new {
            id = m.Id,
            nombre = m.Nombre
        });

        return Json(resultado);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Consulta consulta, IFormFile? Archivo)
    {
        ModelState.Remove("Id_Dueno");
        ModelState.Remove("Mascota");
        ModelState.Remove("Veterinario");

        if (ModelState.IsValid)
        {
            if (Archivo != null && Archivo.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(Archivo.FileName);
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "archivos");
                var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    Archivo.CopyTo(stream);
                }

                consulta.ArchivoAdjunto = "/archivos/" + nombreArchivo;
            }
            repo.Alta(consulta);

            return RedirectToAction("Index");
        }
        var veterinarios = repoVeterinario
            .ObtenerTodos()
            .Select(v => new { Id = v.Id, NombreCompleto = v.Nombre + " " + v.Apellido })
            .ToList();
        ViewBag.Veterinarios = new SelectList(veterinarios, "Id", "NombreCompleto", consulta.Id_Veterinario);
        ViewBag.Mascotas = new SelectList(repoMascota.ObtenerTodos(), "Id", "Nombre");

        return View(consulta.Id_Turno != null ? "CrearDesdeTurno" : "Crear", consulta);
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
            repoDueno
                .ObtenerTodos()
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nombre + " " + d.Apellido,
                })
                .ToList(),
            "Value",
            "Text"
        );

        return View();
    }

    [HttpPost]
    public IActionResult SeleccionarTurnoExistente(
        int? Id_Dueno,
        int? idMascota,
        int? idTurno,
        string action
    )
    {
        ViewBag.Duenos = new SelectList(
            repoDueno
                .ObtenerTodos()
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nombre + " " + d.Apellido,
                })
                .ToList(),
            "Value",
            "Text",
            Id_Dueno?.ToString()
        );

        if (Id_Dueno.HasValue)
        {
            var mascotas = repoMascota.ObtenerPorDueno(Id_Dueno.Value);
            ViewBag.Mascotas = new SelectList(mascotas, "Id", "Nombre", idMascota);

            if (idMascota.HasValue)
            {
                var turnos = repoTurno
                    .ObtenerPorMascota(idMascota.Value)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    })
                    .ToList();

                ViewBag.Turnos = new SelectList(turnos, "Value", "Text", idTurno?.ToString());

                if (action == "confirmar" && idTurno.HasValue)
                {
                    return RedirectToAction("CrearDesdeTurno", new { idTurno = idTurno.Value });
                }
            }
        }

        return View();
    }

    [HttpGet]
    public IActionResult CrearDesdeTurno(int idTurno)
    {
        var turno = repoTurno.ObtenerPorId(idTurno);
        if (turno == null)
        {
            return NotFound();
        }

        var consulta = new Consulta
        {
            Id_Turno = turno.Id,
            Id_Mascota = turno.Id_Mascota.Value,
            Fecha = turno.Fecha,
            Motivo = turno.Motivo
        };

        var veterinarios = repoVeterinario
            .ObtenerTodos()
            .Select(v => new { Id = v.Id, NombreCompleto = v.Nombre + " " + v.Apellido })
            .ToList();
            ViewBag.Veterinarios = new SelectList(
            veterinarios,
            "Id",
            "NombreCompleto",
            consulta.Id_Veterinario
        );

        return View("CrearDesdeTurno", consulta);
    }
}
