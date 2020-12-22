using System;
using System.IO;
using Xamarin.Forms;
using gsNotasNET.Models;
using System.Xml;

namespace gsNotasNET
{
    public partial class NoteEntryPage_ant : ContentPage
    {
        public NoteEntryPage_ant()
        {
            InitializeComponent();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var note = (Nota)BindingContext;
            // no guardar notas en blanco
            if (string.IsNullOrEmpty(note.Text))
            {
                await Navigation.PopAsync();
                return;
            }

            note.Date = DateTime.UtcNow;
            await App.Database.SaveNoteAsync(note);
            await Navigation.PopAsync();
            NotesPage_ant.TituloNotas();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var note = (Nota)BindingContext;
            await App.Database.DeleteNoteAsync(note);
            await Navigation.PopAsync();
            NotesPage_ant.TituloNotas();
        }

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto la nota está asignada
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var note = (Nota)BindingContext;
            if (!(note is null) && !string.IsNullOrWhiteSpace(note.Text))
            {
                char[] returns = { '\r', '\n' };
                if (note.Text.IndexOfAny(returns) > -1)
                {
                    var s = note.Text.Split(returns, StringSplitOptions.RemoveEmptyEntries);
                    this.Title = $"# {note.ID}, {note.Date.ToString("dd/MM/yy HH:mm")}, {note.Text.Length} car. {s.Length} lín.";
                }
                else
                {
                    this.Title = $"# {note.ID}, {note.Date.ToString("dd/MM/yy HH:mm")}, {note.Text.Length} car. 1 lín.";
                }
            }
        }
    }
}