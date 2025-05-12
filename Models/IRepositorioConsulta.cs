using VeterinariaSystem.Models;

public interface IRepositorioConsulta
{
    Consulta ObtenerPorId(int id);
    IList<Consulta> ObtenerTodos();
    int Alta(Consulta consulta);
    int Modificacion(Consulta consulta);
    int Baja(int id);
}
