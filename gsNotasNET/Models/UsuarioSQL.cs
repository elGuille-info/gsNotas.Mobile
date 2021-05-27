//-----------------------------------------------------------------------------
// UsuarioSQL                                                       (22/Dic/20)
// Clase para los usuarios de las notas remotas.
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
using System.Diagnostics;
using gsNotasNET.Data;

namespace gsNotasNET.Models
{
    public class UsuarioSQL : NotasNETSQLDatabase
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string ClaveSHA { get; set; }
        public DateTime Alta { get; set; }
        //public DateTime Baja { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public bool Eliminado { get; set; } = false;
        public bool Validado { get; set; } = false;
        //public bool NotasCopiadas { get; set; } = false;
        public string VersionPrograma { get; set; } = "gsNotasNET.Android v2.1.0.0";
        public int Cuota { get; set; } = 100;
        public decimal Pagos { get; set; } = 0;
        
        private bool _UsarNotasMax = false;
        public bool UsarNotasMax 
        {
            get { return _UsarNotasMax; }
            set 
            { 
                //UsarNotasMaxConfig = value;
                _UsarNotasMax = value;
            } 
        }

        //public bool NotasCopiadasAndroid { get; set; } = false;
        //public string CrLf { get; } = "\r\n";

        /// <summary>
        /// El password usado por este usuario, no se guarda en la base de datos.
        /// </summary>
        public string Password { get; set; }

        public UsuarioSQL()
        {
            ID = 0;
            Email = "";
            Nombre = "";
            ClaveSHA = "";
            Alta = DateTime.UtcNow;
            //Baja = new DateTime(2099, 12, 31);
            UltimoAcceso = DateTime.UtcNow;
            Eliminado = false;
            Validado = false;
            //NotasCopiadas = false;
            VersionPrograma = $"{App.AppName} {App.AppVersion}";
            //NotasCopiadasAndroid = false;
            Cuota = 100;
            Pagos = 0;
            UsarNotasMax = false;
        }

        /// <summary>
        /// El usuario que ha hecho Login.
        /// </summary>
        public static UsuarioSQL UsuarioLogin { get; set; }

        ///// <summary>
        ///// El password usado para hacer Loging.
        ///// </summary>
        //private static string PasswordUsuario { get; set; }


        /// <summary>
        /// Guarda o actualiza la nota indicada. 
        /// Si el ID no es cero, la actualiza, si es cero la crea.
        /// </summary>
        /// <param name="usuario">La nota a guardar.</param>
        /// <returns>
        /// El número de notas afectadas (0 si no se guardó o 1 si se actualizó o creó correctamente).
        /// </returns>
        public static int GuardarUsuario(UsuarioSQL usuario, string password ="")
        {
            if (usuario is null)
                return 0; // new Task<int>(() => 0);

            if (usuario.ID == 0 && password.Any())
            {
                usuario.ClaveSHA = GenerarClaveSHA1(usuario.Email.TrimEnd(), password);
                return UsuarioSQL.Insert(usuario);
            }
            else if(usuario.ID != 0)
            {
                return UsuarioSQL.Update(usuario);
            }
            return 0;
        }

        /// <summary>
        /// Actualiza el usuario en la tabla Usuarios.
        /// </summary>
        /// <param name="usuario">El usuario a actualizar.</param>
        /// <returns>El número de usuarios afectados (o cero si no se pudo actualizar).</returns>
        internal static int Update(UsuarioSQL usuario)
        {
            usuario = ComprobarMaxTexto(usuario);
            var msg = Actualizar(usuario);

            if (msg.StartsWith("ERROR"))
                return 0; // new Task<int>(() => 0);

            return 1; // new Task<int>(() => 1);
        }

        /// <summary>
        /// Inserta un nuevo usuario en la tabla Usuarios.
        /// </summary>
        /// <param name="usuario">El usuario a añadir.</param>
        /// <returns>El número de usuarios afectados (o cero si no se pudo insertar).</returns>
        internal static int Insert(UsuarioSQL usuario)
        {
            usuario = ComprobarMaxTexto(usuario);
            var msg = Crear(usuario);

            if (msg.StartsWith("ERROR"))
                return 0; // new Task<int>(() => 0);

            return 1; // new Task<int>(() => 1);
        }

        /// <summary>
        /// Comprobar las longitudes máximas                        (25/May/21)
        /// </summary>
        internal static UsuarioSQL ComprobarMaxTexto(UsuarioSQL usuario)
        {
            usuario.Email = MaxTexto(usuario.Email, 50);
            usuario.Nombre = MaxTexto(usuario.Nombre, 50);
            usuario.VersionPrograma = MaxTexto(usuario.VersionPrograma, 35);
            return usuario;
        }
        /// <summary>
        /// Devuelve una cadena con el máximo de caracteres indicados.
        /// </summary>
        internal static string MaxTexto(string campo, int maximo)
        {
            if (string.IsNullOrWhiteSpace(campo))
                campo = " ";
            else
                if (campo.Length > maximo)
                    campo = campo.Substring(0, maximo);
            return campo;
        }


