using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using gsNotasNET.Data;
using System.Diagnostics;

namespace gsNotasNET.Models
{
    public class NotaSQL : NotasNETSQLDatabase
    {
        public int ID { get; set; }
        public int idUsuario { get; set; }
        public string Texto { get; set; }
        public DateTime Modificada { get; set; }
        public string Grupo { get; set; }
        public bool Archivada { get; set; } = false;
        public bool Eliminada { get; set; } = false;
        public bool Favorita { get; set; } = false;

        private int LongitudTituloNota = 50;
        /// <summary>
        /// Solo es valor local, no en la base de datos.
        /// </summary>
        public string TituloNota
        {
            get
            {
                if (string.IsNullOrEmpty(Texto))
                    return "";

                var s = Texto.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (s.Length == 0)
                    return Texto;

                if (s[0].Length < LongitudTituloNota)
                    return s[0];

                return s[0].Substring(0, LongitudTituloNota);
            }
        }

        public NotaSQL()
        {
            ID = 0;
            idUsuario = UsuarioSQL.UsuarioLogin.ID; // App.UsuarioLogin.ID;
            Texto = "";
            Modificada = DateTime.UtcNow;
            Grupo = "";
            Archivada = false;
            Eliminada = false;
        }

        /// <summary>
        /// Guarda o actualiza la nota indicada. 
        /// Si el ID no es cero, la actualiza, si es cero la crea.
        /// </summary>
        /// <param name="nota">La nota a guardar.</param>
        /// <returns>
        /// El número de notas afectadas (0 si no se guardó o 1 si se actualizó o creó correctamente).
        /// </returns>
        public static int GuardarNota(NotaSQL nota)
        {
            if (nota is null)
                return 0; // new Task<int>(() => 0);

            if (nota.ID == 0)
            {
                return NotaSQL.Insert(nota);
            }
            else
            {
                return NotaSQL.Update(nota);
            }
        }

        /// <summary>
        /// Actualiza la nota en la tabla Notas.
        /// </summary>
        /// <param name="nota">La nota a actualizar.</param>
        /// <returns>El número de notas afectadas (o cero si no se pudo actualizar).</returns>
        internal static int Update(NotaSQL nota)
        {
            if (nota is null || nota.ID == 0)
                return 0; // new Task<int>(() => 0);

            var msg = Actualizar(nota);

            if (msg.StartsWith("ERROR"))
                return 0; // new Task<int>(() => 0);

            return 1; // new Task<int>(() => 1);
        }

        /// <summary>
        /// Inserta una nueva nota en la tabla Notas.
        /// </summary>
        /// <param name="nota">La nota a añadir.</param>
        /// <returns>El número de notas afectadas (o cero si no se pudo insertar).</returns>
        internal static int Insert(NotaSQL nota)
        {
            if (nota is null)
                return 0; // new Task<int>(() => 0);

            var msg = Crear(nota);

            if (msg.StartsWith("ERROR"))
                return 0; // new Task<int>(() => 0);

            return 1; // new Task<int>(() => 1);
        }

        /// <summary>
        /// Elimina la nota indicada.
        /// En realidad no se elimina, se asigna true a Eliminada.
        /// </summary>
        /// <param name="nota">La nota a eliminar.</param>
        /// <returns>El número de notas afectadas (o cero si no se pudo eliminar).</returns>
        public static int Delete(NotaSQL nota)
        {
            if (nota is null || nota.ID == 0)
                return 0; // new Task<int>(() => 0);

            nota.Eliminada = true;
            var msg = Actualizar(nota);

            //var sel = $"ID = {nota.ID}";
            //var msg = Borrar(sel);

            if (msg.StartsWith("ERROR"))
                return 0; // new Task<int>(() => 0);

            return 1; // new Task<int>(() => 1);
        }

        private static string Actualizar(NotaSQL nota)
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
                    sCommand = $"UPDATE {TablaNotas} SET idUsuario = @idUsuario, Grupo = @Grupo, Texto = @Texto, Modificada = @Modificada, Archivada = @Archivada, Eliminada = @Eliminada, Favorita = @Favorita  WHERE (ID = @ID)";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@ID", nota.ID);
                    cmd.Parameters.AddWithValue("@idUsuario", nota.idUsuario);
                    cmd.Parameters.AddWithValue("@Grupo", nota.Grupo);
                    cmd.Parameters.AddWithValue("@Texto", nota.Texto);
                    cmd.Parameters.AddWithValue("@Modificada", nota.Modificada);
                    cmd.Parameters.AddWithValue("@Archivada", nota.Archivada);
                    cmd.Parameters.AddWithValue("@Eliminada", nota.Eliminada);
                    cmd.Parameters.AddWithValue("@Favorita", nota.Favorita);

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
        private static string Crear(NotaSQL nota)
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
                    sCommand = $"INSERT INTO {TablaNotas} (idUsuario, Grupo, Texto, Modificada, Archivada, Eliminada, Favorita) VALUES(@idUsuario, @Grupo, @Texto, @Modificada, @Archivada, @Eliminada, @Favorita) SELECT @@Identity";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@idUsuario", nota.idUsuario);
                    cmd.Parameters.AddWithValue("@Grupo", nota.Grupo);
                    cmd.Parameters.AddWithValue("@Texto", nota.Texto);
                    cmd.Parameters.AddWithValue("@Modificada", nota.Modificada);
                    cmd.Parameters.AddWithValue("@Archivada", nota.Archivada);
                    cmd.Parameters.AddWithValue("@Eliminada", nota.Eliminada);
                    cmd.Parameters.AddWithValue("@Favorita", nota.Favorita);

