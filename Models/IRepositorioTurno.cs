using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Models
{
    public interface IRepositorioTurno
    {
        int Alta(Turno turno);
        int Baja(int id);
        int Modificacion(Turno turno);
        Turno ObtenerPorId(int id);
        IList<Turno> ObtenerTodos();
        IList<Turno> ObtenerPorFecha(DateTime fecha);
        IList<Turno> ObtenerPorMascota(int idMascota);
        Turno ObtenerPorMascotaYFecha(int idMascota, DateTime fecha);
        bool ExisteTurnoEnHorario(DateTime fecha, TimeSpan hora);
        bool ExisteTurnoParaMascotaEnDia(int idMascota, DateTime fecha);
    }
}
