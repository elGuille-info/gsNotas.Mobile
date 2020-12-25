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
	public partial class NotasEliminadas : ContentPage
	{
        private static NotasEliminadas Current;
        public NotasEliminadas ()
		{
			InitializeComponent ();
            Current = this;
            Title = $"{App.AppName} {App.AppVersion}";
        }

        async private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null)
            {
                await Navigation.PushAsync(new Login(Current));
                return;
            }
            // Solo las notas eliminadas
            listView.ItemsSource = NotaSQL.NotasUsuario(UsuarioSQL.UsuarioLogin.ID, eliminadas: true);

            Current.LabelInfo.Text = $"{UsuarioSQL.UsuarioLogin.Email} - con {NotaSQL.CountEliminadas(UsuarioSQL.UsuarioLogin.ID)} notas eliminadas.";
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }

        async private void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NotaEditar
                {
                    BindingContext = e.SelectedItem as NotaSQL
                });
            }
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}