        /// <summary>
        /// Elimina el usuario con el ID indicado.
        /// En realidad no se elimina, se indica que está de baja con la fecha UTC actual y se marca Eliminado = true.
        /// </summary>
        /// <param name="usuario">El usuario a eliminar.</param>
        /// <returns>El número de usuarios afectados (o cero si no se pudo eliminar).</returns>
        public static int Delete(UsuarioSQL usuario)
        {
            usuario.Eliminado = true;
            var msg = Actualizar(usuario);

            //var sel = $"ID = {usuario.ID}";
            //var msg = Borrar(sel);

            if (msg.StartsWith("ERROR"))
                return 0; // new Task<int>(() => 0);

            return 1; // new Task<int>(() => 1);
        }

        /// <sumary>
        /// Actualiza los datos de la instancia actual.
        /// En caso de error, devolverá la cadena de error empezando por ERROR:.
        /// </sumary>
        /// <remarks>
        /// Usando ExecuteNonQuery si la instancia no hace referencia a un registro existente, NO se creará uno nuevo.
        /// </remarks>
        private static string Actualizar(UsuarioSQL usuario)
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
                    sCommand = $"UPDATE {TablaUsuarios} SET Email = @Email, Nombre = @Nombre, ClaveSHA = @ClaveSHA, Alta = @Alta, UltimoAcceso = @UltimoAcceso, Eliminado = @Eliminado, Validado = @Validado, VersionPrograma = @VersionPrograma, Cuota = @Cuota, Pagos = @Pagos, UsarNotasMax = @UsarNotasMax WHERE (ID = @ID)";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@ID", usuario.ID);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email.TrimEnd());
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre.TrimEnd());
                    cmd.Parameters.AddWithValue("@ClaveSHA", usuario.ClaveSHA);
                    cmd.Parameters.AddWithValue("@Alta", usuario.Alta);
                    cmd.Parameters.AddWithValue("@UltimoAcceso", usuario.UltimoAcceso);
                    cmd.Parameters.AddWithValue("@Eliminado", usuario.Eliminado);
                    cmd.Parameters.AddWithValue("@Validado", usuario.Validado);
                    cmd.Parameters.AddWithValue("@VersionPrograma", usuario.VersionPrograma.TrimEnd());
                    cmd.Parameters.AddWithValue("@Cuota", usuario.Cuota);
                    cmd.Parameters.AddWithValue("@Pagos", usuario.Pagos);
                    cmd.Parameters.AddWithValue("@UsarNotasMax", usuario.UsarNotasMax);

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
                        msg = $"ERROR RollBack: {ex2.Message})";
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
        /// En caso de error, devolverá la cadena de error empezando por ERROR.
        /// </sumary>
        private static string Crear(UsuarioSQL usuario)
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
                    sCommand = $"INSERT INTO {TablaUsuarios} (Email, Nombre, ClaveSHA, Alta, UltimoAcceso, Eliminado, Validado, VersionPrograma, Cuota, Pagos, UsarNotasMax) VALUES(@Email, @Nombre, @ClaveSHA, @Alta, @UltimoAcceso, @Eliminado, @Validado, @VersionPrograma, @Cuota, @Pagos, @UsarNotasMax) SELECT @@Identity";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@Email", usuario.Email.TrimEnd());
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre.TrimEnd());
                    cmd.Parameters.AddWithValue("@ClaveSHA", usuario.ClaveSHA);
                    cmd.Parameters.AddWithValue("@Alta", usuario.Alta);
                    cmd.Parameters.AddWithValue("@UltimoAcceso", usuario.UltimoAcceso);
                    cmd.Parameters.AddWithValue("@Eliminado", usuario.Eliminado);
                    cmd.Parameters.AddWithValue("@Validado", usuario.Validado);
                    cmd.Parameters.AddWithValue("@VersionPrograma", usuario.VersionPrograma.TrimEnd());
                    cmd.Parameters.AddWithValue("@Cuota", usuario.Cuota);
                    cmd.Parameters.AddWithValue("@Pagos", usuario.Pagos);
                    cmd.Parameters.AddWithValue("@UsarNotasMax", usuario.UsarNotasMax);

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
                        msg = $"ERROR RollBack: {ex2.Message})";
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
        private static string Borrar(string where)
        {
            string msg = "";

            string sCon = CadenaConexion;
            string sel = $"DELETE FROM {TablaUsuarios} WHERE {where}";

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
        /// <returns>Un valor entero con los elementos totales de la tabla.</returns>
        internal static int Count()
        {
            int ret = 0;

            var sel = $"SELECT Count(*) FROM {TablaUsuarios} WHERE Eliminado = 0 ";
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

        internal static int CountDeBaja()
        {
            int ret = 0;

            var sel = $"SELECT Count(*) FROM {TablaUsuarios} WHERE (Eliminado = 1 OR Baja < '{DateTime.UtcNow}' ";
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
        /// Devuelve una lista de todos los usuarios de la base de datos.
        /// </summary>
        /// <returns>Una colección de tipo HashSet con los usuarios.</returns>
        internal static List<UsuarioSQL> Usuarios(bool todos = false)
        {
            var colUsers = new List<UsuarioSQL>();

            var sel = $"SELECT * FROM {TablaUsuarios} ";
                if(todos)
                    sel += $"ORDER BY ID";
                else
                    sel += $"WHERE (Eliminado = 0 AND Baja > '{DateTime.UtcNow}' ) ORDER BY ID";
            var con = new SqlConnection(CadenaConexion);
            try 
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    colUsers.Add(AsignarUsuario(reader));
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

            return colUsers;
        }

        /// <summary>
        /// Devuelve el usuario con ese correo.
        /// Si hubiese más de uno (no debería), 
        /// devolverá el último que se registró.
        /// </summary>
        /// <param name="email">El email del usuario a obtener.</param>
        /// <returns>Un objeto del tipo <see cref="UsuarioSQL"/>. Si el usuario no existe, el ID será 0.</returns>
        /// <remarks>Asigna UsuarioLogin.</remarks>
        public static UsuarioSQL Usuario(string email)
        {
            var usuario = new UsuarioSQL();

            var sel = $"SELECT * FROM {TablaUsuarios} " + 
                      $"WHERE Email = '{email.TrimEnd()}' "+ 
                      "ORDER BY ID";
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //usuario = await Task.Run<UsuarioSQL>(() => AsignarUsuario(reader));
                    usuario = AsignarUsuario(reader);
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

            UsuarioLogin = usuario;

            return usuario; // new Task<UsuarioSQL>(()=> usuario);
        }

        /// <summary>
        /// Comprueba si el email indicado está registrado.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true si ya existe, false si no existe ese email.</returns>
        public static bool Existe(string email)
        {
            var sel = $"SELECT Count(*) FROM {TablaUsuarios} " +
                      $"WHERE Email = '{email.TrimEnd()}' ";
            var res = false;
            var con = new SqlConnection(CadenaConexion);
            try
            {
                con.Open();
                var cmd = new SqlCommand(sel, con);

                var t = (int)cmd.ExecuteScalar();
                res = t > 0;
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
            return res;
        }

        /// <summary>
        /// Comprueba si los datos de email y password son correctos.
        /// Si es correcto, se asigna al usuario que ha hecho login.
        /// </summary>
        /// <param name="email">El email del usuario.</param>
        /// <param name="password">El password indicado por el usuario.</param>
        /// <returns>Un valor true si la combinación es correcta, false si no lo es.</returns>
        /// <remarks>Se asigna UsuarioLogin, si el ID es cero es que el usuario no existe.</remarks>
        public static bool ComprobarContraseña(string email, string password)
        {
            var claveSHA = GenerarClaveSHA1(email.TrimEnd(), password);
            var usuario = Usuario(email.TrimEnd());
            
            if (claveSHA == usuario.ClaveSHA)
            {
                // Asignar el usuario, si no existe el ID será 0
                UsuarioSQL.UsuarioLogin = usuario;
                UsuarioSQL.UsuarioLogin.Password = password; // .PasswordUsuario = password;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Asigna un objeto a partir de los datos del SQLReader.
        /// </summary>
        /// <param name="reader">El SQLReader del que se sacará la información.</param>
        /// <returns>Un objeto del tipo <see cref="UsuarioSQL"/>.</returns>
        private static UsuarioSQL AsignarUsuario(SqlDataReader reader)
        {
            var usuario = new UsuarioSQL();

            var id = 0;
            int.TryParse(reader["ID"].ToString(), out id);
            usuario.ID = id;
            usuario.Email = reader["Email"].ToString().TrimEnd();
            usuario.Nombre = reader["Nombre"].ToString().TrimEnd();
            usuario.ClaveSHA = reader["ClaveSHA"].ToString();
            var fec = new DateTime(1900, 1, 1);
            DateTime.TryParse(reader["Alta"].ToString(), out fec);
            usuario.Alta = fec;
            fec = DateTime.Now;
            DateTime.TryParse(reader["UltimoAcceso"].ToString(), out fec);
            usuario.UltimoAcceso = fec;
            var valorBool = false;
            bool.TryParse(reader["Eliminado"].ToString(), out valorBool);
            usuario.Eliminado = valorBool;
            valorBool = false;
            bool.TryParse(reader["Validado"].ToString(), out valorBool);
            usuario.Validado = valorBool;
            usuario.VersionPrograma = reader["VersionPrograma"].ToString().TrimEnd();
            id = 0;
            int.TryParse(reader["Cuota"].ToString(), out id);
            usuario.Cuota = id;
            decimal dec = 0;
            decimal.TryParse(reader["Pagos"].ToString(), out dec);
            usuario.Pagos = dec;
            valorBool = false;
            bool.TryParse(reader["UsarNotasMax"].ToString(), out valorBool);
            usuario.UsarNotasMax = valorBool;

            return usuario;
        }
    }
}
