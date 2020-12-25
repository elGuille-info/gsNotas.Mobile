using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Comentarios : ContentPage
    {
        public Comentarios()
        {
            InitializeComponent();
        }

        async private void btnEnviar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Comentario.Text))
            {
                LabelInfo.Text = "Debes indicar algún comentario.";
                LabelInfo.IsVisible = true;
                Comentario.Focus();
                return;
            }
            // Enviar el comentario
            await App.SendEmailUserLocal($"Comentario a elGuille sobre {App.AppName} {App.AppVersion}", Comentario.Text, "correos.elguille.info@gmail.com");
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}