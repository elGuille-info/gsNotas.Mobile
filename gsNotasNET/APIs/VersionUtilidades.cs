//------------------------------------------------------------------------------
// Versiones utilidades                                              (04/Dic/20)
// Para comprobar las versiones de mis utilidades y programas
//
// Versión convertida a C#                                          (05/Dic/20)
// Agrego la función CompararVersionWeb con dos sobrecargas         (05/Dic/20)
// 
// Esa información está en 
//   www.elguille.info/NET/dotnet/versiones-utilidades.aspx
// Y el formato es: 
//   <meta name="NombreAplicacion" content="1.0.0.0" />
// Por ejemplo:
//   <meta name="Reloj Windows (C#)" content="1.0.0.4" />
//   <meta name="gsPanelClip" content="4.0.1.7" />
// Para comprobar directamente las versiones usar: EsMayorLaVersionWeb
//
// (c) Guillermo (elGuille) Som, 2020
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gsNotasNET.APIs
{
    public class VersionUtilidades
    {
        private const string laUrl = "http://www.elguille.info/NET/dotnet/versiones-utilidades.aspx";

        /// <summary>
        /// Devuelve una cadena en formato 0.0.0.0 con la versión del programa indicado.
        /// </summary>
        /// <param name="aplicacion">Nombre de la aplicación 
        /// en .NET Framework lo que se pone en AssemblyName (Product en la ventana de Assembly Information) o en 
        /// lo que se pone en Product en .NET 5.0, ese el valor devuelto por Application.ProductName.</param>
        /// <returns>Devuelve una cadena con la versión leída en elGuille.info 
        /// o "" si no se ha hallado el nombre de la aplciación</returns>
        public static string VersionWeb(string aplicacion)
        {
            try
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create(laUrl);
                System.Net.WebResponse response;
                StreamReader reader;
                // Obtener la respuesta.
                response = request.GetResponse();
                // Abrir el stream de la respuesta recibida.
                reader = new StreamReader(response.GetResponseStream());
                // Leer el contenido.
                string s = reader.ReadToEnd();
                // Cerrar los streams abiertos.
                reader.Close();
                response.Close();

                // Comprobar el valor de <meta name="ProductName" 
                // Usar esta expresión regular: <meta name="version" content="(\d.\d.\d.\d)" />
                // En Groups(1) estará la versión
                // Comprobar que haya más de una cifra                   (14/Abr/07)
                // Tener en cuenta que se pueda usar en el formato > y /> (con o sin espacio)
                var elMeta = @$"<meta name=""{aplicacion}""";
                Regex r = new Regex(elMeta + @" content=""(\d{1,}.\d{1,}.\d{1,}.\d{1,})""\s?/?>");

                foreach (Match m in r.Matches(s))
                {
                    if (m.Groups.Count > 1)
                        return m.Groups[1].Value;
                }
            }
            catch //(Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                //return "";
            }

            return "";
        }

        /// <summary>
        /// Comprueba la versión de la aplicación indicada y la compara con la indicada en versionActual.
        /// Si la version en la web es mayor, devuelve True, en otro caso devuelve False.
        /// </summary>
        /// <param name="aplicacion">Nombre de la aplicación a comprobar. 
        /// Debería ser el valor devuelto por Application.ProductName.</param>
        /// <param name="versionActual">La versión actual de la aplicación.
        /// Debería ser la versión devuelta por Application.ProductVersion .</param>
        /// <param name="laVersionWeb">Aquí devolverá la versión que hay en la Web</param>
        /// <returns>Devuelve 0 si son iguales, -1 si la de la web es menor o 1 si la de la web es mayor.</returns>
        public static int CompararVersionWeb(string aplicacion, string versionActual, ref string laVersionWeb)
        {
            // Para comprobar las versiones

            var vWeb = VersionWeb(aplicacion);
            laVersionWeb = vWeb;

            // Por si no lee bien la versión
            if (string.IsNullOrEmpty(vWeb))
                vWeb = "0.0.0.0";

            // Para comprobar mejor las versiones de la Web (del AcercaDe usado en colorear código)
            // Solo funcionará bien con valores de 1 cifra
            // ya que 1.0.3.11 será menor que 1.0.3.9 aunque no sea así...
            // Convertirlo en cadena de números de dos cifras

            var aWeb = vWeb.Split('.');

            var aFic = versionActual.Split('.');

            vWeb = "";

            var vApp = "";

            for (var i = 0; i < aWeb.Length; i++)
                vWeb += Convert.ToInt32(aWeb[i]).ToString("00") + ".";
            for (var i = 0; i < aFic.Length; i++)
                vApp += Convert.ToInt32(aFic[i]).ToString("00") + ".";

            // Devolver 0 si son iguales, -1 si la de la web es menor o 1 si la de la web es mayor
            return vWeb.CompareTo(vApp);

            //if (vWeb > vApp)
            //    // Hay una nueva versión en la web
            //    return 1;
            //else if (vWeb < vApp)
            //    // Esta es la versión más reciente
            //    return -1;
            //else
            //    // Son iguales
            //    return 0;
        }

        /// <summary>
        /// Comprueba la versión de la aplicación indicada y la compara con la indicada en versionActual.
        /// Si la version en la web es mayor, devuelve 1, -1 si es menor, 0 si son iguales.
        /// </summary>
        /// <param name="ensamblado">Referencia al ensamblado a comprobar.
        /// Usar: System.Reflection.Assembly.GetExecutingAssembly()</param>
        /// <param name="laVersionWeb">Aquí asignará la versión que hay en la Web.</param>
        /// <returns>Si la version en la web es mayor, devuelve 1, -1 si es menor, 0 si son iguales.</returns>
        public static int CompararVersionWeb(Assembly ensamblado, ref string laVersionWeb)
        {
            // Para comprobar las versiones usando el ensamblado de llamada
            var fvi = FileVersionInfo.GetVersionInfo(ensamblado.Location);
            return CompararVersionWeb(fvi.ProductName, fvi.FileVersion, ref laVersionWeb);
        }
    }
}
