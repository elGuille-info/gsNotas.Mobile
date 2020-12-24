using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
//using SQLite;
using gsNotasNET.Models;
using System.Linq;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;

namespace gsNotasNET.Data
{
    /// <summary>
    /// Clase base para las clases usadas para acceder a las tablas de 8174979_NotasNET.
    /// </summary>
    public class NotasNETSQLDatabase
    {
        /// <summary>
        /// Nombre de la tabla de Usuarios
        /// </summary>
        public static string TablaUsuarios { get { return "GuilleDB.Usuarios"; } }
        /// <summary>
        /// Nombre de la tabla de Notas.
        /// </summary>
        public static string TablaNotas { get { return "GuilleDB.Notas"; } }
        
        ///// <summary>
        ///// Nombre de la tabla de Programas.
        ///// </summary>
        //public static string TablaProgramas { get { return "GuilleDB.Programas"; } }

        private static string _CadenaConexion;

        /// <summary>
        /// La cadena de conexión a la base de SQL Server
        /// en el servidor de ascens.
        /// </summary>
        public static string CadenaConexion
        {
            get
            {
                return _CadenaConexion;
            }
            set
            {
                var csb = new SqlConnectionStringBuilder();
                
                using (var passwStream = new StreamReader(App.CredencialesSQL))
                {
                    var s = passwStream.ReadLine();
                    csb.UserID = s;
                    s = passwStream.ReadLine();
                    csb.Password = s;
                }
                csb.DataSource = "pmssql100.dns-servicio.com";
                csb.InitialCatalog = "8174979_NotasNET";
                csb.IntegratedSecurity = false;

                _CadenaConexion = csb.ConnectionString;
            }
        }

        /// <summary>
        /// Genera un SHA de validación.
        /// </summary>
        /// <param name="email">El email a comprobar</param>
        /// <returns>Una cadena con 8 caracteres para el código de validación.</returns>
        /// <remarks>Es válido durante la hora actual.</remarks>
        public static string ValidarHash(string email)
        {
            // Código válido en esta hora
            DateTime fecha;

            //// Si el minuto actual es 59, añadirle uno
            //if(DateTime.UtcNow.Minute == 59)
            //    fecha = DateTime.UtcNow.AddMinutes(1);
            //else 
            //    fecha = DateTime.UtcNow;

            fecha = DateTime.UtcNow;
            string h = GenerarClaveSHA1(fecha.ToString("yyyyMMddHH"), email);
            if (string.IsNullOrWhiteSpace(h))
                return "";

            return h.Substring(0, 4) + h.Substring(16, 4);
        }

        /// <summary>
        /// Generar una clave SHA1 para guardarla en lugar del password,
        /// de esa forma no se podrá saber la clave.
        /// La longitud es de 40 caracteres.
        /// </summary>
        /// <remarks>
        /// Crear una clave SHA1 como la generada por:
        /// FormsAuthentication.HashPasswordForStoringInConfigFile
        /// Basado en el ejemplo de mi sitio:
        /// http://www.elguille.info/NET/dotnet/comprobar_usuario_usando_base_datos_cs2003.htm
        /// </remarks>
        public static string GenerarClaveSHA1(string nick, string clave)
        {
            // Crear una clave SHA1 como la generada por 
            // FormsAuthentication.HashPasswordForStoringInConfigFile
            // Adaptada del ejemplo de la ayuda en la descripción de SHA1 (Clase)
            UTF8Encoding enc = new UTF8Encoding();
            // Por si el usuario (nick) es nulo
            if (string.IsNullOrWhiteSpace(nick))
                nick = "";
            else
                nick = nick.ToLower();
            byte[] data = enc.GetBytes(nick + clave);
            byte[] result;

            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            // This is one implementation of the abstract class SHA1.
            result = sha.ComputeHash(data);

            // Convertir los valores en hexadecimal
            // cuando tiene una cifra hay que rellenarlo con cero
            // para que siempre ocupen dos dígitos.
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] < 16)
                    sb.Append("0");
                sb.Append(result[i].ToString("x"));
            }

            return sb.ToString().ToUpper();
        }
    }
}
