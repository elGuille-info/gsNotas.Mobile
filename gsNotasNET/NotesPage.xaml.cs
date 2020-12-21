using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using gsNotasNET.Models;
using gsNotasNET.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace gsNotasNET
{
    public partial class NotesPage : ContentPage
    {
        public static NotesPage Current;
        public NotesPage()
        {
            InitializeComponent();
            Current = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemsSource = await NotaSQL.NotasUsuarioAsync(App.UsuarioLogin.ID);
        }

        async void OnNoteAddedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = new NotaSQL()
            });
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NoteEntryPage
                {
                    BindingContext = e.SelectedItem as NotaSQL
                });
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            //var lista = await NotaSQL.NotasUsuarioAsync(App.UsuarioLogin.ID);

            TituloNotas();
        }

        public static void TituloNotas()
        {
            string s = "";
            var total = NotaSQL.CountAsync().Result;
            var nGrupos = NotaSQL.Grupos().Count();
            var sGrupo = "";
            if (nGrupos == 0)
                sGrupo = "No hay grupos";
            else if (nGrupos == 1)
                sGrupo = "Hay 1 grupo";
            else
                sGrupo = $"Hay {nGrupos} grupos";

            sGrupo += " y ";

            if (total == 0)
                s = "ninguna nota.";
            else if (total == 1)
                s = "1 nota.";
            else
                s = $"{total} notas.";

            s = sGrupo + s;

            Current.Title = $"gsNotasNET.Android {App.AppVersion}";
            Current.LabelInfo.Text = s;
        }

        private async void CopiarEnDrive_Clicked(object sender, EventArgs e)
        {
            // Por ahora no usarlo
            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = new NotaSQL() { Texto = $"Por ahora no se sincroniza el contenido con GoogleDrive.", Grupo = "Drive-Docs" }
            }); ;
            return;

            //gsNotasNET.APIs.ApisDriveDocs.IniciadoGuardarNotasEnDrive += ApisDriveDocs_IniciadoGuardarNotasEnDrive;
            //gsNotasNET.APIs.ApisDriveDocs.FinalizadoGuardarNotasEnDrive += ApisDriveDocs_FinalizadoGuardarNotasEnDrive;
            //gsNotasNET.APIs.ApisDriveDocs.GuardandoNotas += ApisDriveDocs_GuardandoNotas;

            //var lasNotas = new Dictionary<string, List<string>>();
            //var grupos = NotaSQL.Grupos();
            //foreach(var g in grupos)
            //{
            //    if(!lasNotas.ContainsKey(g))
            //    {
            //        lasNotas.Add(g, new List<string>());
                    
            //        var colNotas = App.Database.GetNotesAsync(g);
                    
            //        var col = new List<string>();
                    
            //        foreach (var n in colNotas.Result)
            //            col.Add(n.Text);
                    
            //        lasNotas[g] = col;
            //    }
            //}
            //try
            //{
            //    var t = gsNotasNET.APIs.ApisDriveDocs.GuardarNotasDrive("", "NO", lasNotas);

            //    LabelInfo.Text = $"Se han copiado {t} notas en los documentos de Drive.";
            //}
            //catch(Exception ex)
            //{
            //    var crlf = "\r\n";
            //    await Navigation.PushAsync(new NoteEntryPage
            //    {
            //        BindingContext = new Nota() { Text = $"Error:{crlf}{ex.Message}", Grupo = "Drive-Docs" }
            //    }); ;
            //    ApisDriveDocs_FinalizadoGuardarNotasEnDrive();
            //}

            //gsNotasNET.APIs.ApisDriveDocs.IniciadoGuardarNotasEnDrive -= ApisDriveDocs_IniciadoGuardarNotasEnDrive;
            //gsNotasNET.APIs.ApisDriveDocs.FinalizadoGuardarNotasEnDrive -= ApisDriveDocs_FinalizadoGuardarNotasEnDrive;
            //gsNotasNET.APIs.ApisDriveDocs.GuardandoNotas -= ApisDriveDocs_GuardandoNotas;
        }

        private void ApisDriveDocs_GuardandoNotas(string mensaje)
        {
            LabelInfo.Text = mensaje;
        }

        private void ApisDriveDocs_FinalizadoGuardarNotasEnDrive()
        {
            Current.LabelInfo.BackgroundColor = (Color)Application.Current.Resources["NavigationBarColor"];
            Current.LabelInfo.TextColor = (Color)Application.Current.Resources["NavigationBarTextColor"];
        }

        private void ApisDriveDocs_IniciadoGuardarNotasEnDrive()
        {
            Current.LabelInfo.BackgroundColor = Color.Firebrick;
            Current.LabelInfo.TextColor = Color.White;
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}