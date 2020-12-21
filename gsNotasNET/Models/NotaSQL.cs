using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using gsNotasNET.Data;

namespace gsNotasNET.Models
{
    public class NotaSQL : NotasNETSQLDatabase
    {
        public int ID { get; set; }
        public int idUsuario { get; set; }
        public int idPrograma { get; set; }
        public string Texto { get; set; }
        public DateTime Modificada { get; set; }
        public string Grupo { get; set; }
        public bool Archivada { get; set; } = false;

        public NotaSQL()
        {
            ID = 0;
            idUsuario = App.UsuarioLogin.ID;
            idPrograma = 0;
            Texto = "";
            Modificada = DateTime.UtcNow;
            Grupo = "";
            Archivada = false;
        }

        /// <summary>
        /// Guarda o actualiza la nota indicada. 
        /// Si el ID no es cero, la actualiza, si es cero la crea.
        /// </summary>
        /// <param name="nota">La nota a guardar.</param>
        /// <returns>
        /// El número de notas afectadas (0 si no se guardó o 1 si se actualizó o creó correctamente).
        /// </returns>
        public static Task<int> GuardarNotaAsync(NotaSQL nota)
        {
            if(nota is null)
                return new Task<int>(() => 0);

            if (nota.ID == 0)
            {
                return InsertAsync(nota);
            }
            else
            {
                return UpdateAsync(nota);
            }
        }

        /// <summary>
        /// Borrar la nota indicada.
        /// Si tiene un ID = 0, no se hace nada.
        /// </summary>
        /// <param name="nota">La nota a eliminar.</param>
        /// <returns>
        /// El número de notas afectadas (0 si no se eliminó o 1 si se eliminó ).
        /// </returns>
        public static Task<int> BorrarNotaAsync(NotaSQL nota)
        {
            if(nota is null || nota.ID == 0)
                return new Task<int>(() => 0);

            return DeleteAsync(nota);
        }

        /// <summary>
        /// Actualiza la nota en la tabla Notas.
        /// </summary>
        /// <param name="nota">La nota a actualizar.</param>
        /// <returns>El número de notas afectadas (o cero si no se pudo actualizar).</returns>
        public static Task<int> UpdateAsync(NotaSQL nota)
        {
            var msg = Actualizar(nota);

            if (msg.StartsWith("ERROR"))
                return new Task<int>(() => 0);

            return new Task<int>(() => 1);
        }

        /// <summary>
        /// Inserta una nueva nota en la tabla Notas.
        /// </summary>
        /// <param name="nota">La nota a añadir.</param>
        /// <returns>El número de notas afectadas (o cero si no se pudo insertar).</returns>
        public static Task<int> InsertAsync(NotaSQL nota)
        {
            var msg = Crear(nota);

            if (msg.StartsWith("ERROR"))
                return new Task<int>(() => 0);

            return new Task<int>(() => 1);
        }

        /// <summary>
        /// Elimina la nota con el ID indicado.
        /// </summary>
        /// <param name="nota">La nota a eliminar.</param>
        /// <returns>El número de notas afectadas (o cero si no se pudo eliminar).</returns>
        public static Task<int> DeleteAsync(NotaSQL nota)
        {
            var sel = $"ID = {nota.ID}";
            var msg = Borrar(sel);
            if (msg.StartsWith("ERROR"))
                return new Task<int>(() => 0);
            
            return new Task<int>(() => 1);
        }

