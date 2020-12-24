using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.APIs;
using gsNotasNET.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        async private void btnAcceder_Clicked(object sender, EventArgs e)
        {
            LabelInfo.IsVisible = false;

            UsuarioSQL.ComprobarContraseña(email.Text, password.Text);

            if (UsuarioSQL.UsuarioLogin.ID != 0)
            {
                await Navigation.PushAsync(new CopiarSQLLite(_pagina));

                // Si no está validado, enviar código de validación
                if (!UsuarioSQL.UsuarioLogin.Validado)
                {
                    await DialogService.ShowErrorAsync("Validar email.",
                                                      $"Aún no has validado el correo.{App.crlf}" +
                                                      $"Debes indicar el código de validación{App.crlf}" +
                                                      $"(enviado a tu email){App.crlf}" +
                                                      "para usar la aplicación.",
                                                      "ACEPTAR", UsuarioValidar.CallBackAfertHide);
                }
            }
            else
            {
                // No es correcta o existe

                // Preguntar si quiere registrarse
                // No, simplemente mostrar el mensaje y que se registre si quiere
                //if (UsuarioSQL.UsuarioLogin.ID == 0)
                //{
                //    // Enviarlo a la página de registro???
                //    // 
                //    bool res = await DisplayAlert("Registrarte",
                //                      $"¿Quieres registrarte para usar esta aplicación?",
                //                      "SI", "NO");
                //    if (res == false)
                //    {
                //        LabelInfo.Text = "Debes indicar un usuario y password correctos.";
                //        LabelInfo.IsVisible = true;
                //        email.Focus();
                //        return;
                //    }
                //    else
                //    {
                //        await Navigation.PushAsync(new UsuarioPerfil(UsuarioSQL.UsuarioLogin));
                //    }
                //}
                LabelInfo.Text = "El usuario y/o el password no son correctos.";
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

        async private void btnNuevoUsuario_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UsuarioNuevo());
        }
    }
}