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
	public partial class AcercaDegsNotasNET : ContentPage
	{
		public AcercaDegsNotasNET ()
		{
			InitializeComponent ();
			Title = $"{App.AppName} {App.AppVersion}";
			LabelTitulo.Text = $"Acerca de... {App.AppName} {App.AppVersion}";
		}
		private void btnPrivacidad_Clicked(object sender, EventArgs e)
		{
			_ = App.MostrarPoliticaPrivacidad();
		}
	}
}