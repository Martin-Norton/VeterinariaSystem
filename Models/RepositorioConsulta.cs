using MySql.Data.MySqlClient;
using VeterinariaSystem.Models;

public class RepositorioConsulta : RepositorioBase, IRepositorioConsulta
{
    public RepositorioConsulta(IConfiguration configuration) : base(configuration) { }
    
    public int Alta(Consulta consulta)
    {
        int id = 0;
        using var connection = new MySqlConnection(connectionString);
        var sql = @"INSERT INTO Consulta (Id_Turno, Fecha, Motivo, Diagnostico, Tratamiento, ArchivoAdjunto, Id_Mascota, Id_Veterinario)
                    VALUES (@id_turno, @fecha, @motivo, @diagnostico, @tratamiento, @archivo, @id_mascota, @id_veterinario);
                    SELECT LAST_INSERT_ID();";
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id_turno", consulta.Id_Turno);
        command.Parameters.AddWithValue("@fecha", consulta.Fecha);
        command.Parameters.AddWithValue("@motivo", consulta.Motivo);
        command.Parameters.AddWithValue("@diagnostico", consulta.Diagnostico ?? "");
        command.Parameters.AddWithValue("@tratamiento", consulta.Tratamiento ?? "");
        command.Parameters.AddWithValue("@archivo", consulta.ArchivoAdjunto ?? "");
        command.Parameters.AddWithValue("@id_mascota", consulta.Id_Mascota);
        command.Parameters.AddWithValue("@id_veterinario", consulta.Id_Veterinario);
        connection.Open();
        id = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        return id;
    }

    public int Baja(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        var sql = "DELETE FROM Consulta WHERE Id = @id";
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public Consulta ObtenerPorId(int id)
    {
        Consulta consulta = null;
        using var connection = new MySqlConnection(connectionString);
        var sql = @"SELECT Id, Id_Turno, Fecha, Motivo, Diagnostico, Tratamiento, ArchivoAdjunto, Id_Mascota, Id_Veterinario 
                    FROM Consulta WHERE Id = @id";
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            consulta = new Consulta
            {
                Id = reader.GetInt32(0),
                Id_Turno = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                Fecha = reader.GetDateTime(2),
                Motivo = reader.GetString(3),
                Diagnostico = reader.IsDBNull(4) ? null : reader.GetString(4),
                Tratamiento = reader.IsDBNull(5) ? null : reader.GetString(5),
                ArchivoAdjunto = reader.IsDBNull(6) ? null : reader.GetString(6),
                Id_Mascota = reader.GetInt32(7),
                Id_Veterinario = reader.GetInt32(8)
            };
        }
        connection.Close();
        return consulta;
    }

    public IList<Consulta> ObtenerTodos()
    {
        var lista = new List<Consulta>();
        using var connection = new MySqlConnection(connectionString);
        var sql = "SELECT Id, Id_Turno, Fecha, Motivo, Diagnostico, Tratamiento, ArchivoAdjunto, Id_Mascota, Id_Veterinario FROM Consulta";
        using var command = new MySqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Consulta
            {
                Id = reader.GetInt32(0),
                Id_Turno = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                Fecha = reader.GetDateTime(2),
                Motivo = reader.GetString(3),
                Diagnostico = reader.IsDBNull(4) ? null : reader.GetString(4),
                Tratamiento = reader.IsDBNull(5) ? null : reader.GetString(5),
                ArchivoAdjunto = reader.IsDBNull(6) ? null : reader.GetString(6),
                Id_Mascota = reader.GetInt32(7),
                Id_Veterinario = reader.GetInt32(8)
            });
        }
        connection.Close();
        return lista;
    }

    public int Modificacion(Consulta consulta)
    {
        using var connection = new MySqlConnection(connectionString);
        var sql = @"UPDATE Consulta SET Id_Turno=@id_turno, Fecha=@fecha, Motivo=@motivo, Diagnostico=@diagnostico,
                    Tratamiento=@tratamiento, ArchivoAdjunto=@archivo, Id_Mascota=@id_mascota, Id_Veterinario=@id_veterinario 
                    WHERE Id=@id";
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", consulta.Id);
        command.Parameters.AddWithValue("@id_turno", consulta.Id_Turno);
        command.Parameters.AddWithValue("@fecha", consulta.Fecha);
        command.Parameters.AddWithValue("@motivo", consulta.Motivo);
        command.Parameters.AddWithValue("@diagnostico", consulta.Diagnostico ?? "");
        command.Parameters.AddWithValue("@tratamiento", consulta.Tratamiento ?? "");
        command.Parameters.AddWithValue("@archivo", consulta.ArchivoAdjunto ?? "");
        command.Parameters.AddWithValue("@id_mascota", consulta.Id_Mascota);
        command.Parameters.AddWithValue("@id_veterinario", consulta.Id_Veterinario);
        connection.Open();
        return command.ExecuteNonQuery();
    }
}
