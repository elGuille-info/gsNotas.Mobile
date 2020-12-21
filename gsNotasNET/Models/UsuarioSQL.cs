using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using gsNotasNET.Data;

namespace gsNotasNET.Models
{
    public class UsuarioSQL: NotasNETSQLDatabase
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string ClaveSHA { get; set; }
        public DateTime Alta { get; set; }
        public DateTime Baja { get; set; }
        public DateTime UltimoAcceso { get; set; }

        public UsuarioSQL()
        {
            ID = 0;
            Email = "";
            Nombre = "";
            ClaveSHA = "";
            Alta = DateTime.UtcNow;
            Baja = new DateTime(2099, 12, 31);
            UltimoAcceso = DateTime.UtcNow;
        }

        /// <summary>
        /// Guarda o actualiza la nota indicada. 
        /// Si el ID no es cero, la actualiza, si es cero la crea.
        /// </summary>
        /// <param name="usuario">La nota a guardar.</param>
        /// <returns>
        /// El número de notas afectadas (0 si no se guardó o 1 si se actualizó o creó correctamente).
        /// </returns>
        public static Task<int> GuardarUsuarioAsync(UsuarioSQL usuario)
        {
            if (usuario is null)
                return new Task<int>(() => 0);

            if (usuario.ID == 0)
            {
                usuario.ClaveSHA = GenerarClaveSHA1(usuario.Email, App.PasswordUsuario);
                return InsertAsync(usuario);
            }
            else
            {
                return UpdateAsync(usuario);
            }
        }

        /// <summary>
        /// Borrar el usuario indicado.
        /// Si tiene un ID = 0, no se hace nada.
        /// </summary>
        /// <param name="usuario">El usuario a eliminar.</param>
        /// <returns>
        /// El número de usuarios afectados (0 si no se eliminó o 1 si se eliminó ).
        /// </returns>
        public static Task<int> BorrarNotaAsync(UsuarioSQL usuario)
        {
            if (usuario is null || usuario.ID == 0)
                return new Task<int>(() => 0);

            return DeleteAsync(usuario);
        }

        /// <summary>
        /// Actualiza el usuario en la tabla Usuarios.
        /// </summary>
        /// <param name="usuario">El usuario a actualizar.</param>
        /// <returns>El número de usuarios afectados (o cero si no se pudo actualizar).</returns>
        public static Task<int> UpdateAsync(UsuarioSQL usuario)
        {
            var msg = Actualizar(usuario);

            if (msg.StartsWith("ERROR"))
                return new Task<int>(() => 0);

            return new Task<int>(() => 1);
        }

        /// <summary>
        /// Inserta un nuevo usuario en la tabla Usuarios.
        /// </summary>
        /// <param name="usuario">El usuario a añadir.</param>
        /// <returns>El número de usuarios afectados (o cero si no se pudo insertar).</returns>
        public static Task<int> InsertAsync(UsuarioSQL usuario)
        {
            var msg = Crear(usuario);

            if (msg.StartsWith("ERROR"))
                return new Task<int>(() => 0);

            return new Task<int>(() => 1);
        }

        /// <summary>
        /// Elimina el usuario con el ID indicado.
        /// En realidad no se elimina, se indica que está de baja con la fecha UTC actual.
        /// </summary>
        /// <param name="usuario">El usuario a eliminar.</param>
        /// <returns>El número de usuarios afectados (o cero si no se pudo eliminar).</returns>
        public static Task<int> DeleteAsync(UsuarioSQL usuario)
        {
            usuario.Baja = DateTime.UtcNow;
            var msg = Actualizar(usuario);

            //var sel = $"Baja = '{DateTime.UtcNow.ToString("yyyy-MM-dd")}'";
            //var msg = Borrar(sel);
            if (msg.StartsWith("ERROR"))
                return new Task<int>(() => 0);

            return new Task<int>(() => 1);
        }