                    cmd.Transaction = tran;

                    int id = System.Convert.ToInt32(cmd.ExecuteScalar());
                    nota.ID = id;

                    // Si llega aquí es que todo fue bien,
                    // por tanto, llamamos al método Commit.
                    tran.Commit();

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
        private static string Borrar(string where)
        {
            string msg = "";

            string sCon = CadenaConexion;
            string sel = $"DELETE FROM {TablaNotas} WHERE {where}";

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
                    msg = $"ERROR al eliminar los registros con: {where}. {ex.Message}.";
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        msg = $"ERROR (Rollback) al eliminar los registros con: {where}. {ex2.Message}.";
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
        /// Los grupos que hay asignados, de todas las notas, estén o no archivadas o eliminadas.
        /// </summary>
        /// <returns>Una colección de tipo string</returns>
        public static List<string> Grupos(int idUsuario)
        {
            // todas las notas estén o no archivadas
            var colNotas = NotasUsuario(idUsuario, true);
            var grupos = new List<string>();
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
        /// Devuelve el número de notas activas: no archivadas ni eliminadas
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        internal static int Count(int idUsuario)
        {
            int ret = 0;

            var sel = $"SELECT Count(*) FROM {TablaNotas} " +
                       "WHERE idUsuario = {idUsuario} AND (Eliminada = 0  AND Archivada = 0)";
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var t = (int)cmd.ExecuteScalar();
                ret = t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }

            return ret; // new Task<int>(() => ret);
        }

        /// <summary>
        /// El total de notas archivadas, si no están eliminadas.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        internal static int CountArchivadas(int idUsuario)
        {
            int ret = 0;

            var sel = $"SELECT Count(*) FROM {TablaNotas} " + 
                      $"WHERE idUsuario = {idUsuario} AND Archivada = 1 AND Eliminada = 0 ";
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var t = (int)cmd.ExecuteScalar();
                ret = t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }

            return ret; // new Task<int>(() => ret);
        }

        /// <summary>
        /// El total de notas eliminadas.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        internal static int CountEliminadas(int idUsuario)
        {
            int ret = 0;

            var sel = $"SELECT Count(*) FROM {TablaNotas} " +
                      $"WHERE idUsuario = {idUsuario} AND Eliminada = 1 ";
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var t = (int)cmd.ExecuteScalar();
                ret = t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }

            return ret; // new Task<int>(() => ret);
        }

        /// <summary>
        /// Devuelve el número de notas Favoritas: no archivadas ni eliminadas
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        internal static int CountFavoritas(int idUsuario)
        {
            int ret = 0;

            var sel = $"SELECT Count(*) FROM {TablaNotas} " +
                       "WHERE idUsuario = {idUsuario} AND Favorita = 1 AND (Eliminada = 0  AND Archivada = 0)";
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var t = (int)cmd.ExecuteScalar();
                ret = t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }

