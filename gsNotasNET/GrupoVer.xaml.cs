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
    public partial class GrupoVer : ContentPage
    {
        public GrupoVer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto el grupo está asignado.
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var grupo = (Grupo)BindingContext;
            //if (grupo is null)
            //    return;

            LabelInfo.Text = grupo.Nombre;
            Title =$"{App.AppName} {App.AppVersion}";
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}