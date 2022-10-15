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
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            //Title = $"{App.AppName} {App.AppVersion}";
            LabelStatus.Text = App.StatusInfo;

            chkRecordarPassword.IsToggled = App.RecordarPassword;
            chkRecordarUsuario.IsToggled = App.RecordarUsuario;

            var modoDebug = false;

            //// por ahora no usar esto, para probarlo como en el dispositivo final
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    //modoDebug = true;
            //    modoDebug = false;
            //}

            if (App.RecordarUsuario)
                email.Text = App.UltimoUsuario;
            else
            {
                if (modoDebug)
                    email.Text = Usuario;
                else
                    email.Text = "prueba";
            }
            if (App.RecordarPassword)
                password.Text = App.UltimoPassword;
            else
            {
                if (modoDebug)
                    password.Text = Password;
                else
                    password.Text = "";
            }
        }

        async private void btnAcceder_Clicked(object sender, EventArgs e)
        {
            LabelInfo.IsVisible = false;

            //App.UsarNotasMaxConfig = false;

            if (UsuarioSQL.ComprobarContraseña(email.Text, password.Text))
            {
                // Guardar el último usuario que accede
                App.UltimoUsuario = UsuarioSQL.UsuarioLogin.Email;
                App.UltimoPassword = UsuarioSQL.UsuarioLogin.Password;
                // Usar las opciones indicadas                      (26/May/21)
                App.RecordarPassword = chkRecordarPassword.IsToggled;
                App.RecordarUsuario = chkRecordarUsuario.IsToggled;

                // si es el usuario de prueba, no hacer nada.
                if (UsuarioSQL.UsuarioLogin.Email.ToLower() == "prueba")
                {
                    LabelInfo.Text = "Has indicado el usuario de prueba. " +
                        "Te recuerdo que estas notas estarán visibles a todos los que entren con estas credenciales.";
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
                }
                else
                {
                    // Guardar los datos de último acceso, etc.
                    UsuarioSQL.UsuarioLogin.UltimoAcceso = DateTime.UtcNow;
                    UsuarioSQL.UsuarioLogin.VersionPrograma = $"{App.AppName} {App.AppVersion}";
                    // Por si accede después de usar el usuario local (23/May/21)
                    App.UsarNotasLocal = false;
                    UsuarioSQL.GuardarUsuario(UsuarioSQL.UsuarioLogin);

                    // si se deben mostrar las notas a notificar
                    if (App.Notificar)
                    {
                        // Comprobar si hay notas a notificar
                        var colNotificar = NotaSQL.Buscar(UsuarioSQL.UsuarioLogin.ID, "Notificar = 1 AND Eliminada = 0");
                        if (colNotificar.Count() == 0)
                        {
                            App.GuardarConfig();
                            VolverAMain();
                            return;
                        }
                        // Mostrar la ventana de las notas marcadas para notificar
                        await Navigation.PushAsync(new NotasMostrar(NotasDatosMostrar.Notificar));
                        
                        //return;
                    }
                }
                App.GuardarConfig();
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

        private void btnAccederSinConexion_Clicked(object sender, EventArgs e)
        {
            // Usar la base local, por si no hay conexión
            App.AsignarUsuarioLocal();

            VolverAMain();
        }

        async private void VolverAMain()
        {
            if (_pagina == null)
            {
                //Application.Current.MainPage = new NavigationPage(new MainMenu());
                await Navigation.PopToRootAsync();
            }
            else
            {
                try
                {
                    await Navigation.PushAsync(_pagina);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
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