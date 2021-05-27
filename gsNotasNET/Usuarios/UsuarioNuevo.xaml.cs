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
    public partial class UsuarioNuevo : ContentPage
    {
        public UsuarioNuevo()
        {
            InitializeComponent();
        }

        async private void btnRegistrar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Email.Text) || string.IsNullOrEmpty(Password.Text) || string.IsNullOrEmpty(Nombre.Text))
            {
                // se deben indicar los tres valores
                LabelInfo.Text = "Debes indicar los tres valores solicitados.";
                LabelInfo.IsVisible = true;

                if (string.IsNullOrEmpty(Email.Text))
                    Email.Focus();
                else if (string.IsNullOrEmpty(Password.Text))
                    Password.Focus();
                else if (string.IsNullOrEmpty(Nombre.Text))
                    Nombre.Focus();
            }
            else
            {
                // Comprobar si ya existe ese usuario
                if (UsuarioSQL.Existe(Email.Text))
                {
                    LabelInfo.Text = "Debes indicar una cuenta de correo no registrada.";
                    LabelInfo.IsVisible = true;
                    Email.Focus();
                    return;
                }
                // registrarlo
                var usuario = new UsuarioSQL();
                usuario.Email = Email.Text;
                usuario.Nombre = Nombre.Text;
                UsuarioSQL.GuardarUsuario(usuario, Password.Text);
                // Enviarlo a Login
                await Navigation.PushAsync(new Login(new MainMenu()));
            }
        }

        async private void btnCancelar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login(new MainMenu()));
        }

        //private void btnPrivacidad_Clicked(object sender, EventArgs e)
        //{
        //    _ = App.MostrarPoliticaPrivacidad();
        //}

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            LabelStatus.Text = App.StatusInfo;
            //btnPayPal.IsVisible = App.MostrarPayPal;
        }
        private void btnPayPal_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarDonativoPayPal();
        }
    }
}