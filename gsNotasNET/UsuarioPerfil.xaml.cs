using System;
using System.Collections.Generic;
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
    public partial class UsuarioPerfil : ContentPage
    {
        public UsuarioPerfil Current;
        public UsuarioPerfil(UsuarioSQL usuario)
        {
            InitializeComponent();
            Current = this;
            _usuario = usuario;
            BindingContext = usuario;
        }

        private static UsuarioSQL _usuario;
        public static bool Validado = false;

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto el usuario está asignado.
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var usuario = (UsuarioSQL)BindingContext;
            //if (usuario is null)
            //    return;

            LabelInfo.Text = usuario.Email;
            Title = $"{App.AppName} {App.AppVersion}";
        }

        //private void btnPrivacidad_Clicked(object sender, EventArgs e)
        //{
        //    _ = App.MostrarPoliticaPrivacidad();
        //}

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var usuario = (UsuarioSQL)BindingContext;

            if (usuario.ID == 0)
            {
                if (string.IsNullOrEmpty(Password.Text))
                {
                    await DisplayAlert("Indicar el password",
                                      $"Debes indicar el password para registrarte.",
                                      "ACEPTAR", "-");
                    await Navigation.PopAsync();
                    return;
                }
                // Crear el usuario
                int res = UsuarioSQL.GuardarUsuario(usuario, Password.Text);
                if(res == 0)
                {
                    await Navigation.PopAsync();
                    return;
                }
            }

            // no guardar el email en blanco
            if (string.IsNullOrEmpty(usuario.Email))
            {
                await Navigation.PopAsync();
                return;
            }

            if (usuario.Email != _usuario.Email)
            {
                // Enviar un código de confirmación a su email
                // y validarlo antes de seguir
                //await DisplayAlert("Cambio de email.", 
                //                  $"Has indicado otro email diferente.{App.crlf}" +
                //                  "Debes indicar el código de validación (enviado a ese email) para efectuar el cambio.",
                //                  "ACEPTAR","-");


                await DialogService.ShowErrorAsync("Cambio de email.",
                                                  $"Has indicado otro email diferente.{App.crlf}" +
                                                  "Debes indicar el código de validación (enviado a ese email) para efectuar el cambio.",
                                                  "ACEPTAR", UsuarioValidar.CallBackAfertHide);

                UsuarioValidar.ComprobarCodigoValidar();

                if (!Validado)
                    await Navigation.PopAsync();
                else
                {
                    usuario.Validado = true;
                    UsuarioSQL.GuardarUsuario(usuario);
                    await Navigation.PushAsync(UsuarioValidar.Current);
                }
                    
            }
            else
                Validado = true;

            // Solo cambiar el password si está validado
            // si no se cambia el email se considera validado
            if (Password.Text.Any() && Validado)
            {
                // Cambiar el password y generar la nueva clave SHA
                usuario.ClaveSHA = UsuarioSQL.GenerarClaveSHA1(usuario.Email, Password.Text);
            }
            if (!Validado)
            {
                // Esperar la confirmación de la validación.
                UsuarioValidar.CodigoValidar = App.CodigoValidación(usuario.Email).Result;
                UsuarioValidar.ComprobarCodigoValidar();
                //await Navigation.PushAsync(UsuarioValidar.Current);
                //await Navigation.PushAsync(new UsuarioValidar(App.CodigoValidación(usuario.Email).Result));
                if (!Validado)
                    await Navigation.PopAsync();
                else
                    await Navigation.PushAsync(UsuarioValidar.Current);
            }
            else
            {
                UsuarioSQL.GuardarUsuario(usuario);
                await Navigation.PopAsync();
            }
        }
    }
}