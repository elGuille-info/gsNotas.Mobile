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
        public Login()
        {
            InitializeComponent();
            Current = this;
        }

        private void btnAcceder_Clicked(object sender, EventArgs e)
        {
            UsuarioSQL.PasswordUsuario = password.Text;
            LabelInfo.IsVisible = false;

            if (UsuarioSQL.ComprobarContraseña(email.Text, password.Text))
            {
                AbrirPaginaPrincipal(email.Text);

                //// Asignar el usuario que se ha legueado
                //App.UsuarioLogin = UsuarioSQL.Usuario(email.Text);

                //// abrir la página principal
                //Application.Current.MainPage = new NavigationPage(new NotesPage());
            }
            else
            {
                // No es correcta
                LabelInfo.Text = "El usuario o el password no es correcto.";
                LabelInfo.IsVisible = true;
                email.Focus();
            }
        }

        private static void AbrirPaginaPrincipal(string email)
        {
            // Asignar el usuario que se ha legueado
            UsuarioSQL.UsuarioLogin = UsuarioSQL.Usuario(email);

            //Debug.WriteLine(App.UsuarioLogin.Email);

            // abrir la página principal
            //Application.Current.MainPage = new NavigationPage(new NotesPage());
            Current.Navigation.PushAsync(new NotesPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Title = $"gsNotasNET.Android {App.AppVersion}";

        }
    }
}