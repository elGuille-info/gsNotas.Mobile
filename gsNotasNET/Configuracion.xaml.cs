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
    public partial class Configuracion : ContentPage
    {
        public static Configuracion Current;

        public Configuracion()
        {
            InitializeComponent();
            Current = this;

            Title = $"Configuración - {App.AppName} {App.AppVersion}";
        }

        private void btnAplicar_Clicked(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null)
            {
                Navigation.PushAsync(new Login(Current));
                return;
            }

            App.RecordarPassword = chkRecordarPassword.IsToggled;
            App.RecordarUsuario = chkRecordarUsuario.IsToggled;
            App.UltimoUsuario = LabelUsuario.Text;
            if (App.RecordarPassword)
                App.UltimoPassword = UsuarioSQL.UsuarioLogin.Password;// .PasswordUsuario;
            else
                App.UltimoPassword = "";

            //Navigation.PushAsync(new Login(Current));
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null)
            {
                Navigation.PushAsync(new Login(Current));
                return;
            }

            chkRecordarPassword.IsToggled = App.RecordarPassword;
            chkRecordarUsuario.IsToggled = App.RecordarUsuario;
            if (string.IsNullOrEmpty(App.UltimoUsuario))
                App.UltimoUsuario = UsuarioSQL.UsuarioLogin.Email;
            LabelUsuario.Text = App.UltimoUsuario;
        }
    }
}