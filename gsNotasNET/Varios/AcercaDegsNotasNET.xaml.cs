using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.APIs;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AcercaDegsNotasNET : ContentPage
	{
		public static AcercaDegsNotasNET Current;
		public AcercaDegsNotasNET ()
		{
			InitializeComponent ();
			Current = this;
		}

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
			Title = $"Acerca de... {App.AppName}";
			LabelTitulo.Text = $"Acerca de... {App.AppName} {App.AppVersion}";
			LabelStatus.Text = App.StatusInfo;
			btnInicio.IsVisible = (DeviceInfo.Platform == DevicePlatform.UWP);
		}
		async private void btnInicio_Clicked(object sender, EventArgs e)
		{
			// Volver a la anterior
			await Current.Navigation.PopAsync();
		}
		private void btnPayPal_Clicked(object sender, EventArgs e)
		{
			_ = App.MostrarDonativoPayPal();
		}
        async private void btnWeb_Clicked(object sender, EventArgs e)
        {
			// Mostrar el sitio de gsNotas.Mobile					(26/May/21)
			var uri = new Uri("https://www.elguillemola.com/utilidades-net/gsnotasnet-android-multiplataforma-movil/");
			await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
		}
	}
}