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
    public partial class UsuarioVer : ContentPage
    {
        public static UsuarioVer Current;
        public UsuarioVer()
        {
            InitializeComponent();
            Current = this;
            //Title = $"{App.AppName} {App.AppVersion}";
            LabelInfo.Text = "";
        }

        async private void ContentPage_Appearing(object sender, EventArgs e)
        {
            LabelStatus.Text = App.StatusInfo;

            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                await Navigation.PushAsync(new Login(Current));
                LabelInfo.Text = "No hay usuario logueado.";
                return;
            }
            var usuario = (UsuarioSQL)BindingContext;
            LabelInfo.Text = usuario.Email;
        }

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto el usuario está asignado.
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var usuario = (UsuarioSQL)BindingContext;
            //if (usuario is null)
            //    return;

            LabelInfo.Text = usuario.Email;
        }

        //private void btnPrivacidad_Clicked(object sender, EventArgs e)
        //{
        //    _ = App.MostrarPoliticaPrivacidad();
        //}
    }
}