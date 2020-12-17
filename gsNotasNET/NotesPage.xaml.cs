using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using gsNotasNET.Models;
using gsNotasNET.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

            listView.ItemsSource = await App.Database.GetNotesAsync();
        }

        async void OnNoteAddedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = new Nota()
            });
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NoteEntryPage
                {
                    BindingContext = e.SelectedItem as Nota
                });
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            //this.Title = $"gsNotasNET - Hay {App.Database.CountAsync().Result} notas";
            TituloNotas();
        }

        public static void TituloNotas()
        {
            string s = "";
            var total = App.Database.CountAsync().Result;
            //if(total == 0)
            //    s = $"gsNotasNET - No hay notas";
            //else if(total == 1)
            //    s = $"gsNotasNET - Hay 1 nota";
            //else
            //    s = $"gsNotasNET - Hay {total} notas";

            var nGrupos = App.Database.Grupos().Count();
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

            Current.Title = "gsNotasNET.Android";
            Current.LabelInfo.Text = s;
        }

        private async void CopiarEnDrive_Clicked(object sender, EventArgs e)
        {
            gsNotasNET.APIs.ApisDriveDocs.IniciadoGuardarNotasEnDrive += ApisDriveDocs_IniciadoGuardarNotasEnDrive;
            gsNotasNET.APIs.ApisDriveDocs.FinalizadoGuardarNotasEnDrive += ApisDriveDocs_FinalizadoGuardarNotasEnDrive;
            gsNotasNET.APIs.ApisDriveDocs.GuardandoNotas += ApisDriveDocs_GuardandoNotas;

            //var url = "https://accounts.google.com/o/oauth2/v2/auth?access_type=offline&response_type=code&client_id=497045764341-cromici7c1mpisc9ffcjt2buv9olfcq3.apps.googleusercontent.com&redirect_uri=http%3A%2F%2F127.0.0.1%3A42170%2Fauthorize%2F&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdocuments%20https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdrive.file";
            //await Navigation.PushAsync(new NoteEntryPage
            //{
            //    BindingContext = new Nota() { Text = url, Grupo = "Drive-Docs" }
            //}); ;

            //return;

            var lasNotas = new Dictionary<string, List<string>>();
            var grupos = App.Database.Grupos();
            foreach(var g in grupos)
            {
                if(!lasNotas.ContainsKey(g))
                {
                    lasNotas.Add(g, new List<string>());
                    
                    var colNotas = App.Database.GetNotesAsync(g);
                    
                    var col = new List<string>();
                    
                    foreach (var n in colNotas.Result)
                        col.Add(n.Text);
                    
                    lasNotas[g] = col;
                }
            }
            try
            {
                var t = gsNotasNET.APIs.ApisDriveDocs.GuardarNotasDrive("", "NO", lasNotas);

                LabelInfo.Text = $"Se han copiado {t} notas en los documentos de Drive.";
            }
            catch(Exception ex)
            {
                var url = "https://accounts.google.com/o/oauth2/v2/auth?access_type=offline&response_type=code&client_id=497045764341-cromici7c1mpisc9ffcjt2buv9olfcq3.apps.googleusercontent.com&redirect_uri=http%3A%2F%2F127.0.0.1%3A42170%2Fauthorize%2F&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdocuments%20https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdrive.file";
                var crlf = "\r\n";
                await Navigation.PushAsync(new NoteEntryPage
                {
                    BindingContext = new Nota() { Text =$"{url}{crlf}{crlf}Copia esa URL (o abre el navegador) para autorizar la aplicación.{crlf}{crlf}Error {ex.Message}", Grupo = "Drive-Docs" }
                }); ;
                ApisDriveDocs_FinalizadoGuardarNotasEnDrive();
            }

            gsNotasNET.APIs.ApisDriveDocs.IniciadoGuardarNotasEnDrive -= ApisDriveDocs_IniciadoGuardarNotasEnDrive;
            gsNotasNET.APIs.ApisDriveDocs.FinalizadoGuardarNotasEnDrive -= ApisDriveDocs_FinalizadoGuardarNotasEnDrive;
            gsNotasNET.APIs.ApisDriveDocs.GuardandoNotas -= ApisDriveDocs_GuardandoNotas;
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
    }
}