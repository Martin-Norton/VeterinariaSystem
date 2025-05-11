using VeterinariaSystem.Models;

public interface IRepositorioConsulta
{
    Consulta ObtenerPorId(int id);
    IEnumerable<Consulta> ObtenerTodos();
    int Alta(Consulta consulta);
    int Modificacion(Consulta consulta);
    int Baja(int id);
}
