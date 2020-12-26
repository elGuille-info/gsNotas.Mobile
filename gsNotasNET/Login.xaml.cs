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
            Title = $"{App.AppName} {App.AppVersion}";

            if (App.RecordarUsuario)
                email.Text = App.UltimoUsuario;
            else
            {
#if false
                email.Text = Usuario;
#else
                email.Text = "prueba";
#endif
            }
            if (App.RecordarPassword)
                password.Text = App.UltimoPassword;
            else
            {
#if false
                password.Text = Password;
#else
                password.Text = "";
#endif
            }
            //#if true // Para probar sin tener que indicar mi usuario y password
            //            email.Text = Usuario;
            //            password.Text = Password;

            //#else
            //            email.Text = "prueba";
            //            password.Text = "1234";
            //#endif
        }

        async private void btnAcceder_Clicked(object sender, EventArgs e)
        {
            LabelInfo.IsVisible = false;

            if (UsuarioSQL.ComprobarContraseña(email.Text, password.Text))
            {
                // Guardar el último usuario que accede
                App.UltimoUsuario = UsuarioSQL.UsuarioLogin.Email;
                App.UltimoPassword = UsuarioSQL.UsuarioLogin.Password;

                // si es el usuario de prueba, no hacer nada.
                if (UsuarioSQL.UsuarioLogin.Email.ToLower() == "prueba")
                {
                    LabelInfo.Text = "Has indicado el usuario de prueba. Te recuerdo que estas notas estarán visibles a todos los que entren con estas credenciales.";
                    LabelInfo.IsVisible = true;
                    return;
                }
                // Avisar si no está validado
                if (!UsuarioSQL.UsuarioLogin.Validado)
                {
                    await App.CodigoValidación(UsuarioSQL.UsuarioLogin.Email);
                    var minutos = 60 - DateTime.UtcNow.Minute;
                    string plural = (minutos == 1) ? "" : "s";
                    LabelInfo.Text = "Aún no has validado tu email. " +
                        "Te he enviado un correo con el código de validación. " +
                        $"Úsalo en la página de validar antes de {minutos} minuto{plural}. Gracias.";
                    LabelInfo.IsVisible = true;
                    //VolverAMain();
                    //await Navigation.PushAsync(new MainMenu());
                    //Application.Current.MainPage = new NavigationPage(new MainMenu());
                    //return;
                }
                else
                {
                    if (!UsuarioSQL.UsuarioLogin.NotasCopiadas)
                    {
                        // Si se han copiado las notas de SQL Lite.
                        // Hasta que no esté validado no se copiarán.
                        await Navigation.PushAsync(new CopiarSQLLite(_pagina));
                        return;
                    }
                }
                VolverAMain();
            }
            else
            {
                LabelInfo.Text = "El usuario y/o el password no son correctos.";
                LabelInfo.IsVisible = true;
                email.Focus();
                return;
            }
        }

        async private void VolverAMain()
        {
            if (_pagina is null)
            {
                Application.Current.MainPage = new NavigationPage(new MainMenu());
            }
            else
            {
                try
                {
                    await Navigation.PushAsync(_pagina);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    _pagina = null;
                    VolverAMain();
                }
            }
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