            return ret; // new Task<int>(() => ret);
        }


        /// <summary>
        /// Devuelve una lista de todas las notas del usuario indicado que no están archivadas ni eliminadas.
        /// </summary>
        /// <param name="idUsuario">El id del usuario. Si es cero, mostrar todas.</param>
        /// <param name="archivadas">true para mostrar las archivadas. null para no tenerlo en cuenta.</param>
        /// <param name="eliminadas">true para mostrar las eliminadas. null para no tenerlo en cuenta.</param>
        /// <returns>Una colección de tipo HashSet con las notas.</returns>
        public static List<NotaSQL> NotasUsuario(int idUsuario, bool? archivadas = null, bool? eliminadas = null)
        {
            var colNotas = new List<NotaSQL>();

            string sArchivadas, sEliminadas;

            if (archivadas is null)
            {
                sArchivadas = "(Archivada = 1 OR Archivada = 0) ";
            }
            else
            {
                int bitArchivada = archivadas.Value ? 1 : 0;
                sArchivadas = $"(Archivada = {bitArchivada}) ";
            }

            if (eliminadas is null)
            {
                sEliminadas = "(Eliminada = 1 OR Eliminada = 0) ";
            }
            else
            {
                int bitEliminada = eliminadas.Value ? 1 : 0;
                sEliminadas = $"(Eliminada = {bitEliminada}) ";
            }

            var sel = $"SELECT * FROM {TablaNotas} ";
            if(idUsuario !=0)
                sel += $"WHERE idUsuario = {idUsuario} AND "; 
            else
                sel += "WHERE ";
            sel += $"({sArchivadas} AND {sEliminadas}) ";

            sel += "ORDER BY Favorita, Grupo ASC, Modificada DESC, ID";
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    colNotas.Add(AsignarNota(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }

            return colNotas; // new Task<HashSet<NotaSQL>>(() => colNotas);
        }

        /// <summary>
        /// Devuelve las notas del usuario indicado.
        /// </summary>
        /// <param name="idUsuario">El ID delk usuario.</param>
        /// <returns>Una colección del tipo <see cref="NotaSQL"/>.</returns>
        public static List<NotaSQL> NotasFavoritas(int idUsuario)
        {
            var colNotas = new List<NotaSQL>();

            var sel = $"SELECT * FROM {TablaNotas} ";
            sel += $"WHERE idUsuario = {idUsuario} AND Favorita = 1 ";
            sel += "ORDER BY Grupo ASC, Modificada DESC, ID";
            
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    colNotas.Add(AsignarNota(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }

            return colNotas;
        }

        /// <summary>
        /// Buscar notas según el texto indicado.
        /// </summary>
        /// <param name="idUsuario">El id del usuario.</param>
        /// <param name="buscar">El texto a buscar.</param>
        /// <param name="archivada">Si se busca en las archivadas.</param>
        /// <param name="favorita">Si se busca en las favoritas.</param>
        /// <returns>Una colección con las notas halladas.</returns>
        public static List<NotaSQL> NotasBuscar(int idUsuario, string buscar, bool favoritas, bool archivadas)
        {
            var colNotas = new List<NotaSQL>();

            string sArchivadas, sFavoritas;

            if (!archivadas)
            {
                sArchivadas = "(Archivada = 1 OR Archivada = 0) ";
            }
            else
            {
                sArchivadas = "(Archivada = 1) ";
            }

            if (!favoritas)
            {
                sFavoritas = "(Favorita = 1 OR Favorita = 0) ";
            }
            else
            {
                sFavoritas = "(Favorita = 1) ";
            }

            var sel = $"SELECT * FROM {TablaNotas} ";
            sel += $"WHERE idUsuario = {idUsuario} AND Texto like '%{buscar}%' " + 
                   $"AND {sFavoritas} AND {sArchivadas} ";
            sel += "ORDER BY Grupo ASC, Favorita, Modificada DESC, ID";

            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    colNotas.Add(AsignarNota(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }

            return colNotas;
        }


        /// <summary>
        /// Devuelve la nota con el ID indicado.
        /// </summary>
        /// <param name="id">El id de la nota a obtener.</param>
        /// <returns>Un objeto del tipo <see cref="NotaSQL"/> con la nota indicada.</returns>
        /// <remarks>Se busca la nota con ese ID esté o no eliminada o archivada.</remarks>
        internal static NotaSQL Nota(int id)
        {
            var nota = new NotaSQL();

            var sel = $"SELECT * FROM {TablaNotas} " + 
                      $"WHERE ID = {id} " +
                       "ORDER BY ID";
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    nota = AsignarNota(reader);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }

            return nota;
        }

        /// <summary>
        /// Asigna un objeto a partir de los datos del SQLReader.
        /// </summary>
        /// <param name="reader">El SQLReader del que se sacará la información.</param>
        /// <returns>Un objeto del tipo <see cref="UsuarioSQL"/>.</returns>
        private static NotaSQL AsignarNota(SqlDataReader reader)
        {
            var nota = new NotaSQL();

            var id = 0;
            int.TryParse(reader["ID"].ToString(), out id);
            nota.ID = id;
            id = 0;
            int.TryParse(reader["idUsuario"].ToString(), out id);
            nota.idUsuario = id;
            nota.Grupo = reader["Grupo"].ToString().TrimEnd();
            nota.Texto = reader["Texto"].ToString().TrimEnd();
            var fec = DateTime.Now;
            DateTime.TryParse(reader["Modificada"].ToString(), out fec);
            nota.Modificada = fec;
            var valorBool = false;
            bool.TryParse(reader["Archivada"].ToString(), out valorBool);
            nota.Archivada = valorBool;
            valorBool = false;
            bool.TryParse(reader["Eliminada"].ToString(), out valorBool);
            nota.Eliminada = valorBool;
            valorBool = false;
            bool.TryParse(reader["Favorita"].ToString(), out valorBool);
            nota.Favorita = valorBool;

            return nota; // new Task<NotaSQL>(() => nota);
        }
    }
}
