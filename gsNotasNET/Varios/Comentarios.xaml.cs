using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Comentarios : ContentPage
    {
        public static Comentarios Current;
        public Comentarios()
        {
            InitializeComponent();
            Current = this;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            LabelStatus.Text = App.StatusInfo;
            btnPayPal.IsVisible = App.MostrarPayPal;
            btnInicio.IsVisible = (DeviceInfo.Platform == DevicePlatform.UWP);
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
        async private void btnInicio_Clicked(object sender, EventArgs e)
        {
            // Volver a la anterior
            await Current.Navigation.PopAsync();
            // Volver a la principal
            //await Current.Navigation.PopToRootAsync();
        }
        private void btnPayPal_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarDonativoPayPal();
        }
    }
}