using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.APIs;

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

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
			// Esto solo funciona en DEBUG
			//if (System.Diagnostics.Debugger.IsAttached)
			{
				System.Reflection.Assembly ensamblado = typeof(AcercaDegsNotasNET).Assembly;
				//var ensamblado = System.Reflection.Assembly.GetExecutingAssembly();

				var versionWeb = "xx";
				string msgVersion;

				//var cualVersion = VersionUtilidades.CompararVersionWeb(ensamblado, ref versionWeb);
				var cualVersion = VersionUtilidades.CompararVersionWeb(App.AppName, App.AppVersionFull, ref versionWeb);

				if (cualVersion == 1)
					msgVersion = $"Existe una versión más reciente de '{App.AppName}': v{versionWeb}.";
				else //if (cualVersion == -1)
					msgVersion = $"Esta versión de '{App.AppName}' es la más reciente.";

				LabelVersion.Text = msgVersion;
			}
		}
	}
}