        /// <sumary>
        /// Actualiza los datos de la instancia actual.
        /// En caso de error, devolverá la cadena de error empezando por ERROR:.
        /// </sumary>
        /// <remarks>
        /// Usando ExecuteNonQuery si la instancia no hace referencia a un registro existente, NO se creará uno nuevo.
        /// </remarks>
        public static string Actualizar(UsuarioSQL usuario)
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
                    sCommand = "UPDATE GuilleDB.Usuarios SET Email = @Email, Nombre = @Nombre, ClaveSHA = @ClaveSHA, Alta = @Alta, Baja = @Baja, UltimoAcceso = @UltimoAcceso  WHERE (ID = @ID)";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@ID", usuario.ID);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@ClaveSHA", usuario.ClaveSHA);
                    cmd.Parameters.AddWithValue("@Alta", usuario.Alta);
                    cmd.Parameters.AddWithValue("@Baja", usuario.Baja);
                    cmd.Parameters.AddWithValue("@UltimoAcceso", usuario.UltimoAcceso);

                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();

                    // Si llega aquí es que todo fue bien,
                    // por tanto, llamamos al método Commit.
                    tran.Commit();

                    msg = "Se ha actualizado un Usuarios correctamente.";
                }
                catch (Exception ex)
                {
                    msg = $"ERROR: {ex.Message}";
                    // Si hay error, deshacemos lo que se haya hecho.
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        msg = $" (ERROR RollBack: {ex2.Message})";
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
        public static string Crear(UsuarioSQL usuario)
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
                    sCommand = "INSERT INTO GuilleDB.Usuarios (Email, Nombre, ClaveSHA, Alta, Baja, UltimoAcceso)  VALUES(@Email, @Nombre, @ClaveSHA, @Alta, @Baja, @UltimoAcceso) SELECT @@Identity";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@ClaveSHA", usuario.ClaveSHA);
                    cmd.Parameters.AddWithValue("@Alta", usuario.Alta);
                    cmd.Parameters.AddWithValue("@Baja", usuario.Baja);
                    cmd.Parameters.AddWithValue("@UltimoAcceso", usuario.UltimoAcceso);

                    cmd.Transaction = tran;

                    int id = System.Convert.ToInt32(cmd.ExecuteScalar());
                    usuario.ID = id;

                    // Si llega aquí es que todo fue bien,
                    // por tanto, llamamos al método Commit.
                    tran.Commit();

                    msg = "Se ha creado un Usuarios correctamente.";
                }
                catch (Exception ex)
                {
                    msg = $"ERROR: {ex.Message}";
                    try
                    {
                        // Si hay error, deshacemos lo que se haya hecho.
                        tran.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        msg = $" (ERROR RollBack: {ex2.Message})";
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
        /// Borrar el registro con el mismo ID que tenga la clase.
        /// NOTA: En caso de que quieras usar otro criterio
        /// para comprobar cuál es el registro actual, cambia la comparación.
        /// </sumary>
        public static string Borrar(string where)
        {
            string msg = "";

            string sCon = CadenaConexion;
            string sel = "DELETE FROM GuilleDB.Usuarios WHERE " + where;

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

                    // Si llega aquí es que todo fue bien,
                    // por tanto, llamamos al método Commit.
                    tran.Commit();

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
        /// El número de elementos en la tabla.
        /// </summary>
        /// <returns></returns>
        public static Task<int> CountAsync()
        {
            int ret = 0;
            
            var sel = $"SELECT Count(*) FROM {TablaUsuarios} ";
            try
            {
                var con = new SqlConnection(CadenaConexion);
                var cmd = new SqlCommand(sel, con);

                var t = cmd.ExecuteNonQuery();
                ret = t;
            }
            catch { }

            return new Task<int>(() => ret);
        }

        /// <summary>
        /// Devuelve una lista de todos los usuarios de la base de datos.
        /// </summary>
        /// <returns>Una colección de tipo HashSet con los usuarios.</returns>
        public static Task<HashSet<UsuarioSQL>> UsuariosAsync()
        {
            var colUsers = new HashSet<UsuarioSQL>();

            var sel = $"SELECT * FROM {TablaUsuarios} ORDER BY ID";
            try 
            {
                var con = new SqlConnection(CadenaConexion);
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    colUsers.Add(AsignarUsuarioAsync(reader).Result);
                }
            } 
            catch { }

            return new Task<HashSet<UsuarioSQL>>(() => colUsers);
        }

        /// <summary>
        /// Devuelve el usuario con ese correo.
        /// Si hubiese más de uno (no debería), 
        /// devolverá el último que se registró.
        /// </summary>
        /// <param name="email">El email del usuario a obtener.</param>
        /// <returns>Un objeto del tipo <see cref="UsuarioSQL"/>.</returns>
        public static UsuarioSQL Usuario(string email)
        {
            var usuario = new UsuarioSQL();

            var sel = $"SELECT * FROM {TablaUsuarios} WHERE Email = '{email}' ORDER BY ID";
            try
            {
                var con = new SqlConnection(CadenaConexion);
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    usuario = AsignarUsuarioAsync(reader).Result;
                }
            }
            catch { }

            return usuario;
        }

        /// <summary>
        /// Comprueba si los datos de email y password son correctos.
        /// </summary>
        /// <param name="email">El email del usuario.</param>
        /// <param name="password">El password indicado por el usuario.</param>
        /// <returns>Un valor true si la combinación es correcta, false si no lo es.</returns>
        public static bool ComprobarContraseña(string email, string password)
        {
            var claveSHA = GenerarClaveSHA1(email, password);

            var sel = $"SELECT * FROM {TablaUsuarios} " +
                      $"WHERE Email = '{email}' AND ClaveSHA = '{claveSHA}' ORDER BY ID";
            
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(CadenaConexion);
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var t = (int)cmd.ExecuteScalar();
                return t > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (!(con is null))
                    con.Close();
            }
        }

        /// <summary>
        /// Asigna un objeto a partir de los datos del SQLReader.
        /// </summary>
        /// <param name="reader">El SQLReader del que se sacará la información.</param>
        /// <returns>Un objeto del tipo <see cref="UsuarioSQL"/>.</returns>
        private static Task<UsuarioSQL> AsignarUsuarioAsync(SqlDataReader reader)
        {
            var usuario = new UsuarioSQL();

            var id = 0;
            int.TryParse(reader["ID"].ToString(), out id);
            usuario.ID = id;
            usuario.Email = reader["Email"].ToString();
            usuario.Nombre = reader["Nombre"].ToString();
            usuario.ClaveSHA = reader["ClaveSHA"].ToString();
            var fec = new DateTime(1900, 1, 1);
            DateTime.TryParse(reader["Alta"].ToString(), out fec);
            usuario.Alta = fec;
            fec = new DateTime(2900, 12, 31);
            DateTime.TryParse(reader["Baja"].ToString(), out fec);
            usuario.Baja = fec;
            fec = DateTime.Now;
            DateTime.TryParse(reader["UltimoAcceso"].ToString(), out fec);
            usuario.UltimoAcceso = fec;

            return new Task<UsuarioSQL>(() => usuario);
        }
    }
}
