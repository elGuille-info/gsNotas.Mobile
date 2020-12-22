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
    public partial class NotesPage_ant : ContentPage
    {
        public static NotesPage_ant Current;
        public NotesPage_ant()
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
            await Navigation.PushAsync(new NoteEntryPage_ant
            {
                BindingContext = new Nota()
            });
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NoteEntryPage_ant
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
            if(total == 0)
                s = $"gsNotasNET - No hay notas";
            else if(total == 1)
                s = $"gsNotasNET - Hay 1 nota";
            else
                s = $"gsNotasNET - Hay {total} notas";

            Current.Title = s;
        }
    }
}