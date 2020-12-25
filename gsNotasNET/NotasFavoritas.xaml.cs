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
    public partial class NotasFavoritas : ContentPage
    {
        private static NotasFavoritas Current;

        public NotasFavoritas()
        {
            InitializeComponent();
            Current = this;
        }

        async private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null)
            {
                await Navigation.PushAsync(new Login(Current));
                return;
            }
            // Solo las notas archivadas y no eliminadas
            listView.ItemsSource = NotaSQL.NotasFavoritas(UsuarioSQL.UsuarioLogin.ID);
            TituloNotas();
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

        public static void TituloNotas()
        {
            Current.Title = $"{App.AppName} {App.AppVersion}";
            Current.LabelInfo.Text = $"{UsuarioSQL.UsuarioLogin.Email} - con {NotaSQL.CountFavoritas(UsuarioSQL.UsuarioLogin.ID)} notas favoritas."; ;
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