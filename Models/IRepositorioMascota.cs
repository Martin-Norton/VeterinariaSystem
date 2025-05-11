using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Models
{
    public interface IRepositorioMascota
    {
        IList<Mascota> ObtenerTodos();
        Mascota ObtenerPorId(int id);
        IList<Mascota> ObtenerPorDueno(int idDueno);
        int Alta(Mascota mascota);
        int Modificacion(Mascota mascota);
        int Baja(int id);
    }
}