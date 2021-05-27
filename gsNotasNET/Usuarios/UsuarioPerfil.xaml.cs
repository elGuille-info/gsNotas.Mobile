using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.APIs;
using gsNotasNET.Models;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsuarioPerfil : ContentPage
    {
        public static UsuarioPerfil Current;
        public UsuarioPerfil(UsuarioSQL usuario)
        {
            InitializeComponent();
            Current = this;
            //Title = $"{App.AppName} {App.AppVersion}";
            _usuario = usuario;
            if (usuario is null)
            {
                LabelInfo.Text = "El usuario indicado no es válido.";
            }
            else
            {
                BindingContext = usuario;
                LabelInfo.Text = $"Perfil de {usuario.Email}";
            }
        }

        private static UsuarioSQL _usuario;
        public static bool Validado { get; set; } = false;

        async private void ContentPage_Appearing(object sender, EventArgs e)
        {
            LabelStatus.Text = App.StatusInfo;
            btnPayPal.IsVisible = App.MostrarPayPal;
            btnInicio.IsVisible = (DeviceInfo.Platform == DevicePlatform.UWP);

            // UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.Email == "prueba"
            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0)
            {
                await Navigation.PushAsync(new Login(Current));
                LabelInfo.Text = "";
                return;
            }

            // si es el usuario de prueba no permitir cambios
            // ni si es el usuario local                            (24/May/21)
            if (UsuarioSQL.UsuarioLogin.Email.ToLower() == "prueba" || UsuarioSQL.UsuarioLogin.Email.ToLower() == "local")
            {
                LabelAviso.Text = $"El usuario '{UsuarioSQL.UsuarioLogin.Email}' no puede modificar su perfil.";
                LabelAviso.IsVisible = true;
                btnGuardar.IsEnabled = false;
                Password.IsEnabled = false;
                Email.IsEnabled = false;
                ClaveSHA.IsEnabled = false;
                //chkNotasCopiadas.IsEnabled = false;
                chkValidado.IsEnabled = false;
                Nombre.IsEnabled = false;
            }
            txtPagos.Text = UsuarioSQL.UsuarioLogin.Pagos.ToString("#,##0.## €");
            //LabelUsarNotasMax.Text = UsuarioSQL.UsuarioLogin.Pagos >= 25 ? "Sí" : "No";
            LabelUsarNotasMax.Text = UsuarioSQL.UsuarioLogin.UsarNotasMax ? "Sí" : "No";
        }

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto el usuario está asignado.
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            //LabelAviso.IsVisible = false;

            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0)
                return;

            // si es el usuario de prueba no permitir cambios
            // ni si es el usuario local                            (24/May/21)
            if (UsuarioSQL.UsuarioLogin.Email.ToLower() == "prueba" || UsuarioSQL.UsuarioLogin.Email.ToLower() == "local")
            {
                LabelAviso.Text = $"El usuario '{UsuarioSQL.UsuarioLogin.Email}' no puede modificar su perfil.";
                LabelAviso.IsVisible = true;
                btnGuardar.IsEnabled = false;
                Password.IsEnabled = false;
                Email.IsEnabled = false;
                ClaveSHA.IsEnabled = false;
                //chkNotasCopiadas.IsEnabled = false;
                chkValidado.IsEnabled = false;
                Nombre.IsEnabled = false;
                return;
            }
            var usuario = (UsuarioSQL)BindingContext;
            //if (usuario is null)
            //    return;

            txtCuota.Text = usuario.Cuota.ToString("#,##0");
            chkValidado.IsEnabled = (usuario.Email.ToLower().IndexOf("elguille.info@") > -1);
            LabelInfo.Text = $"Perfil de {usuario.Email}";
        }

        async private void btnInicio_Clicked(object sender, EventArgs e)
        {
            // Volver a la anterior
            await Current.Navigation.PopAsync();
            // Volver a inicio
            //await Current.Navigation.PopToRootAsync();
        }

        //private void btnPrivacidad_Clicked(object sender, EventArgs e)
        //{
        //    _ = App.MostrarPoliticaPrivacidad();
        //}

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            LabelAviso.IsVisible = false;

            var usuario = (UsuarioSQL)BindingContext;

            // no guardar el email en blanco
            if (string.IsNullOrEmpty(Email.Text))
            {
                LabelAviso.Text = "El Email no puede estar en blanco.";
                LabelAviso.IsVisible = true;
                Email.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Nombre.Text))
            {
                LabelAviso.Text = "El Nombre no puede estar en blanco.";
                LabelAviso.IsVisible = true;
                Nombre.Focus();
                return;
            }
            if (ClaveSHA.Text.ToUpper() != _usuario.ClaveSHA)
            {
                LabelAviso.Text = "La clave SHA no es modificable.";
                LabelAviso.IsVisible = true;
                ClaveSHA.Text = _usuario.ClaveSHA;
                ClaveSHA.Focus();
                return;
            }

            // Si cambia de email no se guardan los cambios.
            if (usuario.Email.ToLower() != _usuario.Email.ToLower())
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Hola {_usuario.Nombre},");
                sb.AppendLine($"Has solicitado cambiar el email usado en el programa: '{_usuario.Email}',");
                sb.AppendLine($"por otro diferente: '{usuario.Email}'.");
                sb.AppendLine();
                sb.AppendLine("Si no has sido tú, seguramente deberías cambiar el password porque alguien ha accedido con tus datos.");
                sb.AppendLine();
                sb.AppendLine("Si has sido tú, te mando otro mensaje a la nueva cuenta que quieres usar.");
                sb.AppendLine("Por favor, confirma (respondiendo a los dos mensajes) que es correcto ese cambio.");
                sb.AppendLine("También indicame el nuevo password a usar cuando respondas al mensaje recibido en la nueva cuenta de email.");
                sb.AppendLine("Después podrás cambiarlo usando esta misma página del Perfil del usuario.");
                sb.AppendLine();
                sb.AppendLine("Gracias.");
                sb.AppendLine("Guillermo");
                sb.AppendLine("---------");
                sb.AppendLine($"{App.AppName} {App.AppVersion}");
                await App.SendEmail("Cambio de email", sb.ToString(), _usuario.Email);

                sb.Clear();
                sb.AppendLine($"Hola {_usuario.Nombre},");
                sb.AppendLine($"El usuario de la aplicación {App.AppName} con email '{_usuario.Email}' ha solicitado cambiar el email");
                sb.AppendLine($"por este al que te mando este correo: '{usuario.Email}'.");
                sb.AppendLine();
                sb.AppendLine("Si no has sido tú, por favor, indícamelo y procederé como vea conveniente.");
                sb.AppendLine();
                sb.AppendLine("Si has sido tú, te he mandado otro mensaje a la cuenta desde la que has solicitado el cambio.");
                sb.AppendLine("Por favor, confirma (respondiendo a los dos mensajes) que es correcto ese cambio.");
                sb.AppendLine("También indicame aquí el nuevo password a usar.");
                sb.AppendLine("Después podrás cambiarlo usando esta misma página del Perfil del usuario.");
                sb.AppendLine();
                sb.AppendLine("Gracias.");
                sb.AppendLine("Guillermo");
                sb.AppendLine("---------");
                sb.AppendLine($"{App.AppName} {App.AppVersion}");
                await App.SendEmail("Cambio de email", sb.ToString(), usuario.Email);

                btnGuardar.IsEnabled = false;
                LabelAviso.Text = "Has indicado un nuevo email. No se guardan los cambios. Responde a los 2 emails enviados. Gracias.";
                LabelAviso.IsVisible = true;
                Email.Focus();
            }
            else
            {
                // Guardar los cambios y asignar el usuario actual con los nuevos datos.
                if (Password.Text.Any())
                {
                    // Cambiar el password y guardar la nueva clave SHA
                    usuario.ClaveSHA = UsuarioSQL.GenerarClaveSHA1(usuario.Email, Password.Text);
                    usuario.Password = Password.Text;
                    App.UltimoPassword = usuario.Password;
                }
                UsuarioSQL.GuardarUsuario(usuario);
                UsuarioSQL.UsuarioLogin = usuario;
                App.UltimoUsuario = usuario.Email;
                LabelAviso.Text = "Se han guardado correctamente los nuevos datos.";
                LabelAviso.IsVisible = true;
            }
        }
        private void btnPayPal_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarDonativoPayPal();
        }
    }
}