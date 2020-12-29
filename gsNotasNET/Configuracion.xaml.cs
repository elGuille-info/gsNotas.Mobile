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
            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                Navigation.PushAsync(new Login(Current));
                return;
            }

            App.RecordarPassword = chkRecordarPassword.IsToggled;
            App.RecordarUsuario = chkRecordarUsuario.IsToggled;
            // Las notas siempre estarán sincronizadas
            App.SincronizarAuto = true; // chkSincronizarAuto.IsToggled;
            App.Notificar = chkNotificar.IsToggled;
            App.UltimoUsuario = LabelUsuario.Text;
            App.UsarNotasLocal = chkUsarLocal.IsToggled;
            if (App.RecordarPassword)
                App.UltimoPassword = UsuarioSQL.UsuarioLogin.Password;
            else
                App.UltimoPassword = "";
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                Navigation.PushAsync(new Login(Current));
                return;
            }
            chkRecordarPassword.IsToggled = App.RecordarPassword;
            chkRecordarUsuario.IsToggled = App.RecordarUsuario;
            chkSincronizarAuto.IsToggled = App.SincronizarAuto;
            chkNotificar.IsToggled = App.Notificar;
            chkUsarLocal.IsToggled = App.UsarNotasLocal;
            if (string.IsNullOrEmpty(App.UltimoUsuario))
                App.UltimoUsuario = UsuarioSQL.UsuarioLogin.Email;
            LabelUsuario.Text = App.UltimoUsuario;
        }
    }
}