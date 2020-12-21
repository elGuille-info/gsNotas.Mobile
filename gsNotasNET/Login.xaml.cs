using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnAcceder_Clicked(object sender, EventArgs e)
        {
            App.PasswordUsuario = password.Text;
            LabelInfo.IsVisible = false;

            if (UsuarioSQL.ComprobarContraseña(email.Text, password.Text))
            {
                // abrir la página principal
                App.UsuarioLogin = UsuarioSQL.Usuario(email.Text);
                Application.Current.MainPage = null;
                Application.Current.MainPage = new NavigationPage(new NotesPage());
            }
            else
            {
                // No es correcta
                LabelInfo.Text = "El usuario o el password no es correcto.";
                LabelInfo.IsVisible = true;
                email.Focus();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            int t = await UsuarioSQL.CountAsync();
            LabelUsuarios.Text = t.ToString();
        }
    }
}