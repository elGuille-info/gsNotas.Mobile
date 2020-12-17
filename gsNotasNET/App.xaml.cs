using System;
using System.IO;
using Xamarin.Forms;
using gsNotasNET.Data;

namespace gsNotasNET
{
    public partial class App : Application
    {
        static NotasDatabase database;

        //static string DatabasePath;

        public static NotasDatabase Database
        {
            get
            {
                //DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gsNotasNET.db3");

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