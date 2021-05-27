using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsNotasNET.Models;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gsNotasNET
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CopiarNotas : ContentPage
	{
		public CopiarNotas ()
		{
			InitializeComponent ();
		}

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
			LabelStatus.Text = App.StatusInfo;
			btnInicio.IsVisible = (DeviceInfo.Platform == DevicePlatform.UWP);

			var id = UsuarioSQL.UsuarioLogin.ID;
			LabelNotas2048.Text = NotaSQL.CountNotas2048(id).ToString("#,##0");
			LabelNotasMax.Text = NotaSQL.CountNotasMax(id).ToString("#,##0");
		}

        private void btnCopiarNotas_Clicked(object sender, EventArgs e)
        {
			// Copiar las notas de la tabla normal (Notas) a NotasMax.
			var id = UsuarioSQL.UsuarioLogin.ID;
			string sel;
			if (chkArchivadas.IsToggled && chkEliminadas.IsToggled)
				sel = "Archivada = 1 AND Eliminada = 1";
			else if (chkArchivadas.IsToggled)
				sel = "Archivada = 1 AND Eliminada = 0";
			else if (chkEliminadas.IsToggled)
				sel = "Archivada = 0 AND Eliminada = 1";
			else
				sel = "Archivada = 0 AND Eliminada = 0";
			var col = NotaSQL.CopiarNotas20482NotasMax(id, sel);
			if(col.StartsWith("ERROR") == false)
				LabelError.BackgroundColor = Color.Green;
			LabelError.Text = col;
			LabelError.IsVisible = true;
		}

        private void btnCopiarNotasMax2Notas_Clicked(object sender, EventArgs e)
        {
			// Copiar las notas de la tabla NotasMax a la normal (Notas).
			var id = UsuarioSQL.UsuarioLogin.ID;
			string sel;
			if (chkArchivadas.IsToggled && chkEliminadas.IsToggled)
				sel = "Archivada = 1 AND Eliminada = 1";
			else if (chkArchivadas.IsToggled)
				sel = "Archivada = 1 AND Eliminada = 0";
			else if (chkEliminadas.IsToggled)
				sel = "Archivada = 0 AND Eliminada = 1";
			else
				sel = "Archivada = 0 AND Eliminada = 0";
			var col = NotaSQL.CopiarNotasMax2Notas2048(id, sel);
			if (col.StartsWith("ERROR") == false)
				LabelError.BackgroundColor = Color.Green;
			LabelError.Text = col;
			LabelError.IsVisible = true;
		}
		async private void btnInicio_Clicked(object sender, EventArgs e)
		{
			// Volver a la anterior
			await Navigation.PopAsync();
		}
	}
}