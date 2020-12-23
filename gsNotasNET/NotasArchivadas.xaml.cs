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
    public partial class NotasArchivadas : ContentPage
    {
        private static NotasArchivadas Current;
        public NotasArchivadas()
        {
            InitializeComponent();
            Current = this;
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    if (UsuarioSQL.UsuarioLogin is null)
        //    {
        //        Navigation.PushAsync(new Login(Current));
        //        return;
        //    }
        //    listView.ItemsSource = NotaSQL.NotasUsuario(UsuarioSQL.UsuarioLogin.ID, true);
        //}

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (UsuarioSQL.UsuarioLogin is null)
            {
                Navigation.PushAsync(new Login(Current));
                return;
            }
            // Solo las notas archivadas y no eliminadas
            listView.ItemsSource = NotaSQL.NotasUsuario(UsuarioSQL.UsuarioLogin.ID, todas: false, archivadas: true);
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
                await Navigation.PushAsync(new EditarNota
                {
                    BindingContext = e.SelectedItem as NotaSQL
                });
            }
        }

        public static void TituloNotas()
        {
            Current.Title = $"{App.AppName} {App.AppVersion}";
            Current.LabelInfo.Text = $"{UsuarioSQL.UsuarioLogin.Email} - con {NotaSQL.CountArchivadas(UsuarioSQL.UsuarioLogin.ID)} notas archivadas."; ;
        }
    }
}