        public static string Actualizar(NotaSQL nota)
        {
            // Actualiza los datos indicados
            // El parámetro, que es una cadena de selección, indicará el criterio de actualización

            string msg;

            using (SqlConnection con = new SqlConnection(CadenaConexion))
            {
                SqlTransaction tran = null;
                try
                {
                    con.Open();
                    tran = con.BeginTransaction();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    string sCommand;
                    sCommand = "UPDATE GuilleDB.Notas SET idUsuario = @idUsuario, idPrograma = @idPrograma, Grupo = @Grupo, Texto = @Texto, Modificada = @Modificada, Archivada = @Archivada  WHERE (ID = @ID)";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@ID", nota.ID);
                    cmd.Parameters.AddWithValue("@idUsuario", nota.idUsuario);
                    cmd.Parameters.AddWithValue("@idPrograma", nota.idPrograma);
                    cmd.Parameters.AddWithValue("@Grupo", nota.Grupo);
                    cmd.Parameters.AddWithValue("@Texto", nota.Texto);
                    cmd.Parameters.AddWithValue("@Modificada", nota.Modificada);
                    cmd.Parameters.AddWithValue("@Archivada", nota.Archivada);

                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();

                    // Si llega aquí es que todo fue bien,
                    // por tanto, llamamos al método Commit.
                    tran.Commit();

                    msg = "Se ha actualizado una Nota correctamente.";
                }
                catch (Exception ex)
                {
                    msg = $"ERROR: {ex.Message}";
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        msg = $"ERROR: {ex2.Message}";
                    }
                }

                finally
                {
                    if (!(con is null))
                        con.Close();
                }
            }
            return msg;
        }

