using System;
using System.IO;
using Xamarin.Forms;
using gsNotasNET.Data;

namespace gsNotasNET
{
    public partial class App : Application
    {
        /// <summary>
        /// La versión de la aplicación
        /// </summary>
        public static string AppVersion { get; } = "v1..18";

        /// <summary>
        /// Los datos de las credenciales obtenidas del directorio Assets
        /// </summary>
        public static System.IO.Stream ClientSecretJson { get; set; }

        static NotasDatabase database;

        //static string DatabasePath;

        public static NotasDatabase Database
        {
            get
            {
                //DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3");

                if (database == null)
                {
                    //database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3"));
                    database = new NotasDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "gsNotasNET.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new NotesPage());
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