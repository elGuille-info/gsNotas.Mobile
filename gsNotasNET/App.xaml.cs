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
        public static string AppVersion { get; } = "v2..01";

        /// <summary>
        /// El usuario que ha hecho login
        /// </summary>
        public static UsuarioSQL UsuarioLogin { get; set; }

        /// <summary>
        /// El password usado para hacer Loging.
        /// </summary>
        public static string PasswordUsuario { get; set; }

        /// <summary>
        /// Los datos de las credenciales obtenidas del directorio Assets
        /// </summary>
        public static System.IO.Stream ClientSecretJson { get; set; }

        /// <summary>
        /// Muestra la página de la política de privacidad en el navegador predeterminado.
        /// </summary>
        /// <returns></returns>
        public static async Task MostrarPoliticaPrivacidad()
        {
            var uri = new Uri("http://www.elguillemola.com/politica-de-privacidad/");
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }

        //public static 

        //static NotasDatabase database;

        //static string DatabasePath;

        //public static NotasDatabase Database
        //{
        //    get
        //    {
        //        //DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3");

        //        if (database == null)
        //        {
        //            //database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3"));
        //            database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "gsNotasNET.db3"));
        //        }
        //        return database;
        //    }
        //}

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