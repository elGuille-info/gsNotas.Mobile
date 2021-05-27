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
    public enum NotasDatosMostrar
    {
        Activas,
        Archivadas,
        Eliminadas,
        Favoritas,
        Notificar,
        Local
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotasMostrar : ContentPage
    {
        //private static NotasArchivadas Current;

        private List<NotaSQL> colNotas = null;

        private NotasDatosMostrar DatosMostrar = NotasDatosMostrar.Archivadas;

        public NotasMostrar(NotasDatosMostrar datosMostrar)
        {
            InitializeComponent();
            //Current = this;
            //Title = $"{App.AppName} {App.AppVersion}";

            Title = $"Notas {datosMostrar}";
            
            DatosMostrar = datosMostrar;
        }

        async private void ContentPage_Appearing(object sender, EventArgs e)
        {
            LabelStatus.Text = App.StatusInfo;

            if (UsuarioSQL.UsuarioLogin is null || UsuarioSQL.UsuarioLogin.ID == 0 || UsuarioSQL.UsuarioLogin.Email == "prueba")
            {
                await Navigation.PushAsync(new Login(this));
                return;
            }
            var sDatos = "archivada";
            switch (DatosMostrar)
            {
                case NotasDatosMostrar.Archivadas:
                    sDatos = "archivada";

                    // Solo las notas archivadas y no eliminadas
                    if (App.UsarNotasLocal)
                        colNotas = App.Database.NotasArchivadas();
                    else
                        colNotas = NotaSQL.NotasUsuario(UsuarioSQL.UsuarioLogin.ID, archivadas: true);

                    break;
                case NotasDatosMostrar.Eliminadas:
                    sDatos = "eliminada";

                    // Solo las notas eliminadas
                    if (App.UsarNotasLocal)
                        colNotas = App.Database.NotasEliminadas();
                    else
                        colNotas = NotaSQL.NotasUsuario(UsuarioSQL.UsuarioLogin.ID, eliminadas: true);

                    break;
                case NotasDatosMostrar.Favoritas:
                    sDatos = "favorita";

                    // Solo las notas favoritas
                    if (App.UsarNotasLocal)
                        colNotas = App.Database.NotasFavoritas();
                    else
                        colNotas = NotaSQL.NotasFavoritas(UsuarioSQL.UsuarioLogin.ID);

                    break;
                case NotasDatosMostrar.Notificar:
                    sDatos = "a notificar.";

                    // Las notas a notificar que no estén eliminadas
                    if (App.UsarNotasLocal)
                        colNotas = App.Database.NotasNotificar();
                    else
                        colNotas = NotaSQL.Buscar(UsuarioSQL.UsuarioLogin.ID, "Notificar = 1 AND Eliminada = 0");

                    break;

                case NotasDatosMostrar.Local:
                    sDatos = "de la base local.";

                    colNotas = App.Database.NotasTodas();
                    break;

                default:
                    break;
            }
            listView.ItemsSource = colNotas;

            var plural = colNotas.Count() == 1 ? "" : "s";
            var plural2 = plural + ".";
            if (sDatos.EndsWith("."))
                plural2 = "";

            //LabelInfo.Text = $"{UsuarioSQL.UsuarioLogin.Email} con {colNotas.Count()} nota{plural} archivada{plural}.";
            LabelInfo.Text = $"{UsuarioSQL.UsuarioLogin.Email} con {colNotas.Count()} nota{plural} {sDatos}{plural2}";
        }

        //private void btnPrivacidad_Clicked(object sender, EventArgs e)
        //{
        //    _ = App.MostrarPoliticaPrivacidad();
        //}

        async private void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // No editar si se muestran las locales
            if (e.SelectedItem != null && this.DatosMostrar != NotasDatosMostrar.Local)
            {
                await Navigation.PushAsync(new NotaEditar
                {
                    DatosMostrar = this.DatosMostrar,
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