using System;
using System.IO;
using Xamarin.Forms;
using gsNotasNET.Models;
using System.Xml;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace gsNotasNET
{
    public partial class NoteEntryPage : ContentPage
    {
        public NoteEntryPage()
        {
            InitializeComponent();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var nota = (NotaSQL)BindingContext;
            // no guardar notas en blanco
            if (string.IsNullOrEmpty(nota.Texto))
            {
                await Navigation.PopAsync();
                return;
            }

            nota.Modificada = DateTime.UtcNow;
            nota.Archivada = false;


            NotaSQL.GuardarNota(nota);
            await Navigation.PopAsync();
            
            NotesPage.TituloNotas();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var nota = (NotaSQL)BindingContext;

            NotaSQL.Delete(nota);
            await Navigation.PopAsync();
            
            NotesPage.TituloNotas();
        }

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto la nota está asignada
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var nota = (NotaSQL)BindingContext;
            if (!(nota is null) && !string.IsNullOrWhiteSpace(nota.Texto))
            {
                char[] returns = { '\r', '\n' };
                if (nota.Texto.IndexOfAny(returns) > -1)
                {
                    var s = nota.Texto.Split(returns, StringSplitOptions.RemoveEmptyEntries);
                    
                    this.Title = $"#{nota.ID}, {nota.Modificada.ToString("dd/MM/yy HH:mm")}, {nota.Texto.Length} c. {s.Length} l.";
                }
                else
                {
                    this.Title = $"#{nota.ID}, {nota.Modificada.ToString("dd/MM/yy HH:mm")}, {nota.Texto.Length} c. 1 l.";
                }
            }
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}