//-----------------------------------------------------------------------------
// NotaSQL                                                          (22/Dic/20)
// Clase para las notas remotas.
//
// (c) Guillermo (elGuille) Som, 2020-2021
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using gsNotasNET.Data;
using System.Diagnostics;

using static gsNotasNET.APIs.Extensiones;

namespace gsNotasNET.Models
{
    public class NotaSQL : NotasNETSQLDatabase
    {
        public virtual int ID { get; set; }

        public int idUsuario { get; set; }
        public string Texto { get; set; }
        public DateTime Modificada { get; set; }
        public string Grupo { get; set; }
        public bool Archivada { get; set; } = false;
        public bool Eliminada { get; set; } = false;
        public bool Favorita { get; set; } = false;
        public bool Sincronizada { get; set; } = false;
        public bool Notificar { get; set; } = false;

        /// <summary>
        /// Este ID se usará para asociar el ID de la nota local con la remota y viceversa.
        /// </summary>
        /// <remarks>
        /// En la base local este ID indicará el ID de la nota remota.
        /// En la base remota este ID indicará el ID de la nota local.
        /// </remarks>
        public int idNota { get; set; }

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

        /// <summary>
        /// Copiar esta nota de tipo <see cref="NotaSQL"/> a una del tipo <see cref="Nota"/>.
        /// </summary>
        /// <returns>Una copia del tipo <see cref="Nota"/>.</returns>
        public Nota ComoNotaLocal()
        {
            NotaSQL nota = this;

            // Se copian todos los valores tal como está en esta nota.
            // ya que al editarla se usa como NotaSQL aunque sea del tipo Nota.
            var note = new Nota
            {
                //ID = nota.ID,
                Archivada = nota.Archivada,
                Eliminada = nota.Eliminada,
                Favorita = nota.Favorita,
                Grupo = nota.Grupo,
                Modificada = nota.Modificada,
                Notificar = nota.Notificar,
                Sincronizada = nota.Sincronizada,
                Texto = nota.Texto,
                idNota = nota.idNota
            };
            return note;
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
            Favorita = false;
            Sincronizada = false;
            Notificar = false;
            idNota = 0;
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

            try
            {
                if (nota.ID == 0)
                {
                    return NotaSQL.Insert(nota);
                }
                else
                {
                    return NotaSQL.Update(nota);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
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

            nota = ComprobarMaxTexto(nota);
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

            nota = ComprobarMaxTexto(nota);
            var msg = Crear(nota);

            if (msg.StartsWith("ERROR"))
                return 0; // new Task<int>(() => 0);

            return 1; // new Task<int>(() => 1);
        }
        /// <summary>
        /// Comprobar las longitudes máximas                        (25/May/21)
        /// </summary>
        internal static NotaSQL ComprobarMaxTexto(NotaSQL nota)
        {
            if (UsarNotasMaxConfig == false)
                nota.Texto = MaxTexto(nota.Texto, 2048);
            nota.Grupo = MaxTexto(nota.Grupo, 255);
            return nota;
        }
        /// <summary>
        /// Devuelve una cadena con el máximo de caracteres indicados.
        /// </summary>
        internal static string MaxTexto(string campo, int maximo )
        {
            if (string.IsNullOrWhiteSpace(campo))
                campo = " ";
            else
                if (campo.Length > maximo)
                    campo = campo.Substring(0, maximo);
            return campo;
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

        private static string ActualizarNota2048(NotaSQL nota)
        {
            return Actualizar(TablaNotas2048, nota);
        }
        private static string ActualizarNotaMax(NotaSQL nota)
        {
            return Actualizar(TablaNotasMax, nota);
        }

        private static string Actualizar(NotaSQL nota)
        {
            return Actualizar(TablaNotas, nota);
        }
        private static string Actualizar(string tabla, NotaSQL nota)
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
                    sCommand = $"UPDATE {tabla} SET idUsuario = @idUsuario, Grupo = @Grupo, Texto = @Texto, Modificada = @Modificada, Archivada = @Archivada, Eliminada = @Eliminada, Favorita = @Favorita, Sincronizada = @Sincronizada, Notificar = @Notificar, idNota = @idNota  WHERE (ID = @ID)";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@ID", nota.ID);
                    cmd.Parameters.AddWithValue("@idUsuario", nota.idUsuario);
                    cmd.Parameters.AddWithValue("@Grupo", nota.Grupo);
                    cmd.Parameters.AddWithValue("@Texto", nota.Texto);
                    cmd.Parameters.AddWithValue("@Modificada", nota.Modificada);
                    cmd.Parameters.AddWithValue("@Archivada", nota.Archivada);
                    cmd.Parameters.AddWithValue("@Eliminada", nota.Eliminada);
                    cmd.Parameters.AddWithValue("@Favorita", nota.Favorita);
                    cmd.Parameters.AddWithValue("@Sincronizada", nota.Sincronizada);
                    cmd.Parameters.AddWithValue("@Notificar", nota.Notificar);
                    cmd.Parameters.AddWithValue("@idNota", nota.idNota);

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

        private static string CrearNota2048(NotaSQL nota)
        {
            return Crear(TablaNotas2048, nota);
        }
        private static string CrearNotaMax(NotaSQL nota)
        {
            return Crear(TablaNotasMax, nota);
        }

        private static string Crear(NotaSQL nota)
        {
            return Crear(TablaNotas, nota);
        }

        /// <sumary>
        /// Crear un nuevo registro
        /// En caso de error, devolverá la cadena de error empezando por ERROR:.
        /// </sumary>
        private static string Crear(string tabla, NotaSQL nota)
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
                    sCommand = $"INSERT INTO {tabla} (idUsuario, Grupo, Texto, Modificada, Archivada, Eliminada, Favorita, Sincronizada, Notificar, idNota) VALUES(@idUsuario, @Grupo, @Texto, @Modificada, @Archivada, @Eliminada, @Favorita, @Sincronizada, @Notificar, @idNota) SELECT @@Identity";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@idUsuario", nota.idUsuario);
                    cmd.Parameters.AddWithValue("@Grupo", nota.Grupo);
                    cmd.Parameters.AddWithValue("@Texto", nota.Texto);
                    cmd.Parameters.AddWithValue("@Modificada", nota.Modificada);
                    cmd.Parameters.AddWithValue("@Archivada", nota.Archivada);
                    cmd.Parameters.AddWithValue("@Eliminada", nota.Eliminada);
                    cmd.Parameters.AddWithValue("@Favorita", nota.Favorita);
                    cmd.Parameters.AddWithValue("@Sincronizada", nota.Sincronizada);
                    cmd.Parameters.AddWithValue("@Notificar", nota.Notificar);
                    cmd.Parameters.AddWithValue("@idNota", nota.idNota);

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
            // todas las notas estén o no archivadas, pero no las eliminadas
            var colNotas = NotasUsuario(idUsuario, archivadas:null, eliminadas:false);
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
        /// Cuenta el total de registros de la tabla y del usuario indicados.
        /// </summary>
        public static int CountNotas(string tabla, int idUsuario)
        {
            int ret = 0;

            var sel = $"SELECT Count(*) FROM {tabla} " +
                      $"WHERE idUsuario = {idUsuario}";
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

            return ret;
        }
        public static int CountNotas2048(int idUsuario)
        {
            return CountNotas(TablaNotas2048, idUsuario);
        }
        public static int CountNotasMax(int idUsuario)
        {
            return CountNotas(TablaNotasMax, idUsuario);
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
                      $"WHERE idUsuario = {idUsuario} AND (Eliminada = 0  AND Archivada = 0)";
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

        #region Estos métodos ya no son necesarios
        
        /// <summary>
        /// El total de notas archivadas, si no están eliminadas.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        private int CountArchivadas(int idUsuario)
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
        private int CountEliminadas(int idUsuario)
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
        private int CountFavoritas(int idUsuario)
        {
            int ret = 0;

            var sel = $"SELECT Count(*) FROM {TablaNotas} " +
                      $"WHERE idUsuario = {idUsuario} AND Favorita = 1 AND (Eliminada = 0  AND Archivada = 0)";
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


        #endregion

        /// <summary>
        /// Devuelve una lista de todas las notas del usuario indicado.
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

            sel += "ORDER BY Favorita DESC, Modificada DESC, Grupo ASC, ID";
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
        /// Devuelve las notas no sincronizadas del usuario indicado.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario.</param>
        /// <returns>Una colección del tipo <see cref="NotaSQL"/>.</returns>
        public static List<NotaSQL> NotasNoSincronizadas(int idUsuario)
        {
            var colNotas = new List<NotaSQL>();

            var sel = $"SELECT * FROM {TablaNotas} ";
            sel += $"WHERE idUsuario = {idUsuario} AND Sincronizada = 0 ";
            sel += "ORDER BY Favorita DESC, Grupo ASC, Modificada DESC, ID";

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
        /// Devuelve las notas sincronizadas del usuario indicado.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario.</param>
        /// <returns>Una colección del tipo <see cref="NotaSQL"/>.</returns>
        public static List<NotaSQL> NotasSincronizadas(int idUsuario)
        {
            var colNotas = new List<NotaSQL>();

            var sel = $"SELECT * FROM {TablaNotas} ";
            sel += $"WHERE idUsuario = {idUsuario} AND Sincronizada = 1 ";
            sel += "ORDER BY Favorita DESC, Grupo ASC, Modificada DESC, ID";

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
        /// <param name="buscar">El texto a buscar en el Texto.</param>
        /// <param name="buscarGrupo">El texto a buscar en la propiedad Grupo.</param>
        /// <param name="archivadas">Si se busca en las archivadas.</param>
        /// <param name="favoritas">Si se busca en las favoritas.</param>
        /// <param name="eliminadas">Si se busca en las eliminadas.</param>
        /// <param name="notificar">Si se debe buscar en las marcadas para notificar.</param>
        /// <returns>Una colección con las notas halladas.</returns>
        public static List<NotaSQL> NotasBuscar(int idUsuario, string buscar, string buscarGrupo, 
                                                bool favoritas, bool archivadas, bool eliminadas, bool notificar)
        {
            var colNotas = new List<NotaSQL>();

            string sArchivadas, sFavoritas, sEliminadas, sNotificar;

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
            if (!eliminadas)
                sEliminadas = "(Eliminada = 1 OR Eliminada = 0)";
            else
                sEliminadas = "(Eliminada = 1)";
            if (!notificar)
                sNotificar = "(Notificar = 1 OR Notificar = 0)";
            else
                sNotificar = "(Notificar = 1)";

            var sGrupo = "";
            if (buscarGrupo.Any())
            {
                sGrupo = $"AND Grupo like '%{buscarGrupo}%' ";
            }
            var sTexto = "";
            if (buscar.Any())
                sTexto = $"AND Texto like '%{buscar}%' ";

            var sel = $"SELECT * FROM {TablaNotas} ";
            sel += $"WHERE idUsuario = {idUsuario} {sTexto} {sGrupo} " + 
                   $"AND {sFavoritas} AND {sArchivadas} AND {sEliminadas} AND {sNotificar} ";
            sel += "ORDER BY Favorita DESC, Modificada DESC, Grupo ASC, ID";

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
        /// <returns>
        /// Un objeto del tipo <see cref="NotaSQL"/> con la nota indicada.
        /// O un objeto nuevo (ID = 0) si no existe en la base de datos.
        /// </returns>
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

        public static List<NotaSQL> Buscar(string where)
        {
            return Buscar(UsuarioSQL.UsuarioLogin.ID, where);
        }
        /// <summary>
        /// Buscar en la tabla activa de notas.
        /// </summary>
        public static List<NotaSQL> Buscar(int idUsuario, string where)
        {
            return Buscar(TablaNotas, idUsuario, where);
        }
        /// <summary>
        /// Buscar en la tabla Notas.
        /// </summary>
        public static string CopiarNotas20482NotasMax(int idUsuario, string where)
        {
            int t = 0;
            var col= Buscar(TablaNotas2048, idUsuario, where);
            for(var i = 0; i < col.Count; i++)
            {
                col[i].idNota = 0;
                var msg = CrearNotaMax(col[i]);
                if(msg.StartsWith("ERROR"))
                {
                    return msg;
                }
                t++;
            }
            return $"{t.Plural("Copiada")} {t} {t.Plural("nota")} de Notas a NotasMax.";
        }
        /// <summary>
        /// Buscar en la tabla NotasMax.
        /// </summary>
        public static string CopiarNotasMax2Notas2048(int idUsuario, string where)
        {
            int t = 0;
            var col = Buscar(TablaNotasMax, idUsuario, where);
            for (var i = 0; i < col.Count; i++)
            {
                col[i].idNota = 0;
                var msg = CrearNota2048(col[i]);
                if (msg.StartsWith("ERROR"))
                {
                    return msg;
                }
                t++;
            }
            return $"{t.Plural("Copiada")} {t} {t.Plural("nota")} de NotasMax a Notas.";
        }

        /// <summary>
        /// Busca en las notas del usuario indicado lo indicado en where
        /// </summary>
        /// <param name="idUsuario">El ID del usuario que ha hecho login.</param>
        /// <param name="where">Cadena a usar en WHERE con la búsqueda a realizar.</param>
        /// <returns>Una colección de tipo <see cref="NotaSQL"/>.</returns>
        /// <remarks>
        /// En <see cref="where"/> se indicará lo que se quiere comprobar:
        /// Notificar = 1 AND Eliminada = 0
        /// </remarks>
        public static List<NotaSQL> Buscar(string tabla, int idUsuario, string where)
        {
            var colNotas = new List<NotaSQL>();

            var sel = $"SELECT * FROM {tabla} ";
            sel += $"WHERE idUsuario = {idUsuario} AND ({where}) ";
            sel += "ORDER BY Favorita DESC, Grupo ASC, Modificada DESC, ID";

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
            valorBool = false;
            bool.TryParse(reader["Sincronizada"].ToString(), out valorBool);
            nota.Sincronizada = valorBool;
            valorBool = false;
            bool.TryParse(reader["Notificar"].ToString(), out valorBool);
            nota.Notificar = valorBool;

            return nota; // new Task<NotaSQL>(() => nota);
        }
    }
}
