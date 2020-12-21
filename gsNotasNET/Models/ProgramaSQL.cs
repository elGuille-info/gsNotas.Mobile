using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using gsNotasNET.Data;

namespace gsNotasNET.Models
{
    public class ProgramaSQL : NotasNETSQLDatabase
    {

        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Plataformas { get; set; }
        public DateTime Fecha { get; set; }

        /// <sumary>
        /// Actualiza los datos de la instancia actual.
        /// En caso de error, devolverá la cadena de error empezando por ERROR:.
        /// </sumary>
        ///<remarks>
        /// Usando ExecuteNonQuery si la instancia no hace referencia a un registro existente, NO se creará uno nuevo.
        ///</remarks>
        public static string Actualizar(ProgramaSQL programa)
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
                    sCommand = "UPDATE GuilleDB.Programas SET Nombre = @Nombre, Plataformas = @Plataformas, Fecha = @Fecha  WHERE (ID = @ID)";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@ID", programa.ID);
                    cmd.Parameters.AddWithValue("@Nombre", programa.Nombre);
                    cmd.Parameters.AddWithValue("@Plataformas", programa.Plataformas);
                    cmd.Parameters.AddWithValue("@Fecha", programa.Fecha);

                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();

                    // Si llega aquí es que todo fue bien,
                    // por tanto, llamamos al método Commit.
                    tran.Commit();

                    msg = "Se ha actualizado un Programas correctamente.";

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
                        msg = $"ERROR RollBack: {ex2.Message}";
                    }

                    finally
                    {
                        if (!(con is null))
                        {
                            con.Close();
                        }
                    }
                }
                return msg;
            }
        }

        /// <sumary>
        /// Crear un nuevo registro
        /// En caso de error, devolverá la cadena de error empezando por ERROR:.
        /// </sumary>
        public static string Crear(ProgramaSQL programa)
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
                    sCommand = "INSERT INTO GuilleDB.Programas (Nombre, Plataformas, Fecha)  VALUES(@Nombre, @Plataformas, @Fecha) SELECT @@Identity";
                    cmd.CommandText = sCommand;

                    cmd.Parameters.AddWithValue("@Nombre", programa.Nombre);
                    cmd.Parameters.AddWithValue("@Plataformas", programa.Plataformas);
                    cmd.Parameters.AddWithValue("@Fecha", programa.Fecha);

                    cmd.Transaction = tran;

                    int id = (int)(cmd.ExecuteScalar());
                    programa.ID = id;

                    // Si llega aquí es que todo fue bien,
                    // por tanto, llamamos al método Commit.
                    tran.Commit();

                    msg = "Se ha creado un Programas correctamente.";

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

                    finally
                    {
                        if (!(con is null))
                        {
                            con.Close();
                        }
                    }
                }
            }
            return msg;
        }

        ///<sumary>
        /// Borrar el registro con el mismo ID que tenga la clase.
        /// NOTA: En caso de que quieras usar otro criterio
        /// para comprobar cuál es el registro actual, cambia la comparación.
        ///</sumary>
        public static string Borrar(string where)
        {
            string msg = "";

            string sCon = CadenaConexion;
            string sel = "DELETE FROM Programas WHERE " + where;

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

                    msg = "Eliminado correctamente los registros con : " + where + ".";

                }
                catch (Exception ex)
                {
                    msg = "ERROR al eliminar los registros con : " + where + "." + ex.Message;

                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        msg = $"ERROR RollBack: {ex2.Message})";
                    }
                    finally
                    {
                        if (!(con is null))
                        {
                            con.Close();
                        }
                    }
                }
            }
            return msg;
        }
    }
}