using System;
using System.IO;
using Xamarin.Forms;
using gsNotasNET.Models;
using System.Xml;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Linq;

namespace gsNotasNET
{
    public partial class NotaEditar : ContentPage
    {
        public NotaEditar()
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
            nota.Archivada = chkArchivada.IsToggled;
            nota.Favorita = chkFavorita.IsToggled;

            NotaSQL.GuardarNota(nota);
            await Navigation.PopAsync();
            
            NotasActivas.TituloNotas();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var nota = (NotaSQL)BindingContext;

            NotaSQL.Delete(nota);
            await Navigation.PopAsync();

            NotasActivas.TituloNotas();
        }

        /// <summary>
        /// Se produce cuando cambia el binding-context y por tanto la nota está asignada
        /// </summary>
        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var nota = (NotaSQL)BindingContext;
            if (nota is null)
                return;

            if (nota.Texto.Any())
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
            chkArchivada.IsToggled = nota.Archivada;
        }

        private void btnPrivacidad_Clicked(object sender, EventArgs e)
        {
            _ = App.MostrarPoliticaPrivacidad();
        }
    }
}