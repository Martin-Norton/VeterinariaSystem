using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace VeterinariaSystem.Models
{
    public class RepositorioTurno : RepositorioBase, IRepositorioTurno
    {
        public RepositorioTurno(IConfiguration configuration) : base(configuration) { }

        public int Alta(Turno turno)
        {
            int res = -1;

            if (ExisteTurnoEnHorario(turno.Fecha, turno.Hora) || ExisteTurnoParaMascotaEnDia(turno.Id_Mascota.Value, turno.Fecha))
                return -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Turno (Id_Mascota, Motivo, Fecha, Hora, Estado)
                               VALUES (@idMascota, @motivo, @fecha, @hora, @estado);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idMascota", turno.Id_Mascota);
                    command.Parameters.AddWithValue("@motivo", turno.Motivo);
                    command.Parameters.AddWithValue("@fecha", turno.Fecha.Date);
                    command.Parameters.AddWithValue("@hora", turno.Hora);
                    command.Parameters.AddWithValue("@estado", turno.Estado);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }

            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Turno SET Estado = 0 WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Turno turno)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Turno SET Id_Mascota = @idMascota, Motivo = @motivo,
                                Fecha = @fecha, Hora = @hora WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", turno.Id);
                    command.Parameters.AddWithValue("@idMascota", turno.Id_Mascota);
                    command.Parameters.AddWithValue("@motivo", turno.Motivo);
                    command.Parameters.AddWithValue("@fecha", turno.Fecha.Date);
                    command.Parameters.AddWithValue("@hora", turno.Hora);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return res;
        }

        public Turno ObtenerPorId(int id)
        {
            Turno turno = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT t.Id, t.Id_Mascota, t.Motivo, t.Fecha, t.Hora, t.Estado,
                        m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno
                    FROM Turno t
                    LEFT JOIN Mascota m ON t.Id_Mascota = m.Id
                    WHERE t.Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            turno = new Turno
                            {
                                Id = reader.GetInt32(0),
                                Id_Mascota = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                                Motivo = reader.GetString(2),
                                Fecha = reader.GetDateTime(3),
                                Hora = reader.GetTimeSpan(4),
                                Estado = reader.GetInt32(5),
                                Mascota = reader.IsDBNull(1) ? null : new Mascota
                                {
                                    Nombre = reader.GetString(6),
                                    Especie = reader.GetString(7),
                                    Raza = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    Edad = reader.GetInt32(9),
                                    Peso = reader.GetInt32(10),
                                    Sexo = reader.GetString(11),
                                    Id_Dueno = reader.GetInt32(12)
                                }
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return turno;
        }

        public IList<Turno> ObtenerTodos()
        {
            IList<Turno> lista = new List<Turno>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT t.Id, t.Id_Mascota, t.Motivo, t.Fecha, t.Hora, t.Estado,
                        m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno
                    FROM Turno t
                    LEFT JOIN Mascota m ON t.Id_Mascota = m.Id
                    WHERE t.Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Turno
                            {
                                Id = reader.GetInt32(0),
                                Id_Mascota = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                                Motivo = reader.GetString(2),
                                Fecha = reader.GetDateTime(3),
                                Hora = reader.GetTimeSpan(4),
                                Estado = reader.GetInt32(5),
                                Mascota = reader.IsDBNull(1) ? null : new Mascota
                                {
                                    Nombre = reader.GetString(6),
                                    Especie = reader.GetString(7),
                                    Raza = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    Edad = reader.GetInt32(9),
                                    Peso = reader.GetInt32(10),
                                    Sexo = reader.GetString(11),
                                    Id_Dueno = reader.GetInt32(12)
                                }
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }


        public IList<Turno> ObtenerPorFecha(DateTime fecha)
        {
            IList<Turno> lista = new List<Turno>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT t.Id, t.Id_Mascota, t.Motivo, t.Fecha, t.Hora, t.Estado,
                        m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno
                    FROM Turno t
                    LEFT JOIN Mascota m ON t.Id_Mascota = m.Id
                    WHERE t.Fecha = @fecha AND t.Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha", fecha.Date);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Turno
                            {
                                Id = reader.GetInt32(0),
                                Id_Mascota = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                                Motivo = reader.GetString(2),
                                Fecha = reader.GetDateTime(3),
                                Hora = reader.GetTimeSpan(4),
                                Estado = reader.GetInt32(5),
                                Mascota = reader.IsDBNull(1) ? null : new Mascota
                                {
                                    Nombre = reader.GetString(6),
                                    Especie = reader.GetString(7),
                                    Raza = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    Edad = reader.GetInt32(9),
                                    Peso = reader.GetInt32(10),
                                    Sexo = reader.GetString(11),
                                    Id_Dueno = reader.GetInt32(12)
                                }
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

         public IList<Turno> ObtenerPorMascota(int idMascota)
        {
            IList<Turno> lista = new List<Turno>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT t.Id, t.Id_Mascota, t.Motivo, t.Fecha, t.Hora, t.Estado,
                        m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno
                    FROM Turno t
                    LEFT JOIN Mascota m ON t.Id_Mascota = m.Id
                    WHERE t.Id_Mascota = @idMascota AND t.Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idMascota", idMascota);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Turno
                            {
                                Id = reader.GetInt32(0),
                                Id_Mascota = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                                Motivo = reader.GetString(2),
                                Fecha = reader.GetDateTime(3),
                                Hora = reader.GetTimeSpan(4),
                                Estado = reader.GetInt32(5),
                                Mascota = reader.IsDBNull(1) ? null : new Mascota
                                {
                                    Nombre = reader.GetString(6),
                                    Especie = reader.GetString(7),
                                    Raza = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    Edad = reader.GetInt32(9),
                                    Peso = reader.GetInt32(10),
                                    Sexo = reader.GetString(11),
                                    Id_Dueno = reader.GetInt32(12)
                                }
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public Turno ObtenerPorMascotaYFecha(int idMascota, DateTime fecha)
        {
            Turno turno = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT t.Id, t.Id_Mascota, t.Motivo, t.Fecha, t.Hora, t.Estado,
                        m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno
                    FROM Turno t
                    LEFT JOIN Mascota m ON t.Id_Mascota = m.Id
                    WHERE t.Id_Mascota = @idMascota AND t.Fecha = @fecha AND t.Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idMascota", idMascota);
                    command.Parameters.AddWithValue("@fecha", fecha.Date);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            turno = new Turno
                            {
                                Id = reader.GetInt32(0),
                                Id_Mascota = reader.GetInt32(1),
                                Motivo = reader.GetString(2),
                                Fecha = reader.GetDateTime(3),
                                Hora = reader.GetTimeSpan(4),
                                Estado = reader.GetInt32(5),
                                Mascota = new Mascota
                                {
                                    Nombre = reader.GetString(6),
                                    Especie = reader.GetString(7),
                                    Raza = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    Edad = reader.GetInt32(9),
                                    Peso = reader.GetInt32(10),
                                    Sexo = reader.GetString(11),
                                    Id_Dueno = reader.GetInt32(12)
                                }
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return turno;
        }
        public bool ExisteTurnoEnHorario(DateTime fecha, TimeSpan hora)
        {
            bool existe = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Turno WHERE Fecha = @fecha AND Hora = @hora AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha", fecha.Date);
                    command.Parameters.AddWithValue("@hora", hora);
                    connection.Open();
                    existe = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    connection.Close();
                }
            }
            return existe;
        }

        public bool ExisteTurnoParaMascotaEnDia(int idMascota, DateTime fecha)
        {
            bool existe = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Turno WHERE Id_Mascota = @idMascota AND Fecha = @fecha AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idMascota", idMascota);
                    command.Parameters.AddWithValue("@fecha", fecha.Date);
                    connection.Open();
                    existe = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    connection.Close();
                }
            }
            return existe;
        }
    }
}
