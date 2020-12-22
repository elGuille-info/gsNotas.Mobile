using System;
using System.IO;
using Xamarin.Forms;
using gsNotasNET.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using gsNotasNET.Models;

namespace gsNotasNET
{
    public partial class App : Application
    {
        /// <summary>
        /// La versión de la aplicación
        /// </summary>
        public static string AppVersion { get; } = "v2..02";

        /// <summary>
        /// Los datos de las credenciales obtenidas del directorio Assets
        /// </summary>
        public static System.IO.Stream ClientSecretJson { get; set; }

        private static Stream _CredencialesSQL;
        /// <summary>
        /// Los datos para la conexión a la base de datos.
        /// No se incluye en el código fuente.
        /// </summary>
        /// <remarks>
        /// Tienes que crear un fichero llamado encrypted-string.txt,
        /// guardarlo en la carpeta Assets de la aplicación de Android,
        /// y guardar el usuario y el password, cada uno en una línea del fichero.
        /// </remarks>
        public static System.IO.Stream CredencialesSQL 
        {
            get { return _CredencialesSQL; }
            set 
            {
                // Al asignar el stream llamar a la asignación de la cadena de conexión.
                // De esta forma se usa el stream abierto una vez.
                _CredencialesSQL = value;
                NotasNETSQLDatabase.CadenaConexion = "";
            }
        }

        /// <summary>
        /// Muestra la página de la política de privacidad en el navegador predeterminado.
        /// </summary>
        /// <returns></returns>
        public static async Task MostrarPoliticaPrivacidad()
        {
            var uri = new Uri("http://www.elguillemola.com/politica-de-privacidad/");
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }

        //
        // Para acceder a la base de datos SQLLite
        //

        static NotasDatabase database;

        public static NotasDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            //MainPage = new NavigationPage(new NotesPage());
            MainPage = new NavigationPage(new Login());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}