        /// <sumary>
        /// Crear un nuevo registro
        /// En caso de error, devolverá la cadena de error empezando por ERROR:.
        /// </sumary>
        public static string Crear(NotaSQL nota)
        {
            string msg;

            using (SqlConnection con = new SqlConnection(CadenaConexion))
            {
                SqlTransaction tran = null;

                try
                {
                    con.Open();
                    tran = con.BeginTransaction();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    string sCommand;
                    sCommand = "INSERT INTO GuilleDB.Notas (idUsuario, idPrograma, Grupo, Texto, Modificada, Archivada)  VALUES(@idUsuario, @idPrograma, @Grupo, @Texto, @Modificada, @Archivada) SELECT @@Identity";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@idUsuario", nota.idUsuario);
                    cmd.Parameters.AddWithValue("@idPrograma", nota.idPrograma);
                    cmd.Parameters.AddWithValue("@Grupo", nota.Grupo);
                    cmd.Parameters.AddWithValue("@Texto", nota.Texto);
                    cmd.Parameters.AddWithValue("@Modificada", nota.Modificada);
                    cmd.Parameters.AddWithValue("@Archivada", nota.Archivada);

                    cmd.Transaction = tran;

                    int id = System.Convert.ToInt32(cmd.ExecuteScalar());
                    nota.ID = id;

                    msg = "Se ha creado un Notas correctamente.";
                }
                catch (Exception ex)
                {
                    msg = $"ERROR: {ex.Message}";
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        msg = $"ERROR: {ex2.Message}";
                    }
                }

                finally
                {
                    if (!(con is null))
                        con.Close();
                }
            }
            return msg;
        }

        /// <sumary>
        /// Borrar el registro o los registros indicados en la cadena WHERE.
        /// La cadena indicada se usará después de la cláusula WHERE de TSQL.
        /// </sumary>
        public static string Borrar(string where)
        {
            string msg = "";

            string sCon = CadenaConexion;
            string sel = "DELETE FROM GuilleDB.Notas WHERE " + where;

            using (SqlConnection con = new SqlConnection(sCon))
            {
                SqlTransaction tran = null;

                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.CommandText = sel;

                    con.Open();
                    tran = con.BeginTransaction();
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();

                    msg = $"Eliminado correctamente los registros con: {where}.";
                }
                catch (Exception ex)
                {
                    msg = $"ERROR al eliminar los registros con : {where}. {ex.Message}.";
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        msg = $"ERROR (Rollback) al eliminar los registros con : {where}. {ex2.Message}.";
                    }
                }
                finally
                {
                    if (!(con is null))
                        con.Close();
                }
            }
            return msg;
        }

        /// <summary>
        /// Los grupos que hay asignados
        /// </summary>
        /// <returns>Una colección de tipo string</returns>
        public static HashSet<string> Grupos()
        {
            var colNotas = NotasUsuarioAsync(App.UsuarioLogin.ID).Result;
            var grupos = new HashSet<string>();
            foreach (var s in colNotas)
            {
                if (!grupos.Contains(s.Grupo))
                {
                    grupos.Add(s.Grupo);
                }
            }
            return grupos;
        }

        /// <summary>
        /// El número de elementos en la tabla.
        /// </summary>
        /// <returns></returns>
        public static Task<int> CountAsync()
        {
            //return _database.Table<Nota>().CountAsync();

            int ret = 0;

            var sel = $"SELECT Count(*) FROM {TablaNotas} ";
            try
            {
                var con = new SqlConnection(CadenaConexion);
                var cmd = new SqlCommand(sel, con);

                var t = cmd.ExecuteNonQuery();
                ret = t;
                //return t;
            }
            catch { }

            return new Task<int>(() => ret);
        }

        ///// <summary>
        ///// Devuelve una lista de todas las notas que no están archivadas.
        ///// </summary>
        ///// <returns>Una colección de tipo HashSet con las notas.</returns>
        //public static Task<HashSet<NotaSQL>> NotasAsync()
        //{
        //    var colNotas = new HashSet<NotaSQL>();

        //    var sel = $"SELECT * FROM {TablaNotas} WHERE Archivada = 0 " +
        //               "ORDER BY Grupo, Modificada, ID";
        //    try
        //    {
        //        var con = new SqlConnection(CadenaConexion);
        //        var cmd = new SqlCommand(sel, con);

        //        var reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            colNotas.Add(AsignarNota(reader));
        //        }
        //    }
        //    catch { }

        //    return new Task<HashSet<NotaSQL>>(() => colNotas);
        //}

        /// <summary>
        /// Devuelve una lista de todas las notas del usuario  indicado que no están archivadas.
        /// </summary>
        /// <param name="idUsuario">El id del usuario.</param>
        /// <returns>Una colección de tipo HashSet con las notas.</returns>
        public static Task<HashSet<NotaSQL>> NotasUsuarioAsync(int idUsuario)
        {
            var colNotas = new HashSet<NotaSQL>();

            var sel = $"SELECT * FROM {TablaNotas} " + 
                      $"WHERE idUsuario = {idUsuario} AND Archivada = 0 " + 
                      "ORDER BY Grupo, Modificada, ID";
            try
            {
                var con = new SqlConnection(CadenaConexion);
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    colNotas.Add(AsignarNotaAsync(reader).Result);
                }
            }
            catch { }

            return new Task<HashSet<NotaSQL>>(() => colNotas);
        }

        /// <summary>
        /// Devuelve la nota con el ID indicado.
        /// </summary>
        /// <param name="id">El id de la nota a obtener.</param>
        /// <returns>Un objeto del tipo <see cref="NotaSQL"/> con la nota indicada.</returns>
        public static Task<NotaSQL> NotaAsync(int id)
        {
            var nota = new NotaSQL();

            var sel = $"SELECT * FROM {TablaNotas} " + 
                      $"WHERE ID = {id} AND Archivada = 0 " +
                       "ORDER BY ID";
            try
            {
                var con = new SqlConnection(CadenaConexion);
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    nota = AsignarNotaAsync(reader).Result;
                }
            }
            catch { }

            return new Task<NotaSQL>(() => nota);
        }

        /// <summary>
        /// Asigna un objeto a partir de los datos del SQLReader.
        /// </summary>
        /// <param name="reader">El SQLReader del que se sacará la información.</param>
        /// <returns>Un objeto del tipo <see cref="UsuarioSQL"/>.</returns>
        private static Task<NotaSQL> AsignarNotaAsync(SqlDataReader reader)
        {
            var nota = new NotaSQL();

            var id = 0;
            int.TryParse(reader["ID"].ToString(), out id);
            nota.ID = id;
            id = 0;
            int.TryParse(reader["idUsuario"].ToString(), out id);
            nota.idUsuario = id;
            id = 0;
            int.TryParse(reader["idPrograma"].ToString(), out id);
            nota.idPrograma = id;
            nota.Grupo = reader["Grupo"].ToString();
            nota.Texto = reader["Texto"].ToString();
            var fec = DateTime.Now;
            DateTime.TryParse(reader["Modificada"].ToString(), out fec);
            nota.Modificada = fec;
            var archivada = false;
            bool.TryParse(reader["Archivada"].ToString(), out archivada);
            nota.Archivada = archivada;

            return new Task<NotaSQL>(() => nota);
        }
    }
}
