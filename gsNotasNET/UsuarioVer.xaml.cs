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
        public UsuarioVer()
        {
            InitializeComponent();
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
            Title = $"{App.AppName} {App.AppVersion}";
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}