using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.Models;

using Xamarin.Essentials;
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
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            //Title = $"Configuración - {App.AppName}";
            LabelStatus.Text = App.StatusInfo;
            btnPayPal.IsVisible = App.MostrarPayPal;
            btnInicio.IsVisible = (DeviceInfo.Platform == DevicePlatform.UWP);

            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                Navigation.PushAsync(new Login(Current));
                return;
            }
            if(UsuarioSQL.UsuarioLogin.Pagos < 25)
            {
                //App.UsarNotasMaxConfig = false;
                UsuarioSQL.UsuarioLogin.UsarNotasMax = false;
                chkUsarNotasMax.IsEnabled = false;
            }
            chkUsarNotasMax.IsToggled = UsuarioSQL.UsuarioLogin.UsarNotasMax;

            chkRecordarPassword.IsToggled = App.RecordarPassword;
            chkRecordarUsuario.IsToggled = App.RecordarUsuario;
            chkIniciarConUsuario.IsToggled = App.IniciarConUsuario;
            chkSincronizarAuto.IsToggled = App.SincronizarAuto;
            chkNotificar.IsToggled = App.Notificar;
            chkUsarLocal.IsToggled = App.UsarNotasLocal;
            if (string.IsNullOrEmpty(App.UltimoUsuario))
                App.UltimoUsuario = UsuarioSQL.UsuarioLogin.Email;
            LabelUsuario.Text = App.UltimoUsuario;
        }

        async private void btnAplicar_Clicked(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                await Navigation.PushAsync(new Login(Current));
                return;
            }

            App.RecordarPassword = chkRecordarPassword.IsToggled;
            App.RecordarUsuario = chkRecordarUsuario.IsToggled;
            App.IniciarConUsuario = chkIniciarConUsuario.IsToggled;
            // Las notas siempre estarán sincronizadas
            App.SincronizarAuto = true;
            App.Notificar = chkNotificar.IsToggled;
            App.UltimoUsuario = LabelUsuario.Text;
            App.UsarNotasLocal = chkUsarLocal.IsToggled;
            if (App.RecordarPassword)
                App.UltimoPassword = UsuarioSQL.UsuarioLogin.Password;
            else
                App.UltimoPassword = "";
            
            // Solo asignar/guardar si se ha podido cambiar         (26/May/21)
            if (UsuarioSQL.UsuarioLogin.Pagos >= 25)
            {
                UsuarioSQL.UsuarioLogin.UsarNotasMax = chkUsarNotasMax.IsToggled;
                UsuarioSQL.GuardarUsuario(UsuarioSQL.UsuarioLogin);
            }

            App.GuardarConfig();

            // Volver a la anterior
            await Current.Navigation.PopAsync();
        }

        private void btnPayPal_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarDonativoPayPal();
        }
        async private void btnInicio_Clicked(object sender, EventArgs e)
        {
            // Volver a la anterior
            await Current.Navigation.PopAsync();
        }
    }
}