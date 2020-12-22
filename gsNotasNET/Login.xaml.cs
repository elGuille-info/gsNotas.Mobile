using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//using System.IO;
//using gsNotasNET.Data;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using Xamarin.Essentials;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public static Login Current;
        public static string Usuario;
        public static string Password;

        private ContentPage _pagina;
        public Login(ContentPage pagina = null)
        {
            InitializeComponent();
            Current = this;
            _pagina = pagina;



#if true // Para probar sin tener que indicar mi usuario y password
            email.Text = Usuario;
            password.Text = Password;

#else
            email.Text = "prueba";
            password.Text = "1234";
#endif
        }

        private void btnAcceder_Clicked(object sender, EventArgs e)
        {
            LabelInfo.IsVisible = false;

            if (UsuarioSQL.ComprobarContraseña(email.Text, password.Text))
            {
                // si se quiere poder volver al Login
                //Current.Navigation.PushAsync(new ListaNotas());
                // Mostrarla sin páginas anteriores
                //Application.Current.MainPage = new NavigationPage(new ListaNotas());
                // Mostrar la página de comprobación de copia de notas anteriores
                //Application.Current.MainPage = new NavigationPage(new CopiarSQLLite(_pagina));
                Navigation.PushAsync(new CopiarSQLLite(_pagina));
            }
            else
            {
                // No es correcta
                LabelInfo.Text = "El usuario o el password no es correcto.";
                LabelInfo.IsVisible = true;
                email.Focus();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Title = $"{App.AppName} {App.AppVersion}";

        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}