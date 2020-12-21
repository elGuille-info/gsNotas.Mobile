using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
//using SQLite;
using gsNotasNET.Models;
using System.Linq;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace gsNotasNET.Data
{
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
        /// <summary>
        /// Nombre de la tabla de Programas.
        /// </summary>
        public static string TablaProgramas { get { return "GuilleDB.Programas"; } }

        /// <summary>
        /// La cadena de conexión a la base de SQL Server
        /// en el servidor de ascens.
        /// </summary>
        public static string CadenaConexion
        {
            get
            {
                var csb = new SqlConnectionStringBuilder();
                csb.DataSource = "pmssql100.dns-servicio.com";
                csb.InitialCatalog = "8174979_NotasNET";
                csb.IntegratedSecurity = false;
                csb.UserID = "UserNotasDB";
                csb.Password = "jd1zD45!";

                return csb.ConnectionString;
